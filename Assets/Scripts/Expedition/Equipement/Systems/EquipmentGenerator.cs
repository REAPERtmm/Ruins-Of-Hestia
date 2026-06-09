using System.Collections.Generic;
using UnityEngine;

public static class EquipmentGenerator
{
    public static EquipmentInstance Generate(EquipmentDefinition definition, TraitList traitList)
    {
        EquipmentInstance equipment = new()
        {
            Definition = definition
        };

        int traitCount = GetTraitCount(definition.Tier);

        bool hasMythicTrait = false;

        for (int i = 0; i < traitCount; i++)
        {
            TraitInstance trait;

            do
            {
                trait = GenerateTrait(definition.Tier, traitList);
            }
            while (hasMythicTrait && trait.Rarity == TraitRarity.Mythic);

            if (trait.Rarity == TraitRarity.Mythic)
                hasMythicTrait = true;

            equipment.Traits.Add(trait);
        }

        return equipment;
    }

    public static void RollTrait(ref EquipmentInstance equipment, TraitList traitList)
    {
        for (int i = 0; i < equipment.Traits.Count; i++)
        {
            TraitInstance trait = equipment.Traits[i];
            if (!trait.IsLocked)
            {
                trait = GenerateTrait(equipment.Definition.Tier, traitList);
            }
            equipment.Traits[i] = trait;
        }
    }

    public static int GetTraitCount(EquipmentTier tier)
    {
        return tier switch
        {
            EquipmentTier.Common => 1,
            EquipmentTier.Rare => 2,
            EquipmentTier.Epic => 3,
            EquipmentTier.Legendary => 4,
            EquipmentTier.Mythic => 4,
            _ => 0
        };
    }

    public static float GetRarityMultiplier(TraitRarity rarity)
    {
        return rarity switch
        {
            TraitRarity.Common => 1f,
            TraitRarity.Rare => 2f,
            TraitRarity.Epic => 4f,
            TraitRarity.Legendary => 8f,
            TraitRarity.Mythic => 16f,
            _ => 1f
        };
    }

    public static TraitRarity RollTraitRarity(EquipmentTier equipmentTier)
    {
        float roll = Random.value;

        switch (equipmentTier)
        {
            case EquipmentTier.Common: 
                return TraitRarity.Common;

            case EquipmentTier.Rare: 
                return roll < 0.80f ? TraitRarity.Common : TraitRarity.Rare;

            case EquipmentTier.Epic: 
                if (roll < 0.60f)
                    return TraitRarity.Common;

                if (roll < 0.90f)
                    return TraitRarity.Rare;

                return TraitRarity.Epic;

            case EquipmentTier.Legendary: 
                if (roll < 0.45f)
                    return TraitRarity.Common;

                if (roll < 0.75f)
                    return TraitRarity.Rare;

                if (roll < 0.95f)
                    return TraitRarity.Epic;

                return TraitRarity.Legendary;

            case EquipmentTier.Mythic: 
                if (roll < 0.35f)
                    return TraitRarity.Common;

                if (roll < 0.65f)
                    return TraitRarity.Rare;

                if (roll < 0.85f)
                    return TraitRarity.Epic;

                if (roll < 0.97f)
                    return TraitRarity.Legendary;

                return TraitRarity.Mythic;
        }

        return TraitRarity.Common;
    }

    public static TraitInstance GenerateTrait(EquipmentTier equipmentTier, TraitList traits)
    {
        TraitGenerationRule rule = traits.Rules[Random.Range(0, traits.Rules.Count)];

        TraitRarity rarity = RollTraitRarity(equipmentTier);

        TraitType type;

        if (rule.AllowAdditive && rule.AllowMultiplicative)
        {
            type = Random.value < 0.5f ? TraitType.Additive : TraitType.Multiplicative;
        }
        else if (rule.AllowAdditive)
        {
            type = TraitType.Additive;
        }
        else
        {
            type = TraitType.Multiplicative;
        }

        float multiplier = GetRarityMultiplier(rarity);

        float value;

        if (type == TraitType.Additive)
        {
            value = Random.Range(rule.AdditiveMin * multiplier, rule.AdditiveMax * multiplier);
        }
        else
        {
            value = Random.Range(rule.MultiplicativeMin, rule.MultiplicativeMax);
        }

        return new TraitInstance()
        {
            Stat = rule.Stat,
            Type = type,
            Rarity = rarity,
            Value = value
        };
    }
}