using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{

    public float Offset = 1.0f;
    private List<SelectorAnimation> Arrows = new List<SelectorAnimation>();

    public Material     ValidPlace;
    public Material     InvalidPlace;

    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Arrows.Add(gameObject.transform.GetChild(i).GetComponent<SelectorAnimation>());
        }
    }

    public void Show()
    {
        Arrows.ForEach(arrow => arrow.gameObject.SetActive(true));
    }

    public void Hide()
    {
        Arrows.ForEach(arrow => arrow.gameObject.SetActive(false));
    }

    public void Resize(Vector3 center, Vector2 halfExtend)
    {
        if (gameObject.transform.position == center)
            return;

        gameObject.transform.position = center;

        Arrows[0].UpdatePosition (new Vector3(-halfExtend.x - Offset, 0.1f, 0   ));
        Arrows[1].UpdatePosition (new Vector3(halfExtend.x + Offset, 0.1f, 0    ));
        Arrows[2].UpdatePosition (new Vector3(0, 0.1f, halfExtend.y + Offset    ));
        Arrows[3].UpdatePosition (new Vector3(0, 0.1f, -halfExtend.y - Offset   ));
    }

    public void SetGroundColor( GameObject build, bool available )
    {
        MeshRenderer renderer = build.transform.GetChild(0).GetComponent<MeshRenderer>();
        renderer.material = available ? ValidPlace : InvalidPlace;
    }
}
