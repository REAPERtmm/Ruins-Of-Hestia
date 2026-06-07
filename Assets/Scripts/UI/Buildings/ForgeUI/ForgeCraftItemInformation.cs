using UnityEngine;

public class ForgeCraftItemInformation : MonoBehaviour
{
    [SerializeField] private ItemsCostUI CostUI;

    public void Show(ForgeRecipe recipe)
    {
        gameObject.SetActive(true);

        CostUI.Clear();


        CostUI.SetTier(recipe.Result.Tier);

        CostUI.SetTime(recipe.CraftingTime);

        foreach (ResourcesCost cost in recipe.Costs)
        {
            CostUI.SetCost(cost.Type, cost.Qte);
        }
    }
}
