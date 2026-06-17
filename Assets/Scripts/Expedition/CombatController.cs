using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EntityGroup
{
    Ally,
    Ennemi
}

public class CombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject MeleeAttack;

    [Header("Defensive")]
    [SerializeField] EquipmentInstance Helmet;
    [SerializeField] EquipmentInstance ChestPlate;
    [SerializeField] EquipmentInstance Gloves;
    [SerializeField] EquipmentInstance Legs;
    [SerializeField] EquipmentInstance Boots;

    [Header("Offensive")]
    [SerializeField] EquipmentInstance MeleeWeapon;
    [SerializeField] EquipmentInstance DistanceWeapon;

    [Header("Hostility")]
    [SerializeField] List<Transform> Targets = new List<Transform>();

    [Header("Parameters")]
    [SerializeField] EntityGroup MyGroup = EntityGroup.Ally;
    [SerializeField] float BaseMaxHP = 100;
    [SerializeField] float BaseArmor = 0;
    [SerializeField] float BaseStrength = 1;
    [SerializeField] float BaseSpeed = 1;
    [SerializeField] float BaseLuck = 0;
    [SerializeField] float BaseProvocation = 0;

    float CurrentHP;

    bool IsUsingMelee = true;
    float DistanceToClosest;
    Transform ClosestTarget;

    Coroutine AttackPlayed;

    public bool ALIVE => CurrentHP > 0;
    public EntityGroup GROUP => MyGroup;
    public float MAX_HP
    {
        get
        {
            float value = BaseMaxHP;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.HealthPoint);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.HealthPoint);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.HealthPoint);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.HealthPoint);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.HealthPoint);

            return value;
        }
    }
    public float ARMOR
    {
        get
        {
            float value = BaseArmor;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.Armor);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.Armor);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.Armor);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.Armor);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.Armor);

            return value;
        }
    }
    public float STRENGTH
    {
        get
        {
            float value = BaseStrength;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.Strength);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.Strength);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.Strength);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.Strength);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.Strength);

            return value;
        }
    }
    public float SPEED
    {
        get
        {
            float value = BaseSpeed;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.Speed);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.Speed);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.Speed);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.Speed);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.Speed);

            return value;
        }
    }
    public float LUCK
    {
        get
        {
            float value = BaseLuck;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.Luck);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.Luck);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.Luck);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.Luck);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.Luck);

            return value;
        }
    }
    public float PROVOCATION
    {
        get
        {
            float value = BaseProvocation;

            if (Helmet != null)
                value += EquipmentStatCalculator.GetStat(Helmet, StatName.Provocation);

            if (ChestPlate != null)
                value += EquipmentStatCalculator.GetStat(ChestPlate, StatName.Provocation);

            if (Gloves != null)
                value += EquipmentStatCalculator.GetStat(Gloves, StatName.Provocation);

            if (Legs != null)
                value += EquipmentStatCalculator.GetStat(Legs, StatName.Provocation);

            if (Boots != null)
                value += EquipmentStatCalculator.GetStat(Boots, StatName.Provocation);

            return value;
        }
    }

    public float MELEE_DAMAGE
    {
        get
        {
            if (MeleeWeapon != null) return EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.Damage);
            return 0;
        }
    }

    public float MELEE_CRITICAL_CHANCE
    {
        get
        {

            if (MeleeWeapon != null) return EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.CriticalChance);
            return 0;
        }
    }

    public float MELEE_CRITICAL_MULTIPLIER
    {
        get
        {
            if (MeleeWeapon != null) return EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.CriticalMultiplier);
            return 0;
        }
    }

    public float MELEE_ATTACK_SPEED
    {
        get
        {
            if (MeleeWeapon != null) return EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.AttackSpeed);
            return 0;
        }
    }

    public float MELEE_RANGE
    {
        get
        {
            if (MeleeWeapon != null)
            {
                return EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.MeleeRange) * 2.0f;
            }
            return 0;
        }
    }

    public float DISTANCE_DAMAGE
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.Damage);
            return 0;
        }
    }

    public float DISTANCE_CRITICAL_CHANCE
    {
        get
        {

            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.CriticalChance);
            return 0;
        }
    }

    public float DISTANCE_CRITICAL_MULTIPLIER
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.CriticalMultiplier);
            return 0;
        }
    }

    public float DISTANCE_ATTACK_SPEED
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.AttackSpeed);
            return 0;
        }
    }

    public float DISTANCE_PENETRATION
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.Penetration);
            return 0;
        }
    }

    public float DISTANCE_PROJECTILE_SIZE
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.ProjectileSize);
            return 0;
        }
    }

    public float DISTANCE_PROJECTILE_SPEED
    {
        get
        {
            if (DistanceWeapon != null) return EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.ProjectileSpeed);
            return 0;
        }
    }

    public void RegisterTarget(Transform target) => Targets.Add(target);

    private void Start()
    {
        CurrentHP = MAX_HP;
    }

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
            range = EquipmentStatCalculator.GetStat(MeleeWeapon, StatName.MeleeRange);
        }
        else if(DistanceWeapon != null)
        {
            range = EquipmentStatCalculator.GetStat(DistanceWeapon, StatName.MaxRange);
        }
        return DistanceToClosest < range;
    }

    public IEnumerator DefaultMeleeAttackAnimation(Vector3 direction, float duration, float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);

        GameObject instance = Instantiate(MeleeAttack);
        if (instance == null) {
            Debug.LogWarning("Failed to launch Attack");
            yield return null;
        }
        
        HitInstanceDescriptor desc = new HitInstanceDescriptor();
        desc.SourceController = this;

        HitBoxAttack hitBoxAttack = instance.GetComponent<HitBoxAttack>();
        hitBoxAttack.Init(in desc, 10000, MELEE_RANGE, direction, Vector3.forward * MELEE_RANGE * 0.5f);
        hitBoxAttack.transform.position = transform.position;

        yield return new WaitForSeconds(duration);
        if (instance.IsDestroyed() == false) Destroy(instance);
        AttackPlayed = null;
    }

    public bool IsAttacking => AttackPlayed != null;

    // Return wether it could attack or not
    public bool AttackClosest()
    {
        if (IsAttacking) {
            return false;
        }
        UpdateClosest();
        if (ClosestTarget == null)
            return false;

        Vector3 direction = ClosestTarget.position - transform.position;
        AttackPlayed = StartCoroutine(DefaultMeleeAttackAnimation(direction.normalized, 0.1f, 0.5f));
        return true;
    }

    public bool AttackTarget(Transform target)
    {
        if (IsAttacking)
        {
            return false;
        }

        Vector3 direction = target.position - transform.position;
        AttackPlayed = StartCoroutine(DefaultMeleeAttackAnimation(direction.normalized, 0.1f, 0.5f));
        return true;
    }

    public void TakeDamage(CombatController other, bool is_melee)
    {

        if(is_melee)
        {
            CurrentHP -= other.MELEE_DAMAGE;
            Debug.Log(name + " took " + other.MELEE_DAMAGE + " dmg. Now has " + CurrentHP + " hp");
        }
        else
        {
            CurrentHP -= other.DISTANCE_DAMAGE;
            Debug.Log(name + " took " + other.DISTANCE_DAMAGE + " dmg. Now has " + CurrentHP + " hp");
        }
        if (CurrentHP < 0) {
            Die();
        }

    }

    public void Die()
    {
        if (GROUP == EntityGroup.Ennemi)
        {
            EnnemiController ennemi = GetComponent<EnnemiController>();
            MapGeneration.INSTANCE.ennemi_manager.KillEnnemi(ennemi);
        }
    }
}
