using UnityEngine;
using UnityEngine.Serialization;

public abstract class BuildingScript : MonoBehaviour
{
    [SerializeField]
    public GameObject BuildingUIPrefab;

    public BuildingData Data;

    [SerializeField]
    private BuildingView BuildingView;

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
        if (Data.DayLeftToBuild <= 0)
            return;

        BuildingView.SetSelected( Data.Descriptor.Visual, true );
        BuildingUI.Open(this);
    }

    public virtual void OnUnselected()
    {
        BuildingView.SetSelected( Data.Descriptor.Visual, false );
        BuildingUI.Close();
    }

    public virtual void PassDay()
    {
        Data.DayLeftToBuild--;
        if (Data.DayLeftToBuild <= 0)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}