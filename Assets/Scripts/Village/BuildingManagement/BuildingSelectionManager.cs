using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSelectionManager : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;
    private Building SelectedBuilding;

    public void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {

                Building building = hit.collider.GetComponentInParent<Building>();

                if (building != null)
                {
                    SelectedBuilding = building;
                    building.OnClick();
                }
                else
                {
                    if (SelectedBuilding != null)
                    {
                        SelectedBuilding.OnUnselected();
                        SelectedBuilding = null;
                    }
                }
            }
        }
    }
}