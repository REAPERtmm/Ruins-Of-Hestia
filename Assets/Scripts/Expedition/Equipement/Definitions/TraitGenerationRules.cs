using System; 

[Serializable]
public class TraitGenerationRule
{
    public StatName Stat;

    public bool AllowFlat;

    public bool AllowPercent;

    public float FlatMin;
    public float FlatMax;

    public float PercentMin;
    public float PercentMax;
}