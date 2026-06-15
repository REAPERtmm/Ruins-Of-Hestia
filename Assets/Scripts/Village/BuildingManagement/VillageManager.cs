using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public List<Building>           BuildingObjects = new();
    public List<GlobalBuildingData> GlobalBuildingData = new();
    public Transform                BuildingsContainer;
    public Transform                BuildingsUIContainer;

    public Selector     Selector;

    [Space(10)]
    public Grid         Grid;
    public VillageMode  CurrentMode;

    [Header("UI")]
    public UiManager    UiManager;

    [Header("Debug")]
    public GameObject   DebugCube;
    public BuildingScript SelectedBuilding;
    public int PlacingBuildingIndex = -1;

    public int CurrentCityLevel = 1;

    private List<GameObject> BuildingTemplate = new();

    public Action<VillageManager> OnVillageLevelUp;
    public Action<Building> OnBuildingPlaced;

    private void Start()
    {
        DOTween.Init();

        Grid.Create();

        GameObject container = new GameObject("Templates");
        container.transform.parent = BuildingsContainer;
        for (var i = 0; i < BuildingObjects.Count; i++)
        {
            Building desc = BuildingObjects[i];
            GlobalBuildingData.Add(desc.GlobalBuildingData);
            GlobalBuildingData globalBuildingData = GlobalBuildingData[i];
            for (int level = 0; level < CurrentCityLevel; level++)
            {
                globalBuildingData.BaseBuildingCount += globalBuildingData.PerLevelExtention[level];
            }
            GameObject template = Instantiate(desc.Prefab, container.transform);
            template.SetActive(false);
            BuildingTemplate.Add(template);
        }

    }

    private void Update()
    {
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            foreach (var buildingScript in Grid.BuildingScripts)
            {
                buildingScript.PassDay();
            }
        }

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
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

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

            bool isAvailable = !Grid.IsOccupied(gridPos, BuildingObjects[PlacingBuildingIndex].Fondation);

            // Build preview
            BuildingTemplate[PlacingBuildingIndex].SetActive(true);
            BuildingTemplate[PlacingBuildingIndex].transform.position = new (gridPos.x, 0, gridPos.y);
            float halfX = BuildingObjects[PlacingBuildingIndex].Fondation.Width/2.0f;
            float halfY = BuildingObjects[PlacingBuildingIndex].Fondation.Height / 2.0f; // TODO Pass this in selector
            Selector.Resize( new Vector3( gridPos.x + halfX, 0.1f, gridPos.y + halfY), new Vector2(halfX, halfY) );
            Selector.SetGroundColor( BuildingTemplate[PlacingBuildingIndex], isAvailable );

            // Cursor
            DebugCube.transform.position = Grid.GridToWorld(gridPos);

            // Placement
            if (Mouse.current.leftButton.wasPressedThisFrame && isAvailable)
            {
                Grid.PlaceBuilding(gridPos, BuildingObjects[PlacingBuildingIndex], BuildingsUIContainer);
                StopPlacing();
            }
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            LevelUpCity();
        }
    }

    public void StartPlacing(int index)
    {
        PlacingBuildingIndex = index;
        Selector.Show();
        ToPlacementMode();
    }

    public void StopPlacing()
    {
        ToViewMode();
        Selector.Hide();
        PlacingBuildingIndex = -1;
    }

    public void ToViewMode() {
        BuildingTemplate[PlacingBuildingIndex].SetActive(false);
        Grid.Hide();
        ChangeMode(VillageMode.View);
    }
    public void ToPlacementMode() {
        ChangeMode(VillageMode.Placement);
        Grid.Show();
    }
    public void ToEditMode()  => ChangeMode(VillageMode.Edit);

    public void ChangeMode(VillageMode mode)
    {
        CurrentMode = mode;
        UiManager.Activate(mode);
    }

    public void LevelUpCity()
    {
        CurrentCityLevel++;
        for (var i = 0; i < GlobalBuildingData.Count; i++)
        {
            GlobalBuildingData[i].BaseBuildingCount += GlobalBuildingData[i].PerLevelExtention[CurrentCityLevel];
        }
    }
}