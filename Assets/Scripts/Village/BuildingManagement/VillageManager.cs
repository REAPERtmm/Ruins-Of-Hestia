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
    public List<Building> BuildingObjects = new();
    public Transform BuildingsContainer;
    public Transform BuildingsUIContainer;

    public BuildingScript SelectedBuilding;

    [Space(10)]
    public Grid Grid;
    public VillageMode CurrentMode;

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

        if (Grid.CursorToGrid(out Vector2 gridPos))
        {
            // Build preview
            if (!Grid.IsOccupied(gridPos, BuildingObjects[0].Fondation))
            {
                BuildingTemplate[0].SetActive(true);
                BuildingTemplate[0].transform.position = new (gridPos.x, 0, gridPos.y);
            }

            // Cursor
            DebugCube.transform.position = Grid.GridToWorld(gridPos);

            // Placement
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Grid.PlaceBuilding(gridPos, BuildingObjects[0]);
                Grid.GetTile(gridPos).BuildingScript.Instantiate( BuildingsUIContainer );
                BuildingTemplate[0].SetActive(false);
                ToViewMode();
            }
        }

    }

    public void ToViewMode() => ChangeMode(VillageMode.View);
    public void ToPlacementMode() => ChangeMode(VillageMode.Placement);
    public void ToEditMode()  => ChangeMode(VillageMode.Edit);

    public void ChangeMode(VillageMode mode)
    {
        CurrentMode = mode;
    }
}