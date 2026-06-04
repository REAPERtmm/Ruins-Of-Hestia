using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingFondation
{
    public int Width;
    public int Height;
}

[Serializable]
[CreateAssetMenu(fileName = "Building", menuName = "Building", order = 1)]
public class Building : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public BuildingFondation Fondation;
}