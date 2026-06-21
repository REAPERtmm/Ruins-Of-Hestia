using UnityEngine;
using UnityEngine.Serialization;

public enum ProducingBuilding
{
    Forge,
    Farm
}

public class BuildingProduction
{

    public ProducingBuilding Building;
    public int      DayLeft;
    public string   Name;
    public int      Quantity;
    public Sprite   Icon;
}

public abstract class BuildingScript : MonoBehaviour
{
    [SerializeField] public GameObject BuildingUIPrefab;

    public BuildingData Data;

    [SerializeField] private BuildingView BuildingView;

    [SerializeField] private BuildingsUI BuildingUI;

    [Header("Build State")]
    [SerializeField] private GameObject JustBuild;
    [SerializeField] private GameObject Build;
    [SerializeField] private GameObject InBuild;

    [SerializeField] private AudioClip _upgradeSound;
    [SerializeField] private AudioClip _onSelect;
    [SerializeField] private AudioClip _onPlace;

    public BuildingsUI UI => BuildingUI;

    public void Instantiate( Transform parent )
    {
        if (BuildingUIPrefab)
        {
            GameObject obj = Instantiate(BuildingUIPrefab, parent);
            BuildingUI = obj.GetComponent<BuildingsUI>();
        }

        SFXPlayer.PlaySFX(_onPlace);

        InBuild.SetActive(true);
        Build.SetActive(false);
        JustBuild.SetActive(false);

        if (Data.DayLeftToBuild <= 0)
            MakeBuildingVisible();
    }

    public virtual bool OnClick()
    {
        if (Data.DayLeftToBuild > 0)
            return false;

        SFXPlayer.PlaySFX(_onSelect);

        BuildingView.SetSelected( Data.Descriptor.Visual, true );
        BuildingUI?.Open(this);
        return true;
    }

    public virtual void OnUnselected()
    {
        BuildingView.SetSelected( Data.Descriptor.Visual, false );
        BuildingUI?.Close();
    }

    public virtual void PassDay()
    {
        if (Data.DayLeftToBuild <= 0)
            return;

        Data.DayLeftToBuild--;
        if (Data.DayLeftToBuild <= 0)
        {
            MakeBuildingVisible();
        }
    }

    private void MakeBuildingVisible()
    {
        BuildingView.View.gameObject.SetActive(true);

        JustBuild.SetActive(true);
        Build.SetActive(true);
        InBuild.SetActive(false);

        SFXPlayer.PlaySFX(_upgradeSound);
    }

    public abstract BuildingProduction[] GetCurrentProduction();
}