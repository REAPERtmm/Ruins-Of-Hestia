using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController INSTANCE;

    [Header("References")]
    [SerializeField] Animator CaliAnimator;

    [Header("Parameters")]
    [SerializeField] float Speed;
    [SerializeField] float Acceleration;

    Quaternion LookingToward;
    Vector2 Velocity = Vector2.zero;

    CharacterController characterController;
    InputAction ControlMove;

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Exploration").Enable();
    }
    private void OnDisable()
    {
        InputSystem.actions.FindActionMap("Exploration").Disable();
    }
    private void Awake()
    {
        ControlMove = InputSystem.actions.FindAction("ExplorationMove");
        characterController = GetComponent<CharacterController>();
        LookingToward = Quaternion.identity;
        if(INSTANCE == null)
            INSTANCE = this;
    }

    void Update()
    {
        const float COS45 = 0.70710678f;
        const float SIN45 = 0.70710678f;

        Vector2 Input = ControlMove.ReadValue<Vector2>();

        Vector2 TowardedMove;
        if (Input.x != 0 || Input.y != 0)
        {
            TowardedMove = new Vector2(Input.x, Input.y);
            CaliAnimator.SetBool("IsMoving", true);
        }
        else
        {
            TowardedMove = Vector3.zero;
            CaliAnimator.SetBool("IsMoving", false);
        }

        Velocity = Vector2.Lerp(Velocity, TowardedMove, Acceleration * Time.deltaTime);

        CaliAnimator.SetFloat("Speed", Velocity.magnitude);

        Vector3 movement = new Vector3(
                Velocity.x * COS45 + Velocity.y * SIN45,
                0,
                -Velocity.x * SIN45 + Velocity.y * COS45
                ) * Speed * Time.deltaTime;
        characterController.Move( movement );

        Ray below = new Ray(transform.position, Vector3.down);
        var hits = Physics.RaycastAll(below, transform.localScale.y * 0.1f);
        bool isGrounded = false;
        foreach (var hit in hits) { 
            if(hit.collider.tag == "Ground")
            {
                isGrounded = true;
                transform.position = hit.point;
                break;
            }
        }
        if (!isGrounded) {
            characterController.Move(-movement);
        }
        else if(Input.x != 0.0f || Input.y != 0.0f)
        {
            LookingToward = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = LookingToward;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * transform.localScale.y * 0.1f);
    }
}
