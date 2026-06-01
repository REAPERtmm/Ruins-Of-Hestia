using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingFondation
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public BuildingFondation(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}

[Serializable]
public class BuildingImage
{
    public Texture Texture;
    public Vector3 Offset;
    public Vector3 Scale;

    public BuildingImage(Texture texture, Vector3 offset, Vector3 scale)
    {
        Texture = texture;
        Offset = offset;
        Scale = scale;
    }
}

[CreateAssetMenu(fileName="Building", menuName="Village/SpawnManagerScriptableObject", order = 1)]
public class BuildingInstance
{
    public List<BuildingInstance> Holder;
    public BuildingFondation Fondation;
    public BuildingImage Image;
    public int Function;
}
