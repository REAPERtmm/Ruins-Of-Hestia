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
