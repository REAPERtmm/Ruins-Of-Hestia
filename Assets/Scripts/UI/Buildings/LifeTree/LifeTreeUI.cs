using UnityEngine;

public class LifeTreeUI : BuildingsUI
{
    [SerializeField] private GameObject UiTree;

    public override void Open(BuildingScript building)
    {
        UiTree.SetActive(true);
    }

    public override void Close()
    {
        UiTree.SetActive(false);
    }
}
