using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Room
{
    public RawRoom ReferencedRoom;
    public Vector2[] NormalizedPositionSpawnEnnemies;
    public Vector2[] NormalizedPositionSpawnResources;
    public Vector2 NormalizedPositionSpawnPlayer;
    public TileGroup[,] DualGrid;
    public Mesh GeneratedMesh;
    public Vector2Int DualGridSize;

    public Vector2 GetPlayerSpawn(Vector2 rawRoomScale)
    {
         return new Vector2(NormalizedPositionSpawnPlayer.x * rawRoomScale.x, NormalizedPositionSpawnPlayer.y * rawRoomScale.y);
    }

    public IEnumerable<Vector2> EnnemiSpawns(Vector2 rawRoomScale)
    {
        foreach(var ennemi in NormalizedPositionSpawnEnnemies)
        {
            yield return new Vector2(ennemi.x * rawRoomScale.x, ennemi.y * rawRoomScale.y);
        }
    }

    public IEnumerable<Vector2> ResourceSpawns(Vector2 rawRoomScale)
    {
        foreach (var resource in NormalizedPositionSpawnResources)
        {
            yield return new Vector2(resource.x * rawRoomScale.x, resource.y * rawRoomScale.y);
        }
    }

    public Room(RoomDescriptor desc)
    {
        ReferencedRoom = desc.Raw;
        NormalizedPositionSpawnEnnemies = desc.NormalizedPositionSpawnEnnemies.ToArray();
        NormalizedPositionSpawnResources = desc.NormalizedPositionSpawnResources.ToArray();
        NormalizedPositionSpawnPlayer = desc.NormalizedPositionSpawnPlayer;
        DualGridSize = desc.Raw.GetSize() + Vector2Int.one;
    }

    public void GenerateDualGridWithDoors(bool[] Left, bool[] Right, bool[] Top, bool[] Bottom)
    {
        DualGrid = new TileGroup[DualGridSize.x, DualGridSize.y];

        for (int x = 0; x < DualGridSize.x; x++)
        {
            for (int y = 0; y < DualGridSize.y; y++)
            {
                DualGrid[x, y] = new TileGroup();
            }
        }

        for (int x = 0; x < ReferencedRoom.GetSize().x; x++) {
            for (int y = 0; y < ReferencedRoom.GetSize().y; y++)
            {
                DualGrid[x, y].TR           = ReferencedRoom.GetTile(x, y);
                DualGrid[x + 1, y].TL       = ReferencedRoom.GetTile(x, y);
                DualGrid[x, y + 1].BR       = ReferencedRoom.GetTile(x, y);
                DualGrid[x + 1, y + 1].BL   = ReferencedRoom.GetTile(x, y);
            }

            DualGrid[x, 0].BR = Bottom[x];
            DualGrid[x + 1, 0].BL = Bottom[x];

            DualGrid[x, ReferencedRoom.GetSize().y].TR = Top[x];
            DualGrid[x + 1, ReferencedRoom.GetSize().y].TL = Top[x];

            DualGrid[0, x].TL = Left[x];
            DualGrid[0, x + 1].BL = Left[x];

            DualGrid[ReferencedRoom.GetSize().x, x].TR = Right[x];
            DualGrid[ReferencedRoom.GetSize().x, x + 1].BR = Right[x];
        }

    }

    public void GenerateMeshWithResolver(TileGroupResolver resolver)
    {
        if (DualGrid == null) return;

        Quaternion[] quaternions = new Quaternion[4]
        {
            Quaternion.identity,
            Quaternion.AngleAxis(-90, Vector3.up),
            Quaternion.AngleAxis(-180, Vector3.up),
            Quaternion.AngleAxis(-270, Vector3.up),
        };

        List<CombineInstance> combineInstances = new List<CombineInstance>();
        for (int x = 0; x < DualGridSize.x; x++) {
            for (int y = 0; y < DualGridSize.y; y++) {
                if (DualGrid[x, y].IsEmpty()) continue;

                MeshRotation meshRotation = resolver.Get(DualGrid[x, y]);
                int random = Random.Range(0, meshRotation.Models.Length);
                Mesh model = meshRotation.Models[random];

                CombineInstance combine = new CombineInstance();
                combine.transform = Matrix4x4.TRS(
                    new Vector3(x + 0.5f, 0, y + 0.5f),
                    quaternions[meshRotation.Rotation],
                    new Vector3(0.1f, 0.1f, 0.1f)
                );
                combine.mesh = model;

                for(int i = 0; i < model.subMeshCount; ++i)
                {
                    combine.subMeshIndex = i;
                    combineInstances.Add(combine);
                }

            }
        }

        GeneratedMesh = new Mesh();
        GeneratedMesh.indexFormat = IndexFormat.UInt32;
        GeneratedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        GeneratedMesh.Optimize();
        GeneratedMesh.OptimizeIndexBuffers();
        GeneratedMesh.OptimizeReorderVertexBuffer();
    }
}

public class MapFactory
{
    public int Seed = -1;

    public Vector2 RoomScale;
    public Vector2Int RoomCount;
    public Vector2Int TilePerRoom;
    public int WaterTilingPerRoom;

    public Vector2 RawRoomScale { get {
            return new Vector2(
                RoomScale.x * ((float)TilePerRoom.x / (TilePerRoom.x + 1)),
                RoomScale.y * ((float)TilePerRoom.y / (TilePerRoom.y + 1))
                ); } }
    public Vector2Int TileGroupPerRoom { get { return TilePerRoom + Vector2Int.one; } }
    public Vector2 TileScale { get { return new Vector2(RoomScale.x / (float)(TilePerRoom.x), RoomScale.y / (float)(TilePerRoom.y)); } }
    public Vector2 TileGroupScale { get { return new Vector2(RoomScale.x / (float)(TileGroupPerRoom.x), RoomScale.y / (float)(TileGroupPerRoom.y)); } }
    public float RoomScaleX { get { return RoomScale.x / TileGroupPerRoom.x; } }
    public float RoomScaleZ { get { return RoomScale.y / TileGroupPerRoom.y; } }

    RoomSO[] Rooms;
    RoomResolver ResolverRoom;
    TileGroupResolver ResolverTileGroup;
    Room[,] Map;
    MapLayout Layout;

    public Vector2 StartPosition { get { return Map[0, 0].GetPlayerSpawn(RawRoomScale); } }
    public Vector2 EndPosition { get { return Map[RoomCount.x - 1, RoomCount.y - 1].GetPlayerSpawn(RawRoomScale) + new Vector2((RoomCount.x - 1) * RoomScale.x, (RoomCount.y - 1) * RoomScale.y); } }

    public Mesh WaterMesh = null;

    public Room GetRoom(int x, int y) => Map[x, y];

    void CreateDoor(int x, int y, out bool[] left, out bool[] right, out bool[] top, out bool[] bottom)
    {
        left = new bool[TilePerRoom.y];
        right = new bool[TilePerRoom.y];
        top = new bool[TilePerRoom.x];
        bottom = new bool[TilePerRoom.x];

        int firstLeft = TilePerRoom.y;
        int lastLeft = 0;
        int firstRight = TilePerRoom.y;
        int lastRight = 0;
        for (int i = 0; i < TilePerRoom.y; i++)
        {
            bool currentRoomLeft = Map[x, y].ReferencedRoom.GetTile(0, i);
            bool currentRoomRight = Map[x, y].ReferencedRoom.GetTile(TilePerRoom.x - 1, i);

            bool otherRoomLeft = (x > 0 ? Map[x - 1, y].ReferencedRoom.GetTile(TilePerRoom.x - 1, i) : false);
            bool otherRoomRight = (x < RoomCount.x - 1 ? Map[x + 1, y].ReferencedRoom.GetTile(0, i) : false);

            if (currentRoomLeft || otherRoomLeft)
            {
                lastLeft = i;
                if (firstLeft > i)
                {
                    firstLeft = i;
                }
            }

            if (currentRoomRight || otherRoomRight)
            {
                lastRight = i;
                if (firstRight > i)
                {
                    firstRight = i;
                }
            }
        }
        for (int i = 0; i < TilePerRoom.y; i++)
        {
            if (i >= firstLeft && i <= lastLeft) left[i] = true;
            if (i >= firstRight && i <= lastRight) right[i] = true;
        }

        int firstTop = TilePerRoom.x;
        int lastTop = 0;
        int firstBottom = TilePerRoom.x;
        int lastBottom = 0;
        for (int i = 0; i < TilePerRoom.x; i++)
        {
            bool currentRoomTop = Map[x, y].ReferencedRoom.GetTile(i, TilePerRoom.y - 1);
            bool currentRoomBottom = Map[x, y].ReferencedRoom.GetTile(i, 0);

            bool otherRoomTop = (y < RoomCount.y - 1 ? Map[x, y + 1].ReferencedRoom.GetTile(i, 0) : false);
            bool otherRoomBottom = (y > 0 ? Map[x, y - 1].ReferencedRoom.GetTile(i, TilePerRoom.y - 1) : false);

            if (currentRoomTop || otherRoomTop)
            {
                lastTop = i;
                if (firstTop > i)
                {
                    firstTop = i;
                }
            }

            if (currentRoomBottom || otherRoomBottom)
            {
                lastBottom = i;
                if (firstBottom > i)
                {
                    firstBottom = i;
                }
            }
        }
        for (int i = 0; i < TilePerRoom.x; i++)
        {
            if (i >= firstTop && i <= lastTop) top[i] = true;
            if (i >= firstBottom && i <= lastBottom) bottom[i] = true;
        }
    }

    void GenerateWaterMesh()
    {
        WaterMesh = new Mesh();

        int SIZE_X = RoomCount.x * WaterTilingPerRoom;
        int SIZE_Y = RoomCount.y * WaterTilingPerRoom;
        int bufferSize = SIZE_X * SIZE_Y;
        Vector3[] Vertices = new Vector3[bufferSize];
        Vector2[] UV = new Vector2[bufferSize];
        Vector3[] Normal = new Vector3[bufferSize];
        List<int> indices = new List<int>();

        int bufferIndex = 0;
        for (int x = 0; x < SIZE_X; ++x)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                float Normalized_x = (float)x / (float)(SIZE_X - 1);
                float Normalized_y = (float)y / (float)(SIZE_Y - 1);

                Vertices[bufferIndex] = new Vector3(Normalized_x * RoomScale.x * RoomCount.x, 0, Normalized_y * RoomScale.y * RoomCount.y);
                UV[bufferIndex] = new Vector2(Normalized_x, Normalized_y);
                Normal[bufferIndex] = new Vector3(0, 1, 0);

                int currentIndex = bufferIndex;
                int NextLineIndex = bufferIndex + SIZE_X;
                bufferIndex++;
                if (x == SIZE_X - 1 || y == SIZE_Y - 1) continue;

                // TODO register indices
                indices.Add(currentIndex);
                indices.Add(currentIndex + 1);
                indices.Add(NextLineIndex);
                indices.Add(currentIndex + 1);
                indices.Add(NextLineIndex + 1);
                indices.Add(NextLineIndex);
            }
        }

        WaterMesh.SetVertices(Vertices);
        WaterMesh.SetUVs(0, UV);
        WaterMesh.SetNormals(Normal);
        WaterMesh.SetIndices(indices, MeshTopology.Triangles, 0);
    }

    public void Generate(MeshTileGroup[] meshTileGroups, RoomSO[] rooms) 
    { 
        if(Seed != -1)
        {
            Random.InitState(Seed);
        }

        GenerateWaterMesh();

        Rooms = rooms;

        ResolverRoom = new RoomResolver();
        ResolverRoom.Process(Rooms, TilePerRoom);

        ResolverTileGroup = new TileGroupResolver();
        ResolverTileGroup.Process(meshTileGroups);

        Layout = new MapLayout(RoomCount);
        while (Layout.Solve() == false) { }

        Map = new Room[RoomCount.x, RoomCount.y];

        // TODO : Compute Real Doors
        bool[] default_door_x = new bool[TileGroupPerRoom.x];
        bool[] default_door_y = new bool[TileGroupPerRoom.y];

        for (int i = 0; i < TileGroupPerRoom.x; i++) default_door_x[i] = true;
        for (int i = 0; i < TileGroupPerRoom.y; i++) default_door_y[i] = true;

        for (int x = 0; x < RoomCount.x; ++x)
        {
            for (int y = 0; y < RoomCount.y; ++y)
            {
                var link = Layout.Layout[x, y];
                Map[x, y] = new Room(ResolverRoom.GetRoom(link));
            }
        }

        for (int x = 0; x < RoomCount.x; ++x)
        {
            for (int y = 0; y < RoomCount.y; ++y)
            {
                bool[] LeftDoor;
                bool[] RightDoor;
                bool[] TopDoor;
                bool[] BottomDoor;
                CreateDoor(x, y, out LeftDoor, out RightDoor, out TopDoor, out BottomDoor);

                Map[x, y].GenerateDualGridWithDoors(LeftDoor, RightDoor, TopDoor, BottomDoor);
                Map[x, y].GenerateMeshWithResolver(ResolverTileGroup);
            }
        }

    }
}

[ExecuteAlways]
public class MapGeneration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshTileGroup[] TilesGroups;
    [SerializeField] RoomSO[] Rooms;
    [SerializeField] Transform Boudaries;
    [SerializeField] Transform Player;
    [SerializeField] RawImage MiniMapImage;

    [Header("Prefabs")]
    [SerializeField] GameObject RoomPrefab;
    [SerializeField] Transform RoomWhere;
    [SerializeField] GameObject EnnemiPrefab;
    [SerializeField] Transform EnnemiWhere;
    [SerializeField] GameObject ResourcePrefab;
    [SerializeField] Transform ResourceWhere;

    [Header("Water")]
    [SerializeField] GameObject WaterObject;
    [SerializeField] int TilingPerRoom;

    [Header("Parameters")]
    [SerializeField] Vector2 RoomScale;
    [SerializeField] Vector2Int RoomCount;
    [SerializeField] Vector2Int TilePerRoom;

    [Header("Debug")]
    [SerializeField] bool DebugTiles;
    [SerializeField] bool GENERATE;
    [SerializeField] bool DESTROY;
    [SerializeField] int Seed = -1;
    [SerializeField] Texture2D MiniMap;

    MapFactory Factory = null;
    Transform[,] RoomObjects;

    Transform CreateRoom(Room room, int x_room, int y_room)
    {
        GameObject instance = Instantiate(RoomPrefab, RoomWhere);
        instance.GetComponent<MeshFilter>().sharedMesh = Factory.GetRoom(x_room, y_room).GeneratedMesh;
        instance.GetComponent<MeshCollider>().sharedMesh = Factory.GetRoom(x_room, y_room).GeneratedMesh;

        float scaleY = (Factory.RoomScaleX + Factory.RoomScaleZ) * 0.5f;
        instance.transform.localScale = new Vector3(Factory.RoomScaleX, scaleY, Factory.RoomScaleZ);
        instance.transform.position = new Vector3(RoomScale.x * x_room, 0.0f, RoomScale.y * y_room);

        return instance.transform;
    }

    Transform CreateResource(Vector3 position)
    {
        GameObject instance = Instantiate(ResourcePrefab, ResourceWhere);

        float random_offset_x = Random.Range(-Factory.TileGroupScale.x, Factory.TileGroupScale.x) * 0.35f;
        float random_offset_y = Random.Range(-Factory.TileGroupScale.y, Factory.TileGroupScale.y) * 0.35f;
        Vector3 random_offset = new Vector3(random_offset_x, 0, random_offset_y);

        instance.transform.position = position + random_offset;
        return instance.transform;
    }

    Transform CreateEnnemi(Vector3 position)
    {
        GameObject instance = Instantiate(EnnemiPrefab, EnnemiWhere);

        float random_offset_x = Random.Range(-Factory.TileGroupScale.x, Factory.TileGroupScale.x) * 0.35f;
        float random_offset_y = Random.Range(-Factory.TileGroupScale.y, Factory.TileGroupScale.y) * 0.35f;
        Vector3 random_offset = new Vector3(random_offset_x, 0, random_offset_y);

        instance.transform.position = position + random_offset;
        return instance.transform;
    }

    public void GenerateMiniMapTemp()
    {
        int width = RoomCount.x * Factory.TileGroupPerRoom.x * 2;
        int height = RoomCount.y * Factory.TileGroupPerRoom.y * 2;
        MiniMap = new Texture2D(width, height, TextureFormat.RGBA32, false);
        MiniMap.filterMode = FilterMode.Point;
        MiniMap.wrapMode = TextureWrapMode.Clamp;
        MiniMap.anisoLevel = 0;

        Color TERRAIN = Color.white;
        Color WATER = new Color(0, 0, 0, 0);

        for (int x_room = 0; x_room < RoomCount.x; x_room++) {
            for (int y_room = 0; y_room < RoomCount.y; y_room++) {

                int X = x_room * Factory.TileGroupPerRoom.x * 2; 
                int Y = y_room * Factory.TileGroupPerRoom.y * 2; 
                Room ROOM = Factory.GetRoom(x_room, y_room);

                for (int x_tileGroup = 0; x_tileGroup < Factory.TileGroupPerRoom.x; x_tileGroup++)
                {
                    for (int y_tileGroup = 0; y_tileGroup < Factory.TileGroupPerRoom.y; y_tileGroup++)
                    {
                        int X2 = X + x_tileGroup * 2;
                        int Y2 = Y + y_tileGroup * 2;
                        var TILE_GROUP = ROOM.DualGrid[x_tileGroup, y_tileGroup];

                        MiniMap.SetPixel(X2, Y2, TILE_GROUP.BL ? TERRAIN : WATER);
                        MiniMap.SetPixel(X2 + 1, Y2, TILE_GROUP.BR ? TERRAIN : WATER);
                        MiniMap.SetPixel(X2, Y2 + 1, TILE_GROUP.TL ? TERRAIN : WATER);
                        MiniMap.SetPixel(X2 + 1, Y2 + 1, TILE_GROUP.TR ? TERRAIN : WATER);
                    }
                }

            }
        }
        MiniMap.Apply(false, false);

        MiniMapImage.color = Color.white;
        MiniMapImage.texture = MiniMap;
    }

    public void Generate(int seed = -1)
    {
        Factory = new MapFactory();
        Factory.Seed = seed;
        Factory.RoomCount = RoomCount;
        Factory.RoomScale = RoomScale;
        Factory.TilePerRoom = TilePerRoom;
        Factory.WaterTilingPerRoom = TilingPerRoom;

        Factory.Generate(TilesGroups, Rooms);

        // Water Object :
        WaterObject.GetComponent<MeshFilter>().sharedMesh = Factory.WaterMesh;

        const float OverScaling = 1.5f;
        float OverflowX = Factory.RoomCount.x * Factory.RoomScale.x * (OverScaling - 1.0f);
        float OverflowY = Factory.RoomCount.y * Factory.RoomScale.y * (OverScaling - 1.0f);

        float ScaleY = (Factory.RoomScale.x + Factory.RoomScale.y) * 0.5f;

        WaterObject.transform.position = new Vector3(-OverflowX * 0.5f, -0.1f * ScaleY, -OverflowY * 0.5f);
        WaterObject.transform.localScale = new Vector3(OverScaling, 1.0f, OverScaling);

        // Room Objects :
        RoomObjects = new Transform[RoomCount.x, RoomCount.y];

        for (int x = 0; x < RoomCount.x; ++x)
        {
            for (int y = 0; y < RoomCount.y; ++y)
            {
                Room room = Factory.GetRoom(x, y);
                Vector3 roomPosition = new Vector3(x * RoomScale.x, 0, y * RoomScale.y);
                RoomObjects[x, y] = CreateRoom(room, x, y);
                foreach (var ennemi in room.EnnemiSpawns(Factory.RawRoomScale))
                {
                    CreateEnnemi(new Vector3(ennemi.x, 0, ennemi.y) + roomPosition + new Vector3(Factory.TileGroupScale.x, 0, Factory.TileGroupScale.y) * 0.5f);
                }
                foreach (var resource in room.ResourceSpawns(Factory.RawRoomScale))
                {
                    CreateResource(new Vector3(resource.x, 0, resource.y) + roomPosition + new Vector3(Factory.TileGroupScale.x, 0, Factory.TileGroupScale.y) * 0.5f);
                }
            }
        }

        // Camera Boudaries :
        Vector3 size = new Vector3(2.0f * Factory.RoomScale.x * Factory.RoomCount.x, 1000.0f, 2.0f * Factory.RoomScale.y * Factory.RoomCount.y);
        Boudaries.GetComponent<BoxCollider>().size = size;
        Boudaries.transform.position = size * 0.25f + Vector3.down * 100.0f;

        Vector2 start = Factory.StartPosition;
        Player.position = new Vector3(start.x, 0, start.y);

        GenerateMiniMapTemp();
    }

    public void DestroyGeneration()
    {
        while(RoomWhere.childCount > 0)
        {
            DestroyImmediate(RoomWhere.GetChild(0).gameObject);
        }
        while (ResourceWhere.childCount > 0)
        {
            DestroyImmediate(ResourceWhere.GetChild(0).gameObject);
        } 
        while (EnnemiWhere.childCount > 0)
        {
            DestroyImmediate(EnnemiWhere.GetChild(0).gameObject);
        }
        Factory = null;
    }

    public void Update()
    {
        if (GENERATE)
        {
            if(Factory != null) DestroyGeneration();
            Generate(Seed);
            GENERATE = false;
        }

        if (DESTROY)
        {
            DestroyGeneration();
            DESTROY = false;
        }
    }

    private void Start()
    {
        if (Factory == null)
        {
            DestroyGeneration();
            Generate();
        }
    }


    private void OnDrawGizmos()
    {
        if (Factory == null || DebugTiles == false) return;
        Gizmos.color = new Color(1, 0, 1, 0.5f);

        Vector3 Size = new Vector3(Factory.TileScale.x, 0.5f, Factory.TileScale.y);

        for (int x = 0; x < RoomCount.x; ++x)
        {
            for (int y = 0; y < RoomCount.y; ++y)
            {
                Room room = Factory.GetRoom(x, y);
                Vector3 roomPosition = new Vector3(x * RoomScale.x, 0, y * RoomScale.y);

                for (int x_tile = 0; x_tile < TilePerRoom.x; ++x_tile) {
                    for (int y_tile = 0; y_tile < TilePerRoom.y; ++y_tile)
                    {
                        if (room.ReferencedRoom.GetTile(x_tile, y_tile) == false) continue;
                        Vector3 tilePosition = roomPosition + new Vector3(
                            Factory.TileGroupScale.x * (x_tile + 0.5f),
                            0,
                            Factory.TileGroupScale.y * (y_tile + 0.5f)
                            );
                        Gizmos.DrawCube(tilePosition + Size * 0.5f, Size);                        
                    }
                }


            }
        }
    }

}
