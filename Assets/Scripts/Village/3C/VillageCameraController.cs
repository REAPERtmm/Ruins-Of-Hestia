using DG.Tweening;
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

    [Header("Focus")]
    public Vector2 Offset;
    public float MinDuration = 0.3f;
    public float MaxDuration = 1.5f;
    public float SpeedReference = 20f;
    public Ease MoveEase = Ease.InOutCubic;
    private Tweener _tween;

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

    public void FocusOn(Vector3 worldPosition)
    {
        Vector3 destination = new Vector3(worldPosition.x + Offset.x, Target.transform.position.y, worldPosition.z + Offset.y);
        float distance = Vector3.Distance(Target.transform.position, destination);
        float duration = Mathf.Clamp(distance / SpeedReference, MinDuration, MaxDuration);


        _tween?.Kill();
        _tween = Target.transform.DOMove(destination, duration).SetEase(MoveEase);
    }

    public void CancelFocus()
    {
        _tween?.Kill();
    }

}
