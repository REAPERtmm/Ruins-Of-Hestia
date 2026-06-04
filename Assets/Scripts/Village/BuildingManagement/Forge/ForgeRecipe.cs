using Mono.Cecil;
using UnityEngine;

public struct ResourcesCost
{
    public ResourceType Type;
    public int Qte;
}

[CreateAssetMenu(menuName = "Forge/ForgeRecipe")]
public class ForgeRecipe : ScriptableObject
{
    public Equipement Result;

    public int CraftingTime;

    public ResourcesCost[] Costs;
}
