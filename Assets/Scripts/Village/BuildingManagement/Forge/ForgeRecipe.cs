using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Forge/ForgeRecipe")]
public class ForgeRecipe : ScriptableObject
{
    public EquipmentDefinition Result;

    public int CraftingTime;

    public ResourcesCost[] Costs;
}
