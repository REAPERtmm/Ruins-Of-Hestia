using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftRecipeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RecipeName;
    [SerializeField] private Image RecipeIcon;
    [SerializeField] private ForgeCraftItemInformation info;

    private ForgeRecipe Recipe; 

    public void Setup(ForgeRecipe recipe, ForgeUI Ui)
    {
        Recipe = recipe; 

        RecipeName.text = recipe.Result.EquipmentName;

        if (recipe.Result != null)
            RecipeIcon.sprite = recipe.Result.Icon;

        info.forgeUI = Ui;
        info.Show(recipe);
    } 
}