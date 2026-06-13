using UnityEngine;
using UnityEngine.InputSystem;

public class VillageCameraController : MonoBehaviour
{
    public GameObject Target;

    public float Distance;
    public float CameraSize;
    public float Speed;
    public float TargetSpeed;
    public Vector2 BoxHalf;

    InputAction MouseMove;
    InputAction MousePress;

    Camera myCamera;

    bool IsDraging;
    Vector2 MouseDelta;

    Vector3 ROTATEDY;
    Vector3 ROTATEDX;

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Village").Enable();
    }
    private void OnDisable()
    {
        InputSystem.actions.FindActionMap("Village").Disable();
    }

    private void Awake()
    {
        MouseMove = InputSystem.actions.FindAction("MouseMove");
        MousePress = InputSystem.actions.FindAction("MousePress");

        float deg45 = Mathf.Cos(Mathf.PI * 0.25f);
        ROTATEDY = new Vector3(-deg45, 0, -deg45);
        ROTATEDX = new Vector3(-deg45, 0, deg45);

        IsDraging = false;
        MouseDelta = Vector3.zero;
        
        myCamera = GetComponent<Camera>();
    }
    private void Update()
    {
        MouseDelta = MouseMove.ReadValue<Vector2>();
        IsDraging = MousePress.IsPressed();

        transform.position = Vector3.Lerp(transform.position, Target.transform.position - transform.forward * Distance, Speed * Time.deltaTime);

        myCamera.orthographicSize = CameraSize;

        if (IsDraging) {
            Target.transform.position  += (ROTATEDX * MouseDelta.x + ROTATEDY * MouseDelta.y) * (TargetSpeed * Time.deltaTime);
            Vector3 tragetPos = Target.transform.position;
            if (tragetPos.x < 0)
            {
                tragetPos.x = 0;
            }
            else if (tragetPos.x > BoxHalf.x * 2.0f)
            {
                tragetPos.x = BoxHalf.x * 2.0f;
            }

            if (tragetPos.z < 0)
            {
                tragetPos.z = 0;
            }
            else if (tragetPos.z > BoxHalf.y * 2.0f)
            {
                tragetPos.z = BoxHalf.y * 2.0f;
            }
            Target.transform.position = tragetPos;
        }
    }

}
