using System.Collections.Generic;
using UnityEngine;

public static class EquipmentForge
{
    public static void RerollTraits(EquipmentInstance equipment, TraitList traitList)
    {
        for (int i = 0; i < equipment.Traits.Count; i++)
        {
            TraitInstance trait = equipment.Traits[i];

            if (trait.IsLocked)
                continue;

            equipment.Traits[i] = EquipmentGenerator.GenerateTrait(equipment.Definition.Tier, traitList);
        }
    }
}