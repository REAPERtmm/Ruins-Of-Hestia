using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Room
{
    public RawRoom ReferencedRoom;
    public TileGroup[,] DualGrid;
    public Mesh GeneratedMesh;
    public Vector2Int DualGridSize;

    public Room(RawRoom raw)
    {
        ReferencedRoom = raw;
        DualGridSize = raw.GetSize() + Vector2Int.one;
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

    public Vector2Int TileGroupPerRoom { get { return TilePerRoom + Vector2Int.one; } }
    public Vector2 TileScale { get { return new Vector2(RoomScale.x / (float)(TilePerRoom.x), RoomScale.y / (float)(TilePerRoom.y)); } }
    public Vector2 TileGroupScale { get { return new Vector2(RoomScale.x / (float)(TileGroupPerRoom.x), RoomScale.y / (float)(TileGroupPerRoom.y)); } }
    public float RoomScaleX { get { return RoomScale.x / TileGroupPerRoom.x; } }
    public float RoomScaleZ { get { return RoomScale.y / TileGroupPerRoom.y; } }

    List<RawRoom> Raws;
    RoomResolver ResolverRoom;
    TileGroupResolver ResolverTileGroup;
    Room[,] Map;
    MapLayout Layout;

    public Mesh WaterMesh = null;

    public Room GetRoom(int x, int y) => Map[x, y];

    static RawRoom RoomSOToRawRoom(RoomSO room)
    {
        // TODO : Allow for different resolution of room

        RawRoom output = new RawRoom(room.Size);
        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                output.SetTile(x, y, room.Get(x, y));
            }
        }

        return output;
    }

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

        Raws = new List<RawRoom>();

        foreach (var room in rooms)
        {
            if (room.Size != TilePerRoom) continue;
            RawRoom raw = RoomSOToRawRoom(room);
            Raws.Add(raw);
        }

        ResolverRoom = new RoomResolver();
        ResolverRoom.Process(Raws, TilePerRoom);

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
                Map[x, y] = new Room(ResolverRoom.GetRoom(link).Raw);
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

public class MapGeneration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MeshTileGroup[] TilesGroups;
    [SerializeField] RoomSO[] Rooms;
    [SerializeField] GameObject RoomPrefab;
    [SerializeField] Transform Where;
    [SerializeField] Transform Boudaries;

    [Header("Water")]
    [SerializeField] GameObject WaterObject;
    [SerializeField] int TilingPerRoom;

    [Header("Parameters")]
    [SerializeField] Vector2 RoomScale;
    [SerializeField] Vector2Int RoomCount;
    [SerializeField] Vector2Int TilePerRoom;

    MapFactory Factory;

    Transform[,] RoomObjects;

    Transform CreateRoom(Room room, int x_room, int y_room)
    {
        GameObject instance = Instantiate(RoomPrefab, Where);
        instance.GetComponent<MeshFilter>().sharedMesh = Factory.GetRoom(x_room, y_room).GeneratedMesh;
        instance.GetComponent<MeshCollider>().sharedMesh = Factory.GetRoom(x_room, y_room).GeneratedMesh;

        float scaleY = (Factory.RoomScaleX + Factory.RoomScaleZ) * 0.5f;
        instance.transform.localScale = new Vector3(Factory.RoomScaleX, scaleY, Factory.RoomScaleZ);
        instance.transform.position = new Vector3(RoomScale.x * x_room, 0.0f, RoomScale.y * y_room);

        return instance.transform;
    }

    private void Start()
    {
        Factory = new MapFactory();
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

        WaterObject.transform.position = new Vector3(-OverflowX * 0.5f, -0.05f * ScaleY, -OverflowY * 0.5f);
        WaterObject.transform.localScale = new Vector3(OverScaling, 1.0f, OverScaling);

        // Room Objects :
        RoomObjects = new Transform[RoomCount.x, RoomCount.y];

        for (int x = 0; x < RoomCount.x; ++x)
        {
            for (int y = 0; y < RoomCount.y; ++y)
            {
                RoomObjects[x, y] = CreateRoom(Factory.GetRoom(x, y), x, y);
            }
        }

        // Camera Boudaries :
        Vector3 size = new Vector3(2.0f * Factory.RoomScale.x * Factory.RoomCount.x, 1000.0f, 2.0f * Factory.RoomScale.y * Factory.RoomCount.y);
        Boudaries.GetComponent<BoxCollider>().size = size;
        Boudaries.transform.position = size * 0.25f + Vector3.down * 100.0f;


    }

    //private void OnDrawGizmos()
    //{
    //    if (Layout == null || Map == null) return;

    //    Vector3 RoomSize = new Vector3(1.0f, 0.0f, 1.0f);
    //    Vector3 TileGroupSize = new Vector3(RoomSize.x / (float)TileGroupPerRoom.x, 0.0f, RoomSize.z / (float)TileGroupPerRoom.y);
    //    Vector3 TileSize = TileGroupSize * 0.5f;

    //    for (int x = 0; x < RoomCount.x; x++) {
    //        for (int y = 0; y < RoomCount.y; y++)
    //        {
    //            Vector3 roomBottomLeft = new Vector3(x, 0, y);
    //            if(Layout.StartingPoint.x == x && Layout.StartingPoint.y == y)
    //            {
    //                Gizmos.color = Color.green;
    //            }
    //            else if(Layout.EndingPoint.x == x && Layout.EndingPoint.y == y)
    //            {
    //                Gizmos.color = Color.yellow;
    //            }
    //            else Gizmos.color = Color.white;

    //            Vector3 roomCenter = roomBottomLeft + RoomSize * 0.5f;
    //            Gizmos.DrawCube(roomCenter + Vector3.down * 0.1f, RoomSize + Vector3.up * 0.2f);

    //            for (int x_tile = 0; x_tile < TileGroupPerRoom.x; ++x_tile)
    //            {
    //                for (int y_tile = 0; y_tile < TileGroupPerRoom.y; ++y_tile)
    //                {
    //                    Vector3 TileBottomLeft = roomBottomLeft + new Vector3(TileGroupSize.x * x_tile, 0.0f, TileGroupSize.z * y_tile);   

    //                    Gizmos.color = Color.red;
    //                    Gizmos.DrawWireCube(TileBottomLeft + TileGroupSize * 0.5f, TileGroupSize + Vector3.up * 0.01f);

    //                    Gizmos.color = Color.blue;
    //                    if (Map[x, y].DualGrid[x_tile, y_tile].BL)
    //                    {
    //                        Gizmos.DrawCube(TileBottomLeft + new Vector3(0.5f * TileSize.x, 0.0f, 0.5f * TileSize.z), TileSize + Vector3.up * 0.01f);
    //                    }
    //                    if (Map[x, y].DualGrid[x_tile, y_tile].BR)
    //                    {
    //                        Gizmos.DrawCube(TileBottomLeft + new Vector3(1.5f * TileSize.x, 0.0f, 0.5f * TileSize.z), TileSize + Vector3.up * 0.01f);
    //                    }
    //                    if (Map[x, y].DualGrid[x_tile, y_tile].TL)
    //                    {
    //                        Gizmos.DrawCube(TileBottomLeft + new Vector3(0.5f * TileSize.x, 0.0f, 1.5f * TileSize.z), TileSize + Vector3.up * 0.01f);
    //                    }
    //                    if (Map[x, y].DualGrid[x_tile, y_tile].TR)
    //                    {
    //                        Gizmos.DrawCube(TileBottomLeft + new Vector3(1.5f * TileSize.x, 0.0f, 1.5f * TileSize.z), TileSize + Vector3.up * 0.01f);
    //                    }
    //                }
    //            }


    //        }
    //    }

    //    for (int x = 0; x < RoomCount.x; x++)
    //    {
    //        for (int y = 0; y < RoomCount.y; y++)
    //        {
    //            Vector3 roomCenter = new Vector3(x, 0, y) + RoomSize * 0.5f;

    //            Gizmos.color = Color.white;
    //            if (Layout.Layout[x, y].Up)
    //            {
    //                Gizmos.DrawCube(roomCenter + Vector3.up * 0.2f + Vector3.forward * 0.25f, new Vector3(0.1f, 0.1f, 0.5f));
    //            }
    //            if (Layout.Layout[x, y].Down)
    //            {
    //                Gizmos.DrawCube(roomCenter + Vector3.up * 0.2f + Vector3.back * 0.25f, new Vector3(0.1f, 0.1f, 0.5f));
    //            }
    //            if (Layout.Layout[x, y].Left)
    //            {
    //                Gizmos.DrawCube(roomCenter + Vector3.up * 0.2f + Vector3.left * 0.25f, new Vector3(0.5f, 0.1f, 0.1f));
    //            }
    //            if (Layout.Layout[x, y].Right)
    //            {
    //                Gizmos.DrawCube(roomCenter + Vector3.up * 0.2f + Vector3.right * 0.25f, new Vector3(0.5f, 0.1f, 0.1f));
    //            }
    //        }
    //    }

    //}

}
