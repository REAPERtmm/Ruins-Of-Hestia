using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class Inventory : MonoBehaviour
{
    [Serializable]
    public class ResourceStack
    {
        public ResourceType Type;
        public int Amount;
    }


    [SerializeField] private List<EquipmentInstance> equipments = new();

    [SerializeField] private List<ResourceStack> resources = new();

    public IReadOnlyList<EquipmentInstance> Equipments => equipments;

    public static Inventory Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            foreach (var e in resources)
            {
                e.Amount = 9999;
            }
        }
        
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            foreach (var e in resources)
            {
                e.Amount = 0;
            }
        }
    }

    public void AddEquipment(EquipmentInstance equipment)
    {
        equipments.Add(equipment);
    }

    public void RemoveEquipment(EquipmentInstance equipment)
    {
        equipments.Remove(equipment);
    }

    public int GetResource(ResourceType type)
    {
        ResourceStack stack = resources.Find(x => x.Type == type);

        return stack == null ? 0 : stack.Amount;
    }

    public void AddResource(ResourceType type, int amount)
    {
        ResourceStack stack = resources.Find(x => x.Type == type);

        if (stack == null)
        {
            stack = new ResourceStack()
            {
                Type = type,
                Amount = 0
            };

            resources.Add(stack);
        }

        stack.Amount += amount;
    }

    public bool RemoveResource(ResourceType type, int amount)
    {
        ResourceStack stack = resources.Find(x => x.Type == type);

        if (stack == null)
            return false;

        if (stack.Amount < amount)
            return false;

        stack.Amount -= amount;

        return true;
    }

    public bool CanAfford(List<ResourcesCost> costs)
    {
        foreach (ResourcesCost cost in costs)
        {
            if (GetResource(cost.Type) < cost.Qte)
                return false;
        }

        return true;
    }

    public void Pay(List<ResourcesCost> costs)
    {
        foreach (ResourcesCost cost in costs)
        {
            RemoveResource(cost.Type, cost.Qte);
        }
    }
}