using System;
using System.Collections.Generic;

[Serializable]
public class EquipmentInstance
{
    public EquipmentDefinition Definition;

    public int UpgradeLevel;

    public List<TraitInstance> Traits = new();
}