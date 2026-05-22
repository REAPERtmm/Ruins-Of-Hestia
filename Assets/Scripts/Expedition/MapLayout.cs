using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct LayoutLinking
{
    public bool Left;
    public bool Right;
    public bool Up;
    public bool Down;

    public void Link(int orientationIndex)
    {
        switch (orientationIndex)
        {
            case 0: Left = true; break;
            case 1: Right = true; break;
            case 2: Up = true; break;
            case 3: Down = true; break;
            default: break;
        }
    }

    public static int reverseOrientation(int orientationIndex)
    {
        switch (orientationIndex)
        {
            case 0: return 1;
            case 1: return 0;
            case 2: return 3;
            case 3: return 2;
            default: return 0;
        }
    }
}

public class MapLayout
{
    public Vector2Int MapSize;
    public LayoutLinking[,] Layout;
    public Vector2Int StartingPoint;
    public Vector2Int EndingPoint;

    public MapLayout(Vector2Int mapSize)
    {
        MapSize = mapSize;
        Layout = new LayoutLinking[MapSize.x, MapSize.y];
    }

    public bool Solve()
    {
        StartingPoint = new Vector2Int(0, 0);
        EndingPoint = MapSize - Vector2Int.one;

        HashSet<Vector2Int> VisitedFromStart = new();
        HashSet<Vector2Int> VisitedFromEnd = new();
        VisitedFromStart.Add(StartingPoint);
        VisitedFromEnd.Add(EndingPoint);
        Vector2Int currentFromStart = StartingPoint;
        Vector2Int currentFromEnd = EndingPoint;

        // Link Start and End
        while (true)
        {
            int OrientationFromStart;
            int OrientationFromEnd;
            Vector2Int NextFromStart = ChooseRandomNeighboorExclude(VisitedFromStart, currentFromStart, out OrientationFromStart);
            Vector2Int NextFromEnd = ChooseRandomNeighboorExclude(VisitedFromEnd, currentFromEnd, out OrientationFromEnd);
            bool stuck = true;
            bool finished = false;

            if (OrientationFromStart != -1)
            {
                Layout[currentFromStart.x, currentFromStart.y].Link(OrientationFromStart);
                Layout[NextFromStart.x, NextFromStart.y].Link(LayoutLinking.reverseOrientation(OrientationFromStart));
                VisitedFromStart.Add(NextFromStart);
                currentFromStart = NextFromStart;
                stuck = false;
                if (VisitedFromEnd.Contains(currentFromStart))
                {
                    finished = true;
                }
            }
            if (OrientationFromEnd != -1)
            {
                Layout[currentFromEnd.x, currentFromEnd.y].Link(OrientationFromEnd);
                Layout[NextFromEnd.x, NextFromEnd.y].Link(LayoutLinking.reverseOrientation(OrientationFromEnd));
                VisitedFromEnd.Add(NextFromEnd);
                currentFromEnd = NextFromEnd;
                stuck = false;
                if (VisitedFromStart.Contains(currentFromEnd))
                {
                    finished = true;
                }
            }

            if (finished)
            {
                break;
            }

            if (stuck)
            {
                return false;
            }

        }

        HashSet<Vector2Int> Visited = VisitedFromStart.Union(VisitedFromEnd).ToHashSet();
        Vector2Int[] Remaining = new Vector2Int[MapSize.x * MapSize.y];
        int count = 0;
        for(int i = 0; i < MapSize.x; ++i)
        {
            for(int j = 0; j < MapSize.y; ++j)
            {
                Vector2Int tested = new Vector2Int(i, j);
                if (VisitedFromStart.Contains(tested) || VisitedFromEnd.Contains(tested)) continue;
                Remaining[count++] = tested;
            }
        }

        while(count > 0)
        {
            int random = Random.Range(0, count);
            int orient;
            Vector2Int other = ChooseRandomNeighboorInclude(Visited, Remaining[random], out orient);
            if (orient == -1) continue;
            Layout[Remaining[random].x, Remaining[random].y].Link(orient);
            Layout[other.x, other.y].Link(LayoutLinking.reverseOrientation(orient));
            Visited.Add(Remaining[random]);
            Remaining[random] = Remaining[count - 1];
            count--;
        }

        return true;
    }

    static readonly Vector2Int[] offset =
    {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down,
    };

    Vector2Int ChooseRandomNeighboorExclude(HashSet<Vector2Int> excluded, Vector2Int point, out int orientationIndex)
    {
        int bias = Random.Range(0, offset.Length);

        int remaning = offset.Length;
        while (remaning > 0)
        {
            Vector2Int tested = point + offset[bias];
            if (tested.x < 0 || tested.y < 0 || tested.x >= MapSize.x || tested.y >= MapSize.y)
                goto NEXT;
            if (excluded.Contains(tested))
                goto NEXT;

            orientationIndex = bias;
            return tested;

        NEXT:
            bias = (bias + 1) % offset.Length;
            remaning--;
        }
        orientationIndex = -1;
        return point;
    }


    Vector2Int ChooseRandomNeighboorInclude(HashSet<Vector2Int> included, Vector2Int point, out int orientationIndex)
    {
        int bias = Random.Range(0, offset.Length);

        int remaning = offset.Length;
        while (remaning > 0)
        {
            Vector2Int tested = point + offset[bias];
            if (tested.x < 0 || tested.y < 0 || tested.x >= MapSize.x || tested.y >= MapSize.y)
                goto NEXT;
            if (included.Contains(tested) == false)
                goto NEXT;

            orientationIndex = bias;
            return tested;

        NEXT:
            bias = (bias + 1) % offset.Length;
            remaning--;
        }
        orientationIndex = -1;
        return point;
    }
}


