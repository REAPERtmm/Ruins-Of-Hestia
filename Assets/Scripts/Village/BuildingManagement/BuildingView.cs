using UnityEngine;

[ExecuteAlways]
public class BuildingView : MonoBehaviour
{
    public MeshRenderer View;
    public Vector2 Scale;
    public Vector3 Offset;


    private void Update()
    {
        View.transform.localScale = new Vector3(Scale.x, Scale.y, 1);
        View.transform.localPosition = new Vector3(0, Scale.y * 0.5f, 0) + Offset;
    }

}
