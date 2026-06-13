using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ForgeRollItemInformation : MonoBehaviour
{
    [SerializeField] private RollCostUI CostUI;

    [SerializeField] private Button RollButton;

    [SerializeField] private ForgeUI forgeUI;

    [SerializeField] private Transform TraitInformationTransform;

    private List<GemsCost> Costs = new();
    private EquipmentInstance Equipement;

    public void Show(EquipmentInstance equipement)
    {
        gameObject.SetActive(true);


        CostUI.SetEquipementInstance(equipement);
        CostUI.SetTier(equipement.Definition.Tier);
        CostUI.SetTrait(TraitInformationTransform);

        Equipement = equipement;
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

        Forge forge = forgeUI.GetForge();
        if (forge != null)
        {
            forge.StartRoll(ref Equipement);
        }

        CostUI.SetTrait(TraitInformationTransform);
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
