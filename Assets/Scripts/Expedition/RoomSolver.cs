using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
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

        float half_x = (Size.x - 1) * 0.5f;
        float half_y = (Size.y - 1) * 0.5f;

        for (int x = 0; x < Size.x; x++) {
            for (int y = 0; y < Size.y; y++)
            {
                float centered_x = (float)x - half_x;
                float centered_y = (float)y - half_y;

                Vector2Int new_coord = new Vector2Int(
                    (int)(centered_x * matrix[0, 0] + centered_y * matrix[0, 1] + half_x),
                    (int)(centered_x * matrix[1, 0] + centered_y * matrix[1, 1] + half_y)
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
    // TODO : Add SpawnPoint, ennemi spawn, resource spawn, etc...
}

public class RoomResolver
{
    List<RawRoom> Raws;
    Dictionary<LayoutLinking, List<RoomDescriptor>> Resolver;

    public RoomResolver()
    {
        Resolver = new();
    }
    
    public void Process(List<RawRoom> raws, Vector2Int RoomSize)
    {
        Raws = raws;

        for (int i = 0; i < Raws.Count; ++i)
        {
            for(int rotation = 0; rotation < 4; ++rotation)
            {
                Vector2Int size = Raws[i].RotateSize(rotation);
                if (size.x != RoomSize.x || size.y != RoomSize.y) continue;

                RawRoom new_raw = Raws[i].Rotate(rotation);
                if(Resolver.ContainsKey(new_raw.GetLinks()) == false)
                    Resolver.Add(new_raw.GetLinks(), new List<RoomDescriptor>());

                RoomDescriptor desc = new RoomDescriptor();
                desc.Raw = new_raw;

                Resolver[new_raw.GetLinks()].Add(desc);
            }
        }
    }

    public RoomDescriptor GetRoom(LayoutLinking linking) {
        if (Resolver.ContainsKey(linking) == false) return null;
        var rooms = Resolver[linking];
        return rooms[Random.Range(0, rooms.Count)];
    }
}

