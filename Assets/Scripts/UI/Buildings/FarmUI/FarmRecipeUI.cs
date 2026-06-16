using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmRecipeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text RecipeName;
    [SerializeField] private TMP_Text Time;
    [SerializeField] private TMP_Text Quantity;
    [SerializeField] private Image RecipeIcon;

    private FarmRecipe Recipe; 
    private Farm Farm; 

    public void Setup(FarmRecipe recipe, Farm farm)
    {
        Recipe = recipe;
        Farm = farm;

        RecipeName.text = recipe.RecipeName;
        Time.text = recipe.CraftingTime.ToString() + " cycles";
        Quantity.text = "+ " + recipe.Result.Amount.ToString();

        if (recipe.Result != null)
            RecipeIcon.sprite = recipe.Icon;
    }

    public void OnClick()
    {
        Debug.Log("Start Farm");
        Farm.StartCraft(Recipe);
    }
}
