using System;
using System.Linq;

public static class EquipmentStatCalculator
{
    public static float GetStat(EquipmentInstance equipment, StatName stat)
    {
        if (equipment == null)
            return 0.0f;

        if (equipment.Definition == null)
            return 0.0f;

        if (equipment.Definition.BaseStats == null)
            return 0.0f;

        if (equipment.Definition.BaseStats.Length == 0)
            return 0.0f;

        Statistic baseStat = Array.Find(equipment.Definition.BaseStats, x => x.Stat == stat);

        if (baseStat == null)
            return 0.0f;

        float baseValue = baseStat.Value;

        float additive = 0f;
        float multiplier = 1f;

        foreach (TraitInstance trait in equipment.Traits)
        {
            if (trait.Stat != stat)
                continue;

            switch (trait.Type)
            {
                case TraitType.Flat:
                    additive += trait.Value;
                    break;

                case TraitType.Percent:
                    multiplier *= trait.Value;
                    break;
            }
        }

        return (baseValue * multiplier) + additive;
    }
}