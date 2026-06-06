using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RecipeName;
    [SerializeField] private Image RecipeIcon;

    private ForgeRecipe Recipe;

    public void Setup(ForgeRecipe recipe)
    {
        Recipe = recipe;

        RecipeName.text = recipe.name;

        if (recipe.Result != null)
            RecipeIcon.sprite = recipe.Result.Icon;
    }

    public ForgeRecipe GetRecipe()
    {
        return Recipe;
    }
}