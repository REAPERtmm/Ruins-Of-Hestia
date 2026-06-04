using UnityEngine;
using System.Collections.Generic;

public class Forge : BuildingScript
{
    [SerializeField] int Level = 1;

    private readonly List<CraftJob> Jobs = new ();

    public int MaxSlot => 2 + (Level - 1);

    public bool StartCraft(ForgeRecipe recipe)
    {
        if (Jobs.Count >= MaxSlot)
            return false;

        Jobs.Add(new CraftJob() 
        {
            Recipe = recipe, RemainingCraftingTime = recipe.CraftingTime 
        });

        return true;    
    }

    public void PassDay()
    {
        for (int i = Jobs.Count - 1; i >= 0; i--)
        {
            Jobs[i].RemainingCraftingTime--;

            if (Jobs[i].RemainingCraftingTime <= 0)
            {
                // TODO : Add equipement to inventory
                Jobs.RemoveAt(i);
            }
        }
    } 
}
