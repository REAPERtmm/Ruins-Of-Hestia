using System.Collections.Generic;
using UnityEngine;

public class CraftListUI : MonoBehaviour
{
    [Header("Recipes")]
    [SerializeField] private List<ForgeRecipe> Recipes = new();

    [Header("UI")]
    [SerializeField] private Transform ContentParent;

    [SerializeField] private CraftRecipeUI RecipePrefab;

    [SerializeField] private ForgeCraftItemInformation InformationPanel;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in ContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (ForgeRecipe recipe in Recipes)
        {
            CraftRecipeUI ui = Instantiate(RecipePrefab, ContentParent);

            ui.Setup(recipe, InformationPanel);
        }
    }
}