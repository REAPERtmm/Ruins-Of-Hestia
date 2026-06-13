using UnityEngine;
using UnityEngine.Serialization;

public abstract class BuildingScript : MonoBehaviour
{
    [SerializeField]
    public GameObject BuildingUIPrefab;

    [SerializeField]
    private BuildingsUI BuildingUI;

    public BuildingsUI UI => BuildingUI;

    public void Instantiate( Transform parent )
    {
        GameObject obj = Instantiate(BuildingUIPrefab, parent);
        BuildingUI = obj.GetComponent<BuildingsUI>();
    }

    public virtual void OnClick()
    {
        BuildingUI.Open(this);
    }

    public virtual void OnUnselected()
    {
        BuildingUI.Close();
    }
}