using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum OccupationType
{
    Building,
    Decoration,
    Empty
}

public class BuildingData
{
    public Building Descriptor;
    public Vector2 Position;
    public int DayLeftToBuild;
}

public class Tile
{
    public Vector2 Position;
    public OccupationType Occupation;
    public BuildingData BuildingData;
    public BuildingScript BuildingScript;
}

[System.Serializable]
public class Grid
{
    public List<BuildingData> BuildingPlaced = new();
    public List<BuildingScript> BuildingScripts = new();

    public int Width = 60;
    public int Height = 60;

    public GameObject GridPreview;
    public Material GridMaterial;

    private Tile[,] _tiles;

    private int _layerMask;

    public void Create()
    {
        _layerMask = LayerMask.GetMask("Ground");
        _tiles = new Tile[Width, Height];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                _tiles[x, y] = new Tile();
                _tiles[x, y].Position = new(x, y);
                _tiles[x, y].Occupation = OccupationType.Empty;
            }
        }
    }

    public Vector2 WorldToGrid(Vector3 position)
    {
        return new( (int)position.x, (int)position.z );
    }

    private bool IsOccupied(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return true;

        if ( _tiles[x, y] == null )
            return false;

        if ( _tiles[x, y].Occupation == OccupationType.Decoration )
            return false;

        if ( _tiles[x, y].Occupation == OccupationType.Empty )
            return false;

        return true;
    }

    public Vector3 GridToWorld(Vector2 gridPos) => new Vector3(gridPos.x, 0.1f, gridPos.y);

    public bool CursorToGrid(out Vector2 point)
    {
        Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            Vector3 worldPos = hit.point;
            point = WorldToGrid(worldPos);

            return true;
        }
        point = Vector3.zero;
        return false;
    }

    public bool CenterOfScreenToPoint(out Vector2 point)
    {
        Vector2 screenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask))
        {
            Vector3 worldPos = hit.point;
            point = WorldToGrid(worldPos);

            return true;
        }
        point = Vector2.zero;
        return false;
    }

    public bool IsOccupied(Vector2 position, BuildingFondation fondation)
    {
        bool occupied = false;
        for (int x = (int)position.x; x <= position.x + fondation.Width; ++x) {
            for (int y = (int)position.y; y <= position.y + fondation.Height; ++y) {
                occupied |= IsOccupied(x, y);
            }
        }

        return occupied;
    }

    public bool PlaceBuilding(Vector2 position, Building building, Transform buildingContainer, Transform buildingUiContainer)
    {
        if ( IsOccupied(position, building.Fondation) )
            return false;

        Inventory.Instance.Pay(building.GlobalBuildingData.PerLevelUpgradeCost);

        GameObject gameObject = Object.Instantiate(building.Prefab, GridToWorld(position), Quaternion.identity, buildingContainer);
        BuildingScript script = gameObject.GetComponentInChildren<BuildingScript>();
        BuildingData data = new BuildingData();
        data.Descriptor = building;
        data.Position = position;
        data.DayLeftToBuild = building.GlobalBuildingData.BuildingDuration;
        BuildingPlaced.Add(data);
        script.Data = data;
        for (int x = (int)position.x; x <= position.x + building.Fondation.Width; ++x) {
            for (int y = (int)position.y; y <= position.y + building.Fondation.Height; ++y) {
                _tiles[x, y].Occupation = OccupationType.Building;
                _tiles[x, y].BuildingData = data;
                _tiles[x, y].BuildingScript = script;
            }
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        script.Instantiate( buildingUiContainer );
        script.OnUnselected();
        BuildingScripts.Add( script );

        return true;
    }

    public Tile GetTile(Vector2 position)
    {
        if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height)
            return null;

        return _tiles[(int)position.x, (int)position.y];
    }

    public void Show()
    {
        GridPreview.SetActive(true);
    }

    public void Hide()
    {
        GridPreview.SetActive(false);
    }

}
