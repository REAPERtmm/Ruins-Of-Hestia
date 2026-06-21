using System.Collections.Generic;
using UnityEngine;


public struct MeshRotation
{
    public Mesh[] Models;
    public int Rotation;
}

public class TileGroupResolver
{
    MeshTileGroup[] TileGroups;
    Dictionary<TileGroup, List<MeshRotation>> Resolver;

    public void Process(MeshTileGroup[] tileGroups)
    {
        TileGroups = tileGroups;
        Resolver = new();

        foreach (var tileGroup in TileGroups) {
            TileGroup alignement;
            alignement.TL = tileGroup.TopLeft;
            alignement.TR = tileGroup.TopRight;
            alignement.BL = tileGroup.BottomLeft;
            alignement.BR = tileGroup.BottomRight;

            for (int o = 0; o < 4; o++) { 

                if(Resolver.ContainsKey(alignement) == false)
                {
                    Resolver.Add(alignement, new List<MeshRotation>());
                }

                MeshRotation mr;
                mr.Models = tileGroup.Models;
                mr.Rotation = o;
                Resolver[alignement].Add(mr);

                alignement = alignement.Rotate();
            }

        }
    }

    public MeshRotation Get(TileGroup alignement)
    {
        return Resolver[alignement][Random.Range(0, Resolver[alignement].Count)];
    }
}