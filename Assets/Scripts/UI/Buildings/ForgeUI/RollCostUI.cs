using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class RollCostUI : MonoBehaviour
{
    [SerializeField] private TMP_Text Tier;
    [SerializeField] private TraitInformation TraitPrefab; 
    [SerializeField] private List<GemImage> Image; 

    private EquipmentInstance EquipmentInstance;


    [Serializable]
    private struct GemImage
    {
        public GemType Type;
        public Sprite Sprite;
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
                costs[index].Qte += trait.Cost.Qte;
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

        int basePurpleCost = 5;
        int numLockedTraits = EquipmentInstance.Traits.Count(t => t.IsLocked) + 1;

        costs.Add(new GemsCost
        {
            Type = GemType.Purple,
            Qte = numLockedTraits * basePurpleCost,
        });

        foreach (var cost in costs)
        {
            GameObject go = new GameObject("GemImage");
            go.transform.SetParent(parent, false);
            go.transform.localScale = Vector3.one * 2f;

            Image image = go.AddComponent<Image>();
            image.sprite = GetGemSprite(cost.Type);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(128, 128);

            GameObject goText = new GameObject("GemText");
            goText.transform.SetParent(go.transform, false);

            TextMeshProUGUI text = goText.AddComponent<TextMeshProUGUI>();
            text.text = cost.Qte.ToString();
            text.fontSize = 18;
            text.color = Color.black;

            RectTransform rectText = goText.GetComponent<RectTransform>();
            rectText.anchoredPosition = new Vector2(100, -25);
        }
    }

    private Sprite GetGemSprite(GemType type)
    {
        return type switch
        {
            GemType.Red => Image.Find(g => g.Type == GemType.Red).Sprite,
            GemType.Blue => Image.Find(g => g.Type == GemType.Blue).Sprite,
            GemType.Green => Image.Find(g => g.Type == GemType.Green).Sprite,
            GemType.Orange => Image.Find(g => g.Type == GemType.Orange).Sprite,
            GemType.Yellow => Image.Find(g => g.Type == GemType.Yellow).Sprite,
            GemType.Purple => Image.Find(g => g.Type == GemType.Purple).Sprite,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public void Clear()
    {
        EquipmentInstance = null;
        Tier.text = "I";
    }
} 