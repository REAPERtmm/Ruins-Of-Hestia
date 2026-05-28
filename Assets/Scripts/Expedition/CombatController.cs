using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [Header("Defensive")]
    [SerializeField] Equipement Helmet;
    [SerializeField] Equipement ChestPlate;
    [SerializeField] Equipement Gloves;
    [SerializeField] Equipement Pants;
    [SerializeField] Equipement Boots;

    [Header("Offensive")]
    [SerializeField] Equipement MeleeWeapon;
    [SerializeField] Equipement DistanceWeapon;

    [Header("Hostility")]
    [SerializeField] List<Transform> Targets = new List<Transform>();

    bool IsUsingMelee = true;
    float DistanceToClosest;
    Transform ClosestTarget;

    Coroutine AttackPlayed;

    public void RegisterTarget(Transform target) => Targets.Add(target);

    public void UpdateClosest()
    {
        DistanceToClosest = float.MaxValue;
        foreach(Transform t in Targets)
        {
            if (t.gameObject.activeSelf == false) continue;
            float distance = Vector3.Distance(t.position, transform.position);
            if(distance < DistanceToClosest)
            {
                DistanceToClosest = distance;
                ClosestTarget = t;
            }
        }
    }

    public bool TargetInRange() {
        float range = 0;
        if (IsUsingMelee && MeleeWeapon != null)
        {
            range = MeleeWeapon.GetStatistic(StatName.MeleeRange).FinalValue;
        }
        else if(DistanceWeapon != null)
        {
            range = DistanceWeapon.GetStatistic(StatName.MaxRange).FinalValue;
        }
        return DistanceToClosest < range;
    }

    public IEnumerator DefaultAttackAnimation()
    {
        // TODO : Launch Animation + Hit Box 
        yield return new WaitForSeconds(1);
        AttackPlayed = null;
    }

    public bool IsAttacking => AttackPlayed != null;

    // Return wether it could attack or not
    public bool AttackClosest()
    {
        if (IsAttacking) {
            return false;
        }
        AttackPlayed = StartCoroutine(DefaultAttackAnimation());
        return true;
    }

    public void FixedUpdate() { 
        UpdateClosest();
    }
}
