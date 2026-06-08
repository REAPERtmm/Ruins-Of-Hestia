using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct ResourcesCost
{
    public ResourceType Type;
    public int Qte;
}

[CreateAssetMenu(menuName = "Forge/ForgeRecipe")]
public class ForgeRecipe : ScriptableObject
{
    public EquipmentDefinition Result;

    public int CraftingTime;

    public ResourcesCost[] Costs;
}
