using UnityEngine;

public class FarmUI : BuildingsUI
{
    [SerializeField] private GameObject UiFarm;
    [SerializeField] private Farm CurrentFarm;

    public override void Open(BuildingScript building)
    {
        CurrentFarm = (Farm)building;
        UiFarm.SetActive(true);
    }

    public override void Close()
    {
        UiFarm.SetActive(false);
    }

    public Farm GetFarm()
    {
        return CurrentFarm;
    }
}
