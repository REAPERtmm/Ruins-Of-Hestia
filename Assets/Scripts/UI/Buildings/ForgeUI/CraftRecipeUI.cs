using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RecipeName;
    [SerializeField] private Image RecipeIcon;

    private ForgeRecipe Recipe;

    private ForgeCraftItemInformation InfoPanel;

    public void Setup(ForgeRecipe recipe, ForgeCraftItemInformation infoPanel)
    {
        Recipe = recipe;
        InfoPanel = infoPanel;

        RecipeName.text = recipe.Result.EquipmentName;

        if (recipe.Result != null)
            RecipeIcon.sprite = recipe.Result.Icon;
    }

    public void OnClick()
    {
        InfoPanel.Show(Recipe);
    }
}