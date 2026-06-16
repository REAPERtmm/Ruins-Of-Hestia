using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Farm/FarmRecipe")]
public class FarmRecipe : ScriptableObject
{
    public string RecipeName;

    public ResourceStack Result;

    public int CraftingTime;

    public Sprite Icon;
}
