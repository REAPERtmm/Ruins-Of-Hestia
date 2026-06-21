using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/Equipment")]
public class EquipmentDefinition : ScriptableObject
{
    public string EquipmentName;

    public Sprite Icon;

    public EquipmentTier Tier;

    public EquipmentSlot Slot;

    public Statistic[] BaseStats;

    public SpecialEffectType InnateMythicEffect;
}