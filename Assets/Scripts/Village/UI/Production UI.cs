using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionUI : MonoBehaviour
{
    [SerializeField] public Image ResouceIcon;

    [SerializeField] public TMP_Text Title;
    [SerializeField] public TMP_Text Quantity;
    [SerializeField] public TMP_Text TimeLeft;

    public void SetProductionUI(BuildingProduction prod)
    {
        ResouceIcon.sprite = prod.Icon;
        Title.text = prod.Name;
        Quantity.text = prod.Quantity.ToString();
        TimeLeft.text = prod.DayLeft.ToString();
    }
}
