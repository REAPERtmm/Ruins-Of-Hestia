using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(menuName = "ScriptableObject/Room")]
public class RoomSO : ScriptableObject
{
    public Vector2Int Size = new Vector2Int(0, 0);
    public bool[] Terrain = null;

    public void ResizeTo(int width, int height)
    {
        Vector2Int newSize = new Vector2Int(width, height);
        bool[] newTerrain = new bool[newSize.x * newSize.y];
        if (Terrain != null)
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    if (i >= newSize.x || j >= newSize.y || i >= Size.x || j >= Size.y) continue;
                    newTerrain[j * newSize.x + i] = Get(i, j);
                }
            }
        }
        Terrain = newTerrain;
        Size = newSize;
    }

    public bool Get(int x, int y) => Terrain[y * Size.x + x];
    public void Set(int x, int y, bool value) => Terrain[y * Size.x + x] = value;
}

[CustomEditor(typeof(RoomSO))]
class RoomSOEditor : Editor
{
    int lastSizeSliderValue = -1;
    bool isDragging = false;
    bool dragValue = false;

    static StyleColor ACTIVE = new StyleColor(new UnityEngine.Color(1, 1, 1));
    static StyleColor INACTIVE = new StyleColor(new UnityEngine.Color(0, 0, 0));

    void SetValue(VisualElement[,] grid, int x, int y, bool value)
    {
        RoomSO inspected = (RoomSO)target;
        grid[x, y].style.backgroundColor    = value ? ACTIVE : INACTIVE;
        grid[x, y].style.borderBottomColor  = value ? INACTIVE : ACTIVE;
        grid[x, y].style.borderTopColor     = value ? INACTIVE : ACTIVE;
        grid[x, y].style.borderLeftColor    = value ? INACTIVE : ACTIVE;
        grid[x, y].style.borderRightColor   = value ? INACTIVE : ACTIVE;
        inspected.Set(x, y, value);
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

        VisualElement[,] TerrainToggles = new VisualElement[inspected.Size.x, inspected.Size.y];
        VisualElement[] Lines = new VisualElement[inspected.Size.y];

        root.Add(SizeVisual);
        root.Add(SizeSlider);

        for (int j = 0; j < inspected.Size.y; ++j)
        {
            Lines[j] = new VisualElement();
            Lines[j].style.flexDirection = FlexDirection.Row;
            for (int i = 0; i < inspected.Size.x; ++i)
            {
                int x, y;
                x = i;
                y = j;

                TerrainToggles[i, j] = new VisualElement();
                TerrainToggles[i, j].style.width = 15;
                TerrainToggles[i, j].style.height = 15;
                TerrainToggles[i, j].style.borderBottomWidth = 1;
                TerrainToggles[i, j].style.borderTopWidth = 1;
                TerrainToggles[i, j].style.borderLeftWidth = 1;
                TerrainToggles[i, j].style.borderRightWidth = 1;

                SetValue(TerrainToggles, i, j, inspected.Get(i, j));

                TerrainToggles[i, j].RegisterCallback<PointerDownEvent>(ctx =>
                {
                    isDragging = true;

                    dragValue = ctx.button == 0;

                    SetValue(TerrainToggles, x, y, dragValue);

                    ctx.StopPropagation();
                });

                TerrainToggles[i, j].RegisterCallback<PointerEnterEvent>(evt =>
                {
                    if (!isDragging)  return;
                    SetValue(TerrainToggles, x, y, dragValue);
                });

                Lines[j].Add(TerrainToggles[i, j]);
            }
            root.Add(Lines[j]);
        }

        root.RegisterCallback<PointerUpEvent>(ctx =>
        {
            isDragging = false;
        });

        return root;
    }

}
