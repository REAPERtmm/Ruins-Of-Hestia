using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BuildingFondation
{
    public int Width;
    public int Height;
}

[Serializable]
public class BuildingVisual
{
    public Sprite Preview;
}

[Serializable]
[CreateAssetMenu(fileName = "Building", menuName = "Building", order = 1)]
public class Building : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public BuildingFondation Fondation;
    public BuildingVisual Visual;
}