using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

public struct TileGroup
{
    public bool TL, TR;
    public bool BL, BR;
    public TileGroup(bool tl, bool tr, bool bl, bool br)
    {
        TL = tl; TR = tr;
        BL = bl; BR = br;
    }

    public TileGroup Rotate()
    {
        TileGroup rotated;
        rotated.BL = TL;
        rotated.TL = TR;
        rotated.TR = BR;
        rotated.BR = BL;
        return rotated;
    }

    public bool IsEmpty() {  return !TL && !TR && !BR && !BL; }
    public bool IsFull() {  return TL && TR && BR && BL; }
}

[CreateAssetMenu(fileName = "MeshTileGroup", menuName = "ScriptableObject/MeshTileGroup")]
public class MeshTileGroup : ScriptableObject
{
    public bool TopLeft = false;
    public bool TopRight = false;
    public bool BottomLeft = false;
    public bool BottomRight = false;
    public Mesh[] Models;
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshTileGroup))]
public class MeshTileGroupEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        MeshTileGroup inspected = (MeshTileGroup)target;

        VisualElement TopRow = new VisualElement();
        TopRow.style.flexDirection = FlexDirection.Row;

        Toggle TopLeft = new Toggle();
        TopLeft.value = inspected.TopLeft;
        TopLeft.RegisterValueChangedCallback(ctx => inspected.TopLeft = ctx.newValue);

        Toggle TopRight = new Toggle();
        TopRight.value = inspected.TopRight;
        TopRight.RegisterValueChangedCallback(ctx => inspected.TopRight = ctx.newValue);

        TopRow.Add(TopLeft);
        TopRow.Add(TopRight);

        VisualElement BottomRow = new VisualElement();
        BottomRow.style.flexDirection = FlexDirection.Row;

        Toggle BottomLeft = new Toggle();
        BottomLeft.value = inspected.BottomLeft;
        BottomLeft.RegisterValueChangedCallback(ctx => inspected.BottomLeft = ctx.newValue);

        Toggle BottomRight = new Toggle();
        BottomRight.value = inspected.BottomRight;
        BottomRight.RegisterValueChangedCallback(ctx => inspected.BottomRight = ctx.newValue);

        BottomRow.Add(BottomLeft);
        BottomRow.Add(BottomRight);

        PropertyField Models = new PropertyField(serializedObject.FindProperty("Models"));

        root.Add(TopRow);
        root.Add(BottomRow);
        root.Add(Models);

        return root;
    }
}

#endif