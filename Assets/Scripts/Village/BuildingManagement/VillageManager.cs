using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum VillageMode
{
    View,
    Edit,
    Placement
}

public class VillageManager : MonoBehaviour
{
    public List<Building> BuildingObjects = new();
    private List<GameObject> BuildingTemplate = new();

    public GameObject BuildingsContainer;

    public GameObject Cube;

    public Grid Grid;

    public VillageMode CurrentMode;

    private void Start()
    {
        Grid.Create();


        GameObject container = new GameObject("Templates");
        container.transform.parent = BuildingsContainer.transform;
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
        if (Grid.CursorToGrid(out Vector2 gridPos))
        {
            Cube.transform.position = Grid.GridToWorld(gridPos);
        }

        UpdatePlacement();

    }

    private void UpdatePlacement()
    {
        if (CurrentMode == VillageMode.Placement)
        {
            if (Grid.CursorToGrid(out Vector2 gridPos))
            {
                if (!Grid.IsOccupied(gridPos, BuildingObjects[0].Fondation))
                {
                    BuildingTemplate[0].SetActive(true);
                    BuildingTemplate[0].transform.position = new (gridPos.x, 0, gridPos.y);
                }
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