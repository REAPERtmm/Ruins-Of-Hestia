using System; 

[Serializable]
public class TraitGenerationRule
{
    public StatName Stat;

    public bool AllowAdditive;

    public bool AllowMultiplicative;

    public float AdditiveMin;
    public float AdditiveMax;

    public float MultiplicativeMin;
    public float MultiplicativeMax;
}