using UnityEngine;

public class ForgeListUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform ContentParent;

    [SerializeField] private GameObject InventoryItem;

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

        foreach (EquipmentInstance equipement in Inventory.Instance.Equipments)
        {
            Instantiate(InventoryItem, ContentParent); 
        }
    }
}
