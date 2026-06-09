using System;
using TMPro;
using UnityEngine;

public class RollCostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Tier;
    [SerializeField] private TraitInformation TraitPrefab;

    private EquipmentInstance EquipmentInstance;

    public void SetEquipementInstance(EquipmentInstance equipmentInstance)
    {
        EquipmentInstance = equipmentInstance;
    }

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

    public void SetTrait(Transform parent)
    { 
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < EquipmentInstance.Traits.Count; i++)
        {
            TraitInformation info = Instantiate(TraitPrefab, parent);
            info.TraitName.text = EquipmentInstance.Traits[i].Stat.ToString();
            info.TraitValue.text = Math.Round(EquipmentInstance.Traits[i].Value, 2).ToString();
        } 
    }

    public void Clear()
    {
        EquipmentInstance = null;
        Tier.text = "I";
    }
}
