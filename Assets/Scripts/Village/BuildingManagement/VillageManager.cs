using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum VillageMode
{
    View,
    Edit,
    Placement
}

public class VillageManager : MonoBehaviour
{

    [Header("Buildings")]
    public List<Building> BuildingObjects = new();
    public Transform BuildingsContainer;
    public Transform BuildingsUIContainer;

    public BuildingScript SelectedBuilding;
    public int PlacingBuildingIndex = -1;

    [Space(10)]
    public Grid Grid;
    public VillageMode CurrentMode;

    [Header("UI")]
    public UiManager UiManager;

    [Header("Debug")]
    public GameObject DebugCube;

    private List<GameObject> BuildingTemplate = new();

    private void Start()
    {
        Grid.Create();

        GameObject container = new GameObject("Templates");
        container.transform.parent = BuildingsContainer;
        for (var i = 0; i < BuildingObjects.Count; i++)
        {
            Building desc = BuildingObjects[i];
            GameObject template = Instantiate(desc.Prefab, container.transform);
            template.SetActive(false);
            BuildingTemplate.Add(template);
        }

    }

    private void Update()
    {
        UpdateView();

        UpdatePlacement();
    }

    private void UpdateView()
    {
        if (CurrentMode != VillageMode.View)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Grid.CursorToGrid( out Vector2 gridPos))
            {

                Tile tile = Grid.GetTile(gridPos);

                if (tile.BuildingScript != null)
                {
                    SelectedBuilding = tile.BuildingScript;
                    tile.BuildingScript.OnClick();
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

    private void UpdatePlacement()
    {
        if (CurrentMode != VillageMode.Placement)
            return;

        if (PlacingBuildingIndex == -1)
            return;

        if (Grid.CursorToGrid(out Vector2 gridPos))
        {
            // Build preview
            if (!Grid.IsOccupied(gridPos, BuildingObjects[PlacingBuildingIndex].Fondation))
            {
                BuildingTemplate[PlacingBuildingIndex].SetActive(true);
                BuildingTemplate[PlacingBuildingIndex].transform.position = new (gridPos.x, 0, gridPos.y);
            }

            // Cursor
            DebugCube.transform.position = Grid.GridToWorld(gridPos);

            // Placement
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Grid.PlaceBuilding(gridPos, BuildingObjects[PlacingBuildingIndex]);
                Grid.GetTile(gridPos).BuildingScript.Instantiate( BuildingsUIContainer );
                BuildingTemplate[PlacingBuildingIndex].SetActive(false);
                ToViewMode();
            }
        }
    }

    public void StartPlacing(int index)
    {
        PlacingBuildingIndex = index;
        ToPlacementMode();
    }

    public void StopPlacing()
    {
        PlacingBuildingIndex = -1;
        ToViewMode();
    }

    public void ToViewMode() => ChangeMode(VillageMode.View);
    public void ToPlacementMode() => ChangeMode(VillageMode.Placement);
    public void ToEditMode()  => ChangeMode(VillageMode.Edit);

    public void ChangeMode(VillageMode mode)
    {
        CurrentMode = mode;
        UiManager.Activate(mode);
    }
}