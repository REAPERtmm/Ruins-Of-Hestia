using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForgeUI : BuildingsUI
{
    [SerializeField] private Forge CurrentForge;
    [SerializeField] private GameObject UiForge;

    public override void Open(BuildingScript building)
    {
        CurrentForge = (Forge)building;

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

    public void Update()
    {
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if (CurrentForge != null)
            {
                CurrentForge.PassDay();
                Debug.Log("PassDay");
            }
        }
    }
}