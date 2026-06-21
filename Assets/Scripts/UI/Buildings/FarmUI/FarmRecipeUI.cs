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
    private FarmUI Farm; 

    public void Setup(FarmRecipe recipe, FarmUI farm)
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

    public void Update()
    {
        if (Farm.GetFarm().Jobs.Count > 0)
        {
            LockIcon.gameObject.SetActive(true);
            Button.interactable = false;
        }
        else
        {
            if (Recipe.IsUnlocked)
            {
                LockIcon.gameObject.SetActive(false);
                Button.interactable = true;
            }
        }
    }

    public void OnClick()
    { 
        Farm.GetFarm().StartCraft(Recipe);
    }
}
