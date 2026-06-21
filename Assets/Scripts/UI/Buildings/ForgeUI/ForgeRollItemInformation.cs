using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ForgeRollItemInformation : MonoBehaviour
{
    [SerializeField] private RollCostUI CostUI;

    [SerializeField] private Button RollButton;

    [SerializeField] private ForgeUI forgeUI;

    [SerializeField] private Transform TraitInformationTransform;

    [SerializeField] private Transform TraitCostInformationTransform;

    private List<GemsCost> Costs = new();
    private EquipmentInstance Equipement;

    public void Show(EquipmentInstance equipement)
    {
        gameObject.SetActive(true);


        CostUI.SetEquipementInstance(equipement);
        CostUI.SetTier(equipement.Definition.Tier);
        CostUI.SetTrait(TraitInformationTransform);

        UnsubscribeFromTraitEvents();

        Equipement = equipement;

        SubscribeToTraitEvents(); 
    }

    private void SubscribeToTraitEvents()
    {
        foreach (var trait in Equipement.Traits)
        {
            trait.OnLockChanged -= OnTraitLockChanged; 
            trait.OnLockChanged += OnTraitLockChanged;
        }
    }

    private void UnsubscribeFromTraitEvents()
    {
        if (Equipement == null)
            return;

        foreach (var trait in Equipement.Traits)
        {
            trait.OnLockChanged -= OnTraitLockChanged;
        }
    }

    private void OnTraitLockChanged(TraitInstance trait)
    {
        CostUI.SetGemCost(TraitCostInformationTransform, Costs); 
    }

    public void StartRoll()
    {
        if (Equipement == null)
            return;

        if (!Inventory.Instance.CanAfford(Costs))
            return;

        foreach (GemsCost cost in Costs)
        {
            Inventory.Instance.RemoveResource(cost.Type, cost.Qte);
        }

        UnsubscribeFromTraitEvents();

        Forge forge = forgeUI.GetForge();
        if (forge != null)
        {
            forge.StartRoll(ref Equipement);
        }

        CostUI.SetTrait(TraitInformationTransform);
        CostUI.UpdateGemsInventoryUI();

        SubscribeToTraitEvents();
    }

    public void Update()
    {
        if (Costs.Count == 0)
        {
            RollButton.interactable = true;
            return;
        } 

        if (!Inventory.Instance.CanAfford(Costs))
        {
            RollButton.interactable = false;
            return;
        }

        RollButton.interactable = true;
    } 
}
