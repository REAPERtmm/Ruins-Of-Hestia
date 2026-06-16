using System.Collections.Generic;
using UnityEngine;

public class FarmListRecipe : MonoBehaviour
{
    [Header("Recipes")]
    [SerializeField] private List<FarmRecipe> Recipes = new();

    [Header("UI")]
    [SerializeField] private Transform ContentParent;

    [SerializeField] private FarmRecipeUI RecipePrefab; 
    [SerializeField] private FarmUI farmUI; 



    public void Start()
    {
        Refresh(); 
    }
    public void OnEnable()
    {
        Refresh(); 
    }

    public void Refresh()
    {
        foreach (Transform child in ContentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (FarmRecipe recipe in Recipes)
        { 
            FarmRecipeUI ui = Instantiate(RecipePrefab, ContentParent);
            ui.Setup(recipe, farmUI.GetFarm());
        }
    }
}
