using System;
using System.Collections.Generic;
using UnityEngine;

public class LifeTree : BuildingScript
{
    public override void PassDay()
    {
        base.PassDay();
        Inventory.Instance.AddResource(ResourceType.Food, 10);
    }

    public override BuildingProduction[] GetCurrentProduction() {
        return Array.Empty<BuildingProduction>();
    }
} 
