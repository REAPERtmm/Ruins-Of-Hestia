using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class HitBoxCallBack : MonoBehaviour
{
    [SerializeField] HitBoxAttack hitBoxAttack;

    HashSet<Transform> Hitted;

    private void Start()
    {
        Hitted = new HashSet<Transform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CombatController otherCombat = other.GetComponent<CombatController>();
        if (otherCombat == null)
        {
            return;
        }


        if (Hitted.Contains(other.transform) == false && otherCombat.GROUP != hitBoxAttack.GetCaster().GROUP)
        {
            hitBoxAttack.ProccessCollision(otherCombat);

            Hitted.Add(other.transform);

            hitBoxAttack.DecrementPenetration();
            if (hitBoxAttack.HasPenetrationRunOut())
            {
                Destroy(hitBoxAttack.gameObject);
            }
        }
    }

}
