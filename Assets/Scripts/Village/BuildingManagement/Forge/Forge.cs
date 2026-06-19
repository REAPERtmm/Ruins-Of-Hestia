using UnityEngine;
using System.Collections.Generic;

public class Forge : BuildingScript
{

    [SerializeField] int Level = 1;

    [SerializeField] private TraitList TraitList;

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

    public void StartRoll(ref EquipmentInstance equipment)
    {
         EquipmentGenerator.RollTrait(ref equipment, TraitList);
    }

    public override void PassDay() 
    {
        Debug.Log("PassDay");
        base.PassDay();

        for (int i = Jobs.Count - 1; i >= 0; i--)
        {
            Jobs[i].RemainingCraftingTime--;

            if (Jobs[i].RemainingCraftingTime <= 0)
            {
                EquipmentInstance equipment = EquipmentGenerator.Generate(Jobs[i].Recipe.Result, TraitList);
                Inventory.Instance.AddEquipment(equipment);
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
                Name = craftJob.Recipe.Result.EquipmentName,
                DayLeft = craftJob.RemainingCraftingTime,
                Icon = craftJob.Recipe.Result.Icon,
                Quantity = 1
            });
        }

        return productions.ToArray();
    }

}
