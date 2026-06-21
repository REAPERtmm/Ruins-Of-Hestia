using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    public Material Active;
    public Material Default;
}

[Serializable]
public class GlobalBuildingData
{
    public int BuildingDuration;

    public int BaseBuildingCount;
    public int[] PerLevelBuildCountAdd;

    public List<ResourcesCost> PerLevelUpgradeCost = new List<ResourcesCost>();
}

[Serializable]
[CreateAssetMenu(fileName = "Building", menuName = "Building", order = 1)]
public class Building : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public BuildingFondation Fondation;
    public BuildingVisual Visual;
    public GlobalBuildingData GlobalBuildingData;
}