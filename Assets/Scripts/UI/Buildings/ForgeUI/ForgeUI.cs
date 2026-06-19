using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForgeUI : BuildingsUI
{
    [SerializeField] private Forge CurrentForge;
    [SerializeField] private GameObject UiForge;
    [SerializeField] private CraftListUI CraftList;

    public override void Open(BuildingScript building)
    {
        CurrentForge = (Forge)building;

        CraftList.Refresh();

        UiForge.SetActive(true);
    }

    public override void Close()
    {
        UiForge.SetActive(false);
    }

    public Forge GetForge()
    {
        return CurrentForge;
    }
}