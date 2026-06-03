using UnityEngine;

public class ForgeUI : BuildingsUI
{
    private Forge CurrentForge;
    [SerializeField] private GameObject UiForge;

    public override void Open(Building building)
    {
        CurrentForge = (Forge)building;

        UiForge.SetActive(true);

        Refresh();
    }

    public override void Close()
    {
        UiForge.SetActive(false);
    }

    private void Refresh()
    {
        // show craft
    }
}