using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ForgeCraftItemInformation : MonoBehaviour
{
    [SerializeField] private ItemsCostUI CostUI;

    [SerializeField] private Button CraftButton;

    public ForgeUI forgeUI;

    private List<ResourcesCost> Costs = new();
    private ForgeRecipe CurrentRecipe;

    public void Show(ForgeRecipe recipe)
    {
        gameObject.SetActive(true);

        Costs.Clear();
        CostUI.Clear();


        CostUI.SetTier(recipe.Result.Tier);

        CostUI.SetTime(recipe.CraftingTime);

        foreach (ResourcesCost cost in recipe.Costs)
        {
            CostUI.SetCost(cost.Type, cost.Qte);
            Costs.Add(cost);
        }

        CurrentRecipe = recipe;
    }

    public void StartCraft()
    {
        if (CurrentRecipe == null)
            return;

        if (!Inventory.Instance.CanAfford(Costs))
            return;

        foreach (ResourcesCost cost in Costs)
        {
            Inventory.Instance.RemoveResource(cost.Type, cost.Qte);
        }

        Forge forge = forgeUI.GetForge();
        if (forge != null)
        {
            forge.StartCraft(CurrentRecipe);
        }
    }

    public void Update()
    {
        if (Costs.Count == 0)
        {
            CraftButton.interactable = true;
            return;
        }

        if (!Inventory.Instance.CanAfford(Costs))
        {
            CraftButton.interactable = false;
            return;
        }

        CraftButton.interactable = true;
    }
}
