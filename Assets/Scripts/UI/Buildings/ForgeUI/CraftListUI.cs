using System.Collections.Generic;
using UnityEngine;

public class CraftListUI : MonoBehaviour
{
    [Header("Recipes")]
    [SerializeField] private List<ForgeRecipe> Recipes = new();

    [Header("UI")]
    [SerializeField] private Transform ContentParent;

    [SerializeField] private CraftRecipeUI RecipePrefab; 
    [SerializeField] private ForgeUI Ui;

    public void Refresh()
    {
        foreach (Transform child in ContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ForgeRecipe recipe in Recipes)
        {
            CraftRecipeUI ui = Instantiate(RecipePrefab, ContentParent);

            ui.Setup(recipe, Ui);
        }
    }
}