using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class EquipmentInstance
{
    public Equipement Template;

    public List<TraitInstance> Traits;
}