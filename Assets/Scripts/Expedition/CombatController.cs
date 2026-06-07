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
    [SerializeField] Equipement Helmet;
    [SerializeField] Equipement ChestPlate;
    [SerializeField] Equipement Gloves;
    [SerializeField] Equipement Legs;
    [SerializeField] Equipement Boots;

    [Header("Offensive")]
    [SerializeField] Equipement MeleeWeapon;
    [SerializeField] Equipement DistanceWeapon;

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
            if (Helmet != null) value += Helmet         .GetStatistic(StatName.HealthPoint).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.HealthPoint).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.HealthPoint).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.HealthPoint).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.HealthPoint).FinalValue;
            return value;
        }
    }
    public float ARMOR
    {
        get
        {
            float value = BaseArmor;
            if (Helmet != null)  value += Helmet        .GetStatistic(StatName.Armor).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.Armor).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.Armor).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.Armor).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.Armor).FinalValue;
            return value;
        }
    }
    public float STRENGTH
    {
        get
        {
            float value = BaseStrength;
            if (Helmet != null) value += Helmet         .GetStatistic(StatName.Strength).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.Strength).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.Strength).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.Strength).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.Strength).FinalValue;
            return value;
        }
    }
    public float SPEED
    {
        get
        {
            float value = BaseSpeed;
            if (Helmet != null) value += Helmet         .GetStatistic(StatName.Speed).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.Speed).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.Speed).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.Speed).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.Speed).FinalValue;
            return value;
        }
    }
    public float LUCK
    {
        get
        {
            float value = BaseLuck;
            if (Helmet != null) value += Helmet         .GetStatistic(StatName.Luck).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.Luck).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.Luck).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.Luck).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.Luck).FinalValue;
            return value;
        }
    }
    public float PROVOCATION
    {
        get
        {
            float value = BaseLuck;
            if (Helmet != null) value += Helmet         .GetStatistic(StatName.Provocation).FinalValue;
            if (ChestPlate != null) value += ChestPlate .GetStatistic(StatName.Provocation).FinalValue;
            if (Gloves != null) value += Helmet         .GetStatistic(StatName.Provocation).FinalValue;
            if (Legs != null) value += Legs             .GetStatistic(StatName.Provocation).FinalValue;
            if (Boots != null) value += Boots           .GetStatistic(StatName.Provocation).FinalValue;
            return value;
        }
    }

    public float MELEE_DAMAGE
    {
        get
        {
            if (MeleeWeapon != null) return MeleeWeapon.GetStatistic(StatName.Damage).FinalValue;
            return 0;
        }
    }

    public float MELEE_CRITICAL_CHANCE
    {
        get
        {
            if (MeleeWeapon != null) return MeleeWeapon.GetStatistic(StatName.CriticalChance).FinalValue;
            return 0;
        }
    }

    public float MELEE_CRITICAL_MULTIPLIER
    {
        get
        {
            if (MeleeWeapon != null) return MeleeWeapon.GetStatistic(StatName.CriticalMultiplier).FinalValue;
            return 0;
        }
    }

    public float MELEE_ATTACK_SPEED
    {
        get
        {
            if (MeleeWeapon != null) return MeleeWeapon.GetStatistic(StatName.AttackSpeed).FinalValue;
            return 0;
        }
    }

    public float MELEE_RANGE
    {
        get
        {
            if (MeleeWeapon != null)
            {
                return MeleeWeapon.GetStatistic(StatName.MeleeRange).FinalValue * 2.0f;
            }
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
            range = MeleeWeapon.GetStatistic(StatName.MeleeRange).FinalValue;
        }
        else if(DistanceWeapon != null)
        {
            range = DistanceWeapon.GetStatistic(StatName.MaxRange).FinalValue;
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
        if (IsAttacking || ClosestTarget == null) {
            return false;
        }

        Vector3 direction = ClosestTarget.position - transform.position;
        AttackPlayed = StartCoroutine(DefaultMeleeAttackAnimation(direction.normalized, 0.1f, 0.5f));
        return true;
    }

    public void FixedUpdate() { 
        UpdateClosest();

        if (MeleeAttack == null) return;
    }
}
