using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    public UiManager        UiManager;
    public PlacementModeUi  UiPlacement;
    public ResourceUI       UiResource;
    public GameObject       UiBillboard;

    [Header("Debug")]
    public GameObject   DebugCube;
    public BuildingScript SelectedBuilding;
    public int PlacingBuildingIndex = -1;

    public int CurrentCityLevel = 1;


    public Action<VillageManager> OnVillageLevelUp;
    public Action<Building> OnBuildingPlaced;

    private List<GameObject> BuildingTemplate = new();
    private Vector2 _selectedBuildingPosition;

    [Header("Camera")]
    public VillageCameraController CameraController;

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
                globalBuildingData.BaseBuildingCount += globalBuildingData.PerLevelBuildCountAdd[level];
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
            PassDay();
        }

        UpdateView();

        UpdatePlacement();
    }

    private void UpdateView()
    {
        if (CurrentMode != VillageMode.View)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Grid.CursorToGrid( out Vector2 gridPos))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                Tile tile = Grid.GetTile(gridPos);

                if (tile == null)
                    return;

                if (tile.BuildingScript != null)
                {
                    if (tile.BuildingScript.OnClick() == false)
                        return;

                    SelectedBuilding = tile.BuildingScript;
                    UiManager.ViewModeGroup.SetActive(false);
                    CameraController.FocusOn(tile.BuildingScript.transform.position);
                }
                else
                {
                    if (SelectedBuilding != null)
                    {
                        SelectedBuilding.OnUnselected();
                        SelectedBuilding = null;
                        UiManager.ViewModeGroup.SetActive(true);
                        CameraController.CancelFocus();
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

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Grid.CursorToGrid(out Vector2 gridPos))
        {

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            // Build preview
            PlacePreviewAt(gridPos);

            // Cursor
            DebugCube.transform.position = Grid.GridToWorld(gridPos);
            _selectedBuildingPosition = gridPos;

        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            LevelUpCity();
        }
    }

    public void StartPlacing(int index)
    {
        PlacingBuildingIndex = index;
        BuildingTemplate[PlacingBuildingIndex].SetActive(true);

        if (Grid.CenterOfScreenToPoint(out Vector2 gridPos))
        {
            // Build preview
            PlacePreviewAt(gridPos);
        }
        else
        {
            PlacePreviewAt(new (15, 15));
        }

        Selector.Show();
        ToPlacementMode();
        UiPlacement.ShowConfirmation();
    }

    public void StopPlacing()
    {
        ToPlacementMode();
        Selector.Hide();
        BuildingTemplate[PlacingBuildingIndex].SetActive(false);
        PlacingBuildingIndex = -1;
    }

    public void ToViewMode() {
        if (PlacingBuildingIndex > 0)
            BuildingTemplate[PlacingBuildingIndex].SetActive(false);
        Grid.Hide();
        ChangeMode(VillageMode.View);
    }
    public void ToPlacementMode() {
        ChangeMode(VillageMode.Placement);
        UiPlacement.ShowShop();
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
            GlobalBuildingData[i].BaseBuildingCount += GlobalBuildingData[i].PerLevelBuildCountAdd[CurrentCityLevel];
        }
    }

    public void ConfirmPlacement()
    {
        Building building = BuildingObjects[PlacingBuildingIndex];
        // Consume ressources
        if (!Inventory.Instance.CanAfford(building.GlobalBuildingData.PerLevelUpgradeCost))
        {
            UiResource.Highlight(building.GlobalBuildingData.PerLevelUpgradeCost);
            return;
        }

        if (!Grid.PlaceBuilding(_selectedBuildingPosition, building, BuildingsUIContainer))
        {
            // TODO FEEDBACK CANT PLACE
            return;
        }
        StopPlacing();
        ToViewMode();
    }

    public void PassDay()
    {
        foreach (var buildingScript in Grid.BuildingScripts)
        {
            buildingScript.PassDay();
        }
    }

    public void OpenBillboard()
    {
        UiBillboard.SetActive(true);
    }

    public void CloseBillboard()
    {
        ToViewMode();
        UiBillboard.SetActive(false);
    }

    public void ToExpeditionMap()
    {
        SceneManager.LoadScene(2);
    }

    private void PlacePreviewAt( Vector2 gridPos )
    {
        bool isAvailable = !Grid.IsOccupied(gridPos, BuildingObjects[PlacingBuildingIndex].Fondation);

        BuildingTemplate[PlacingBuildingIndex].transform.position = new (gridPos.x, 0, gridPos.y);
        float halfX = BuildingObjects[PlacingBuildingIndex].Fondation.Width/2.0f;
        float halfY = BuildingObjects[PlacingBuildingIndex].Fondation.Height / 2.0f; // TODO Pass this in selector
        Selector.Resize( new Vector3( gridPos.x + halfX, 0.1f, gridPos.y + halfY), new Vector2(halfX, halfY) );
        Selector.SetGroundColor( BuildingTemplate[PlacingBuildingIndex], isAvailable );
        _selectedBuildingPosition = gridPos;
    }
}