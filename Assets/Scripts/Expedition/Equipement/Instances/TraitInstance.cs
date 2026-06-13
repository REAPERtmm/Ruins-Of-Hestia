using System;

[Serializable] 
public class TraitInstance
{
    public TraitRarity Rarity;

    public TraitType Type;

    public StatName Stat;

    public float Value;

    public bool IsLocked;
}