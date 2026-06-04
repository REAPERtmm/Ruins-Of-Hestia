using UnityEngine;

public abstract class BuildingScript : MonoBehaviour
{
    [SerializeField] private BuildingsUI BuildingUI; 

    public BuildingsUI UI => BuildingUI;

    public virtual void OnClick()
    {
        BuildingUI.Open(this);
    }

    public virtual void OnUnselected()
    {
        BuildingUI.Close();
    }
}