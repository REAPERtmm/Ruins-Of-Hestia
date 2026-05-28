using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;

[Serializable]
public enum StatName
{
    Unknown,

    HealthPoint,
    Armor,
    Strength,
    Luck,
    Speed,
    Provocation,

    Damage,
    CriticalChance,
    CriticalMultiplier,
    AttackSpeed,

    MeleeRange,

    ProjectileSpeed,
    ProjectileSize,
    Penetration,
    MaxRange,

}

[CreateAssetMenu(fileName = "Equipement", menuName = "ScriptableObject/Equipement")]
public class Equipement : ScriptableObject
{
    [SerializeField] public Statistic[] Statistics;

    public Statistic GetStatistic(StatName name)
    {
        return Statistics.First(ctx => ctx.Stat == name);
    }
}

[Serializable]
public enum GemCost
{
    Red,
    Blue, 
    Green,
    Orange,
    Yellow
}

[Serializable]
public enum Unit
{
    Value,
    Multiplier,
    Percent,
    CPS,
    M_S,
    M
}

[Serializable]
public class Statistic
{
    [SerializeField] public StatName    Stat                = StatName.Unknown;
    [SerializeField] public float BaseValue                 = 0;
    [SerializeField] public GemCost NeededGemToReroll       = 0;
    [SerializeField] public Unit StatUnit                   = 0;

    [SerializeField] public float BaseAddModifier           = 0;
    [SerializeField] public float MultiplierModifier        = 1;
    [SerializeField] public float FinalAddModifier          = 0;

    public float FinalValue { get { return (BaseValue + BaseAddModifier) * MultiplierModifier + FinalAddModifier; } } 
}
