using System;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    Unknown,
    Maison,
    Ferme,
    Forge,
    BuildingTypeCount
}
public class VillageGrid15x15
{
    public const int GridWidth = 15;
    public const int GridHeight = 15;
    int[,] GridOccupied;
    int X;
    int Y;

    public VillageGrid15x15(int x, int y)
    {
        GridOccupied = new int[GridWidth, GridHeight];

        X = x;
        Y = y;
        for (int i = 0; i < GridWidth; i++)
        {
            for(int j = 0; j < GridHeight; j++)
            {
                GridOccupied[i, j] = -1;
            }
        }
    } 

    public bool CheckIfRectFree(BuildingFondation fondation)
    {
        for(int x = fondation.X; x <= fondation.X + fondation.Width; x++)
        {
            for (int y = fondation.Y; y <= fondation.Y + fondation.Height; y++)
            {
                if (GridOccupied[x, y] != -1) return false;
            }
        }
        return true;
    }
}

public class VillageManager : MonoBehaviour
{
    List<BuildingInstance> Buildings;
    List<VillageGrid15x15> Grids;

    bool CheckIfRectFree(BuildingFondation fondation)
    {
        foreach (VillageGrid15x15 grid in Grids) {
            if (grid.CheckIfRectFree(fondation) == false) return false;
        }
        return true;
    }

    public bool TryRegisterBuilding(int x, int y)
    {
        BuildingFondation fondation = new BuildingFondation(x, y, 1, 1);
        if (CheckIfRectFree(fondation) == false) return false;

        BuildingInstance instance = new BuildingInstance();
        instance.Holder = Buildings;
        instance.Fondation = fondation;
        instance.Image = new BuildingImage(null, Vector3.zero, Vector3.one);

        return true;
    }

}
