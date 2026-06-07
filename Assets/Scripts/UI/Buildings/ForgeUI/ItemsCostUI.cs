using TMPro;
using UnityEngine; 

public class ItemsCostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Tier;
    [SerializeField] private TMP_Text Time;
    [SerializeField] private TMP_Text Wood;
    [SerializeField] private TMP_Text Stone;
    [SerializeField] private TMP_Text Metal;
    [SerializeField] private TMP_Text Leaves;
    [SerializeField] private TMP_Text Food;

    public void SetTier(EquipmentTier tier)
    {
        switch (tier)
        {
            case EquipmentTier.Common:
                Tier.text = "I";
                break;
            case EquipmentTier.Rare:
                Tier.text = "II";
                break;
            case EquipmentTier.Epic:
                Tier.text = "III";
                break;
            case EquipmentTier.Legendary:
                Tier.text = "IV";
                break;
            case EquipmentTier.Mythic:
                Tier.text = "V";
                break;
        }
    }

    public void SetTime(int time)
    {
        Time.text = time.ToString();
    }

    public void SetCost(int wood, int stone, int metal, int leaves, int food)
    {
        Wood.text = wood.ToString();
        Stone.text = stone.ToString();
        Metal.text = metal.ToString();
        Leaves.text = leaves.ToString();
        Food.text = food.ToString();
    }

    public void SetCost(ResourceType type, int cost)
    {
        switch (type)
        {
            case ResourceType.Wood:
                Wood.text = cost.ToString();
                break;
            case ResourceType.Stone:
                Stone.text = cost.ToString();
                break;
            case ResourceType.Metal:
                Metal.text = cost.ToString();
                break;
            case ResourceType.Leaves:
                Leaves.text = cost.ToString();
                break;
            case ResourceType.Food:
                Food.text = cost.ToString();
                break;
        }
    }

    public void Clear()
    {
        Tier.text = "I";
        Time.text = "1";
        Wood.text = "0";
        Stone.text = "0";
        Metal.text = "0";
        Leaves.text = "0";
        Food.text = "0";
    }
}
