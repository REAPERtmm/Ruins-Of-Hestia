using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmRecipeUI : MonoBehaviour
{
    [SerializeField] private Button     Button;
    [SerializeField] private TMP_Text   RecipeName;
    [SerializeField] private TMP_Text   Time;
    [SerializeField] private TMP_Text   Quantity;
    [SerializeField] private Image      LockIcon;

    private FarmRecipe Recipe; 
    private Farm Farm; 

    public void Setup(FarmRecipe recipe, Farm farm)
    {
        
        Recipe = recipe;
        Farm = farm;

        RecipeName.text = recipe.RecipeName;
        Time.text = recipe.CraftingTime.ToString() + " cycles";
        Quantity.text = "+ " + recipe.Result.Amount.ToString();

        if (recipe.IsUnlocked)
        {
            LockIcon.gameObject.SetActive(false);
            Button.GetComponent<Image>().sprite = recipe.Background;
        }
        else
        {
            LockIcon.gameObject.SetActive(true);
            Button.interactable = false;
        }

    }

    public void OnClick()
    { 
        Farm.StartCraft(Recipe);
    }
}
