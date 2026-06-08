using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityGroup
{
    Ally,
    Ennemi
}

public class CombatController : MonoBehaviour
{
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
