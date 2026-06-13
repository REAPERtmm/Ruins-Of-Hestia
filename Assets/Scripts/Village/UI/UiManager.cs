using UnityEngine;

public class UiManager : MonoBehaviour
{
    public GameObject PlacementModeGroup;
    public GameObject EditModeGroup;
    public GameObject ViewModeGroup;

    public void Activate( VillageMode mode )
    {
        PlacementModeGroup.SetActive(mode == VillageMode.Placement);
        EditModeGroup.SetActive(mode == VillageMode.Edit);
        ViewModeGroup.SetActive(mode == VillageMode.View);
    }

}
