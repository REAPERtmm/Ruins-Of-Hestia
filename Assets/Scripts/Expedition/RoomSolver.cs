using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

public class RawRoom
{
    Vector2Int Size;
    bool[,] Tiles;
    LayoutLinking Links;

    public RawRoom(Vector2Int size)
    {
        Size = size;
        Tiles = new bool[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; ++y)
            {
                Tiles[x, y] = false;
            }
        }
        Links.Left = false;
        Links.Right = false;
        Links.Up = false;
        Links.Down = false;
    }

    public void GenerateRandom(float filledRatio)
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; ++y)
            {
                Tiles[x, y] = Random.Range(0.0f, 1.0f) < filledRatio;
            }
        }
        UpdateLinks();
    }

    public Vector2Int GetSize() => Size;
    public bool GetTile(int x, int y) => Tiles[x, y];
    public LayoutLinking GetLinks() => Links;
    public void SetTile(int x, int y, bool status)
    {
        Tiles[x, y] = status;
        UpdateLinks();
    }

    public Vector2Int RotateSize(int rotation)
    {
        int x = Size.x;
        int y = Size.y;

        while (rotation > 0)
        {
            x = x ^ y;
            y = x ^ y;
            x = x ^ y;
            --rotation;
        }

        return new Vector2Int(x, y);
    }

    public LayoutLinking RotateLinks(int rotation)
    {
        LayoutLinking link = Links;

        while (rotation > 0) {

            bool left = link.Left;
            link.Left = link.Down;
            link.Down = link.Right;
            link.Right = link.Up;
            link.Up = left;

            --rotation;
        }

        return link;
    }

    public RawRoom Rotate(int rotation)
    {
        float[,] matrix = new float[2, 2]
        {
            { Mathf.Cos(Mathf.PI * 0.5f * rotation), Mathf.Sin(Mathf.PI * 0.5f * rotation) },
            { -Mathf.Sin(Mathf.PI * 0.5f * rotation), Mathf.Cos(Mathf.PI * 0.5f * rotation) },
        };

        RawRoom raw = new RawRoom(RotateSize(rotation));

        float half_x = (float)(Size.x - 1) * 0.5f;
        float half_y = (float)(Size.y - 1) * 0.5f;

        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++)
            {
                float centered_x = (float)x - half_x;
                float centered_y = (float)y - half_y;

                Vector2Int new_coord = new Vector2Int(
                    Mathf.RoundToInt(centered_x * matrix[0, 0] + centered_y * matrix[0, 1] + half_x),
                    Mathf.RoundToInt(centered_x * matrix[1, 0] + centered_y * matrix[1, 1] + half_y)
                );

                raw.Tiles[new_coord.x, new_coord.y] = Tiles[x, y];
            }
        }

        raw.UpdateLinks();

        return raw;
    }

    void UpdateLinks()
    {
        Links.Left = false;
        Links.Right = false;
        Links.Up = false;
        Links.Down = false;
        for (int x = 0; x < Size.x; x++) {
            if (Tiles[x, 0]) Links.Down = true;
            if (Tiles[x, Size.y - 1]) Links.Up = true;
        }
        for (int y = 0; y < Size.y; y++)
        {
            if (Tiles[0, y]) Links.Left = true;
            if (Tiles[Size.x - 1, y]) Links.Right = true;
        }
    }
}


public class RoomDescriptor
{
    public RawRoom Raw;
    public List<Vector2> NormalizedPositionSpawnEnnemies;
    public List<Vector2> NormalizedPositionSpawnResources;
    public Vector2 NormalizedPositionSpawnPlayer;
    // TODO : Add SpawnPoint, ennemi spawn, resource spawn, etc...
}

public class RoomResolver
{
    public static RawRoom RoomSOToRawRoom(RoomSO room)
    {
        // TODO : Allow for different resolution of room

        RawRoom output = new RawRoom(room.Size);
        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                output.SetTile(x, y, room.GetTerrain(x, y));
            }
        }

        return output;
    }

    RoomSO[] Rooms;
    Dictionary<LayoutLinking, List<RoomDescriptor>> Resolver;

    public RoomResolver()
    {
        Resolver = new();
    }
    
    public void Register(RawRoom raw, List<Vector2> ennemies, List<Vector2> resources, Vector2 player)
    {
        if (Resolver.ContainsKey(raw.GetLinks()) == false)
            Resolver.Add(raw.GetLinks(), new List<RoomDescriptor>());

        RoomDescriptor desc = new RoomDescriptor();
        desc.Raw = raw;
        desc.NormalizedPositionSpawnEnnemies = ennemies;
        desc.NormalizedPositionSpawnResources = resources;
        desc.NormalizedPositionSpawnPlayer = player;

        Resolver[raw.GetLinks()].Add(desc);
    }

    public void Process(RoomSO[] rooms, Vector2Int RoomSize)
    {
        Rooms = rooms;

        for (int i = 0; i < Rooms.Length; ++i)
        {
            RawRoom raw = RoomSOToRawRoom(Rooms[i]);
            List<Vector2> ennemies = new List<Vector2>();
            List<Vector2> resources = new List<Vector2>();
            Vector2 player = new Vector2(
                (Rooms[i].PlayerSpawn.x + 0.5f) / Rooms[i].Size.x,
                (Rooms[i].PlayerSpawn.y + 0.5f) / Rooms[i].Size.y
            );
            
            for (int x = 0; x < Rooms[i].Size.x; ++x)
            {
                for (int y = 0; y < Rooms[i].Size.y; ++y)
                {
                    if (Rooms[i].GetEnnemi(x, y))
                    {
                        ennemies.Add(new Vector2(
                                (x + 0.5f) / Rooms[i].Size.x,
                                (y + 0.5f) / Rooms[i].Size.y
                            ));
                    }

                    if (Rooms[i].GetResource(x, y))
                    {
                        resources.Add(new Vector2(
                                (x + 0.5f) / Rooms[i].Size.x,
                                (y + 0.5f) / Rooms[i].Size.y
                            ));
                    }
                }
            }

            for (int rotation = 0; rotation < 4; ++rotation)
            {
                float[,] matrix = new float[2, 2]
                {
                     { Mathf.Cos(Mathf.PI * 0.5f * rotation), Mathf.Sin(Mathf.PI * 0.5f * rotation) },
                     { -Mathf.Sin(Mathf.PI * 0.5f * rotation), Mathf.Cos(Mathf.PI * 0.5f * rotation) },
                };

                RawRoom new_raw = raw.Rotate(rotation);
                List<Vector2> new_ennemies = new List<Vector2>();
                List<Vector2> new_resources = new List<Vector2>();
                Vector2 new_player = new Vector2();

                Vector2 centered = player - Vector2.one * 0.5f;
                Vector2 rotated = new Vector2(
                       centered.x * matrix[0, 0] + centered.y * matrix[0, 1],
                       centered.x * matrix[1, 0] + centered.y * matrix[1, 1]
                    );
                new_player = rotated + Vector2.one * 0.5f;

                foreach (var ennemi in ennemies)
                {
                    centered = ennemi - Vector2.one * 0.5f;
                    rotated = new Vector2(
                           centered.x * matrix[0, 0] + centered.y * matrix[0, 1],
                           centered.x * matrix[1, 0] + centered.y * matrix[1, 1]
                        );
                    new_ennemies.Add(rotated + Vector2.one * 0.5f);
                }
                foreach (var resource in resources)
                {
                    centered = resource - Vector2.one * 0.5f;
                    rotated = new Vector2(
                           centered.x * matrix[0, 0] + centered.y * matrix[0, 1],
                           centered.x * matrix[1, 0] + centered.y * matrix[1, 1]
                        );
                    new_resources.Add(rotated + Vector2.one * 0.5f);
                }

                Register(new_raw, new_ennemies, new_resources, new_player);
            }
        }
    }

    public RoomDescriptor GetRoom(LayoutLinking linking) {
        if (Resolver.ContainsKey(linking) == false) return null;
        var rooms = Resolver[linking];
        return rooms[Random.Range(0, rooms.Count)];
    }
}

