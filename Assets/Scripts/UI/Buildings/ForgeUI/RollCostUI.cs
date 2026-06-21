using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollCostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Tier;
    [SerializeField] private TraitInformation TraitPrefab;
     
    [SerializeField] private List<GemImage> GemSprites;
     
    [SerializeField] private List<GemsInfo> Text;
    [SerializeField] private TMP_FontAsset Font;

    private EquipmentInstance EquipmentInstance;

    [Serializable]
    private struct GemImage
    {
        public GemType GemType;
        public Sprite Sprite;
    }

    [Serializable]
    private struct GemsInfo
    {
        public GemType GemType;
        public TMP_Text Text;
    }

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

        foreach (var trait in EquipmentInstance.Traits)
        {
            TraitInformation info = Instantiate(TraitPrefab, parent);
            info.Initialize(trait);
        }
    } 

    public void UpdateGemsInventoryUI()
    {
        foreach (var gemsInfo in Text)
        {
            if (gemsInfo.Text != null)
                gemsInfo.Text.text = Inventory.Instance.GetGemAmount(gemsInfo.GemType).ToString();
        }
    }

    public void SetGemCost(Transform parent, List<GemsCost> costs)
    {
        costs.Clear();

        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }

        foreach (var trait in EquipmentInstance.Traits)
        {
            if (!trait.IsLocked)
                continue;

            int index = costs.FindIndex(c => c.Type == trait.Cost.Type);

            if (index >= 0)
            {
                var existing = costs[index];
                existing.Qte += trait.Cost.Qte;
                costs[index] = existing;
            }
            else
            {
                costs.Add(new GemsCost
                {
                    Type = trait.Cost.Type,
                    Qte = trait.Cost.Qte
                });
            }
        }

        const int basePurpleCost = 5;
        int numLockedTraits = EquipmentInstance.Traits.Count(t => t.IsLocked) + 1;
        int purpleQte = numLockedTraits * basePurpleCost;

        int purpleIndex = costs.FindIndex(c => c.Type == GemType.Purple);
        if (purpleIndex >= 0)
        {
            var existing = costs[purpleIndex];
            existing.Qte += purpleQte;
            costs[purpleIndex] = existing;
        }
        else
        {
            costs.Add(new GemsCost { Type = GemType.Purple, Qte = purpleQte });
        }

        foreach (var cost in costs)
        {
            GameObject go = new GameObject("GemImage");
            go.transform.SetParent(parent, false);
            go.transform.localScale = Vector3.one;

            Image image = go.AddComponent<Image>();
            image.sprite = GetGemSprite(cost.Type);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(128, 128);

            GameObject goText = new GameObject("GemText");
            goText.transform.SetParent(go.transform, false);

            TextMeshProUGUI text = goText.AddComponent<TextMeshProUGUI>();
            text.text = cost.Qte.ToString();
            text.fontSize = 24;
            text.color = Color.black;
            text.font = Font;

            RectTransform rectText = goText.GetComponent<RectTransform>();
            rectText.anchoredPosition = new Vector2(145, -50);
        }
    }

    private Sprite GetGemSprite(GemType type)
    {
        GemImage gemImage = GemSprites.FirstOrDefault(g => g.GemType == type);

        return gemImage.Sprite;
    }

    public void Clear()
    {
        EquipmentInstance = null;
        Tier.text = "I";
    }
}