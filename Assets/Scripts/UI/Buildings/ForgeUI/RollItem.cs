using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollItem : MonoBehaviour
{
    [SerializeField] private TMP_Text ItemName;
    [SerializeField] private Image ItemIcon;

    private EquipmentInstance Equipement;

    private ForgeRollItemInformation InfoPanel;


    public void Setup(EquipmentInstance equipement, ForgeRollItemInformation infoPanel)
    {
        Equipement = equipement;
        InfoPanel = infoPanel;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        ItemName.text = Equipement.Definition.EquipmentName;

        if (Equipement.Definition != null)
            ItemIcon.sprite = Equipement.Definition.Icon;
    }

    public void OnClick()
    {
        InfoPanel.Show(Equipement);
    }
}