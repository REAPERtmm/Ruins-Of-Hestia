using System.Collections.Generic;
using UnityEngine;

public class RollListUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform ContentParent;

    [SerializeField] private RollItem RollPrefab;

    [SerializeField] private ForgeRollItemInformation InformationPanel;

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
            RollItem ui = Instantiate(RollPrefab, ContentParent);

            ui.Setup(equipement, InformationPanel);
        }
    }
}