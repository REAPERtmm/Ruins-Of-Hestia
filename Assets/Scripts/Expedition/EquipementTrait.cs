using System;
using System.Collections;
using UnityEngine;

public enum TraitRarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Mythic
}

[CreateAssetMenu(menuName = "Equipment/Trait")]
public class TraitDefinition : ScriptableObject
{
    public string TraitName;

    public TraitRarity Rarity;

    public StatName AffectedStat;

    public Unit Unit;
}

[Serializable]
public class TraitInstance
{
    public TraitDefinition Definition;

    public float Value;

    public bool IsLocked;
}