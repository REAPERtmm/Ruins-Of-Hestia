using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Equipment/TraitList")]
public class TraitList : ScriptableObject
{
    public List<TraitGenerationRule> Rules;
}