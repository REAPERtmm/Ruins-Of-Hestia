using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public struct HitInstanceDescriptor
{
    public CombatController SourceController;
}

public class HitBoxAttack : MonoBehaviour
{
    [SerializeField] int Penetration = 1;
    [SerializeField] HitInstanceDescriptor HitInstance;
    [SerializeField] Collider SelfCollider;

    Vector3 LastPosition = Vector3.zero;
    bool ShouldFaceDirection;

    public void Init(in HitInstanceDescriptor desc, int penetration, float range, Vector3 direction, Vector3 offset, bool shouldFaceDirection = false)
    {
        HitInstance = desc;
        ShouldFaceDirection = shouldFaceDirection;
        // SelfCollider.excludeLayers = desc.SourceController.GROUP == EntityGroup.Ennemi ? LayerMask.GetMask("Ennemi") : LayerMask.GetMask("Ally");
        Penetration = penetration;

        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        Transform collider_transform = SelfCollider.transform;
        collider_transform.localPosition = offset;
        collider_transform.localScale = Vector3.one * range;
    }

    public void DecrementPenetration() => Penetration--;

    public bool HasPenetrationRunOut() => Penetration < 0;

    public CombatController GetCaster() => HitInstance.SourceController;

    private void FixedUpdate()
    {
        if(ShouldFaceDirection)
        {
            Vector3 direction = (transform.position - LastPosition).normalized;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        LastPosition = transform.position;
    }

    public void ProccessCollision(CombatController other)
    {
        other.TakeDamage(GetCaster(), true);
    }
}
