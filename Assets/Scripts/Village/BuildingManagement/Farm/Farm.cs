using UnityEngine;
using System.Collections.Generic;

public class Farm : BuildingScript
{

    private readonly List<FarmJob> Jobs = new ();

    public bool StartCraft(FarmRecipe recipe)
    {
        if (Jobs.Count >= 1)
            return false;

        Jobs.Add(new FarmJob() 
        {
            Recipe = recipe, RemainingCraftingTime = recipe.CraftingTime 
        });

        return true;
    }

    public override void PassDay() 
    {
        base.PassDay();

        for (int i = Jobs.Count - 1; i >= 0; i--)
        {
            Jobs[i].RemainingCraftingTime--;

            if (Jobs[i].RemainingCraftingTime <= 0)
            { 
                Inventory.Instance.AddResource(Jobs[i].Recipe.Result.Type, Jobs[i].Recipe.Result.Amount);
                Jobs.RemoveAt(i);
            }
        }
    }

    public override BuildingProduction[] GetCurrentProduction()
    {
        List<BuildingProduction> productions = new();
        for (var i = 0; i < Jobs.Count; i++)
        {
            var craftJob = Jobs[i];
            productions.Add( new BuildingProduction()
            {
                Building = ProducingBuilding.Forge,
                Name = craftJob.Recipe.RecipeName,
                DayLeft = craftJob.RemainingCraftingTime,
                Icon = craftJob.Recipe.Icon
            });
        }

        return productions.ToArray();
    }
}
