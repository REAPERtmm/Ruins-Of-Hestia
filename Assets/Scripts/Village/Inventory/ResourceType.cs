using System;

[Serializable]
public enum ResourceType
{
    Wood,
    Stone,
    Metal,
    Leaves,
    Food
}

[Serializable]
public class ResourceStack
{
    public ResourceType Type;
    public int Amount;
}

[Serializable]
public class GemStack
{
    public GemType Type;
    public int Amount;
}

[Serializable]
public struct ResourcesCost
{
    public ResourceType Type;
    public int Qte;
}

[Serializable]
public class GemsCost
{
    public GemType Type;
    public int Qte;
}