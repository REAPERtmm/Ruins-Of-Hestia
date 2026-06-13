
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(menuName = "ScriptableObject/Room")]
public class RoomSO : ScriptableObject
{

    public Vector2Int Size = new Vector2Int(0, 0);
    public bool[] Terrain;
    public bool[] Ennemi;
    public bool[] Resource;
    public Vector2Int PlayerSpawn;

    public Color GetVisual(int x, int y)
    {
        if (GetTerrain(x, y))
        {
            if (GetEnnemi(x, y))
            {
                if (GetResource(x, y)) return Color.purple;
                return Color.red;
            }
            else
            {
                if (GetResource(x, y)) return Color.blue;
            }
            return Color.white;
        }
        else
        {
            return Color.black;
        }
    }

    public void ResizeTo(int width, int height)
    {
        Vector2Int newSize = new Vector2Int(width, height);
        bool[] newTerrain = new bool[newSize.x * newSize.y];
        bool[] newEnnemi = new bool[newSize.x * newSize.y];
        bool[] newResource = new bool[newSize.x * newSize.y];
        if (Terrain != null)
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    if (i >= newSize.x || j >= newSize.y || i >= Size.x || j >= Size.y) continue;
                    newTerrain[j * newSize.x + i] = Terrain[j * Size.x + i];
                    newEnnemi[j * newSize.x + i] = Ennemi[j * Size.x + i];
                    newResource[j * newSize.x + i] = Resource[j * Size.x + i];
                }
            }
        }
        Terrain = newTerrain;
        Ennemi = newEnnemi;
        Resource = newResource;
        Size = newSize;
    }

    public bool GetTerrain(int x, int y) => Terrain[y * Size.x + x];
    public void SetTerrain(int x, int y, bool value) => Terrain[y * Size.x + x] = value;

    public bool GetEnnemi(int x, int y) => Ennemi[y * Size.x + x];
    public void SetEnnemi(int x, int y, bool value) => Ennemi[y * Size.x + x] = value;

    public bool GetResource(int x, int y) => Resource[y * Size.x + x];
    public void SetResouce(int x, int y, bool value) => Resource[y * Size.x + x] = value;

    public Vector2Int GetPlayer() => PlayerSpawn;
    public void SetPlayer(Vector2Int position) => PlayerSpawn = position;
}

[CustomEditor(typeof(RoomSO))]
class RoomSOEditor : Editor
{
    enum RoomBrush
    {
        Terrain,
        Ennemi,
        Resource,
        Player
    }

    int lastSizeSliderValue = -1;
    bool isDragging = false;
    bool dragValue = false;
    RoomBrush currentBrush = RoomBrush.Terrain;

    VisualElement[,] GridVisual;

    static StyleColor ACTIVE = new StyleColor(new Color(1, 1, 1));
    static StyleColor INACTIVE = new StyleColor(new Color(0, 0, 0));

    void UpdateGridVisual()
    {
        RoomSO inspected = (RoomSO)target;
        for (int j = 0; j < inspected.Size.y; ++j)
        {
            for (int i = 0; i < inspected.Size.x; ++i)
            {
                Color color = inspected.GetVisual(i, j);
                if (inspected.GetPlayer().x == i && inspected.GetPlayer().y == j) color = Color.yellow;
                StyleColor active = new StyleColor(color);

                GridVisual[i, j].style.backgroundColor    = active;
                GridVisual[i, j].style.borderBottomColor  = inspected.GetTerrain(i, j) ? INACTIVE : ACTIVE;
                GridVisual[i, j].style.borderTopColor     = inspected.GetTerrain(i, j) ? INACTIVE : ACTIVE;
                GridVisual[i, j].style.borderLeftColor    = inspected.GetTerrain(i, j) ? INACTIVE : ACTIVE;
                GridVisual[i, j].style.borderRightColor   = inspected.GetTerrain(i, j) ? INACTIVE : ACTIVE;
            }
        }
    }

    void SetValue(int x, int y, bool value)
    {
        RoomSO inspected = (RoomSO)target;
        switch (currentBrush)
        {
            default:
            case RoomBrush.Terrain:
                {
                    inspected.SetTerrain(x, y, value);
                    break;
                }
            case RoomBrush.Resource:
                {
                    if(inspected.GetTerrain(x, y))
                        inspected.SetResouce(x, y, value);
                    break;
                }
            case RoomBrush.Ennemi:
                {
                    if (inspected.GetTerrain(x, y))
                        inspected.SetEnnemi(x, y, value);
                    break;
                }
            case RoomBrush.Player:
                {
                    if (inspected.GetTerrain(x, y))
                        inspected.SetPlayer(new Vector2Int(x, y));
                    break;
                }
        }
        UpdateGridVisual();
        EditorUtility.SetDirty(inspected);
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        RoomSO inspected = (RoomSO)target;
        if (inspected.Terrain == null)
        {
            inspected.Size = new Vector2Int(16, 16);
            inspected.Terrain = new bool[16 * 16];
            inspected.Ennemi = new bool[16 * 16];
            inspected.Resource = new bool[16 * 16];
            inspected.Terrain[8 * 16 + 8] = true;
            inspected.PlayerSpawn = new Vector2Int(8, 8);
        }

        if (lastSizeSliderValue < 0) { 
            lastSizeSliderValue = (int)Mathf.Log(inspected.Size.x, 2.0f);
        }

        Vector2IntField SizeVisual = new Vector2IntField();
        SizeVisual.value = inspected.Size;
        SizeVisual.SetEnabled(false);

        SliderInt SizeSlider = new SliderInt(1, 5);
        SizeSlider.value = lastSizeSliderValue;
        SizeSlider.RegisterValueChangedCallback(ctx => {
            Undo.RecordObject(inspected, "Resize Room");

            lastSizeSliderValue = ctx.newValue;
            int size = (int)Mathf.Pow(2.0f, ctx.newValue); 
            inspected.ResizeTo(size, size);

            EditorUtility.SetDirty(inspected);
        });

        root.Add(SizeVisual);
        root.Add(SizeSlider);

        GridVisual = new VisualElement[inspected.Size.x, inspected.Size.y];
        VisualElement[] Lines = new VisualElement[inspected.Size.y];

        for (int j = 0; j < inspected.Size.y; ++j)
        {
            Lines[j] = new VisualElement();
            Lines[j].style.flexDirection = FlexDirection.Row;
            for (int i = 0; i < inspected.Size.x; ++i)
            {
                int x, y;
                x = i;
                y = j;

                GridVisual[i, j] = new VisualElement();
                GridVisual[i, j].style.width = 15;
                GridVisual[i, j].style.height = 15;
                GridVisual[i, j].style.borderBottomWidth = 1;
                GridVisual[i, j].style.borderTopWidth = 1;
                GridVisual[i, j].style.borderLeftWidth = 1;
                GridVisual[i, j].style.borderRightWidth = 1;

                GridVisual[i, j].RegisterCallback<PointerDownEvent>(ctx =>
                {
                    isDragging = true;

                    dragValue = ctx.button == 0;

                    SetValue(x, y, dragValue);

                    ctx.StopPropagation();
                });

                GridVisual[i, j].RegisterCallback<PointerEnterEvent>(evt =>
                {
                    if (!isDragging)  return;
                    SetValue(x, y, dragValue);
                });

                Lines[j].Add(GridVisual[i, j]);
            }
            root.Add(Lines[j]);
        }

        VisualElement ToolBar = new VisualElement();
        ToolBar.style.flexDirection = FlexDirection.Row;

        Label TerrainBrush     = new Label();
        Label EnnemiBrush      = new Label();
        Label ResourceBrush    = new Label();
        Label PlayerBrush      = new Label();

        currentBrush = RoomBrush.Terrain;
        TerrainBrush.style.color = ACTIVE;
        EnnemiBrush.style.color = INACTIVE;
        ResourceBrush.style.color = INACTIVE;
        PlayerBrush.style.color = INACTIVE;

        TerrainBrush.text = "Terrain";
        TerrainBrush.RegisterCallback<PointerDownEvent>(ctx =>
        {
            currentBrush = RoomBrush.Terrain;

            TerrainBrush.style.color = ACTIVE;
            EnnemiBrush.style.color = INACTIVE;
            ResourceBrush.style.color = INACTIVE;
            PlayerBrush.style.color = INACTIVE;
        });

        EnnemiBrush.text = "Ennemi";
        EnnemiBrush.RegisterCallback<PointerDownEvent>(ctx =>
        {
            Debug.Log("Ennemi");
            currentBrush = RoomBrush.Ennemi;
            TerrainBrush.style.color = INACTIVE;
            EnnemiBrush.style.color = ACTIVE;
            ResourceBrush.style.color = INACTIVE;
            PlayerBrush.style.color = INACTIVE;
        });

        ResourceBrush.text = "Resource";
        ResourceBrush.RegisterCallback<PointerDownEvent>(ctx =>
        {
            currentBrush = RoomBrush.Resource;

            TerrainBrush.style.color = INACTIVE;
            EnnemiBrush.style.color = INACTIVE;
            ResourceBrush.style.color = ACTIVE;
            PlayerBrush.style.color = INACTIVE;
        });

        PlayerBrush.text = "Player";
        PlayerBrush.RegisterCallback<PointerDownEvent>(ctx =>
        {
            currentBrush = RoomBrush.Player;

            TerrainBrush.style.color = INACTIVE;
            EnnemiBrush.style.color = INACTIVE;
            ResourceBrush.style.color = INACTIVE;
            PlayerBrush.style.color = ACTIVE;
        });

        ToolBar.Add(TerrainBrush);
        ToolBar.Add(EnnemiBrush);
        ToolBar.Add(ResourceBrush);
        ToolBar.Add(PlayerBrush);
        root.Add(ToolBar);

        root.RegisterCallback<PointerUpEvent>(ctx =>
        {
            isDragging = false;
        });

        UpdateGridVisual();

        return root;
    }

}
