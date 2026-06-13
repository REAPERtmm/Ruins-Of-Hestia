using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController INSTANCE;

    [Header("References")]
    [SerializeField] Image PlayerMiniMapUI;
    [SerializeField] Animator CaliAnimator;
    [SerializeField] public CombatController Combat;
    [SerializeField] EnnemiManager ManagerEnnemis;
    [SerializeField] ResourceManager ManagerResources;
    MapGeneration MapGenerationManager;

    [Header("Parameters")]
    [SerializeField] float Speed;
    [SerializeField] float Acceleration;

    Quaternion LookingToward;
    Vector2 Velocity = Vector2.zero;

    CharacterController characterController;
    InputAction ControlMove;

    [SerializeField] ResourceDescriptor closestResource = null;
    [SerializeField] Ennemi closestEnnemi = null;

    public Vector2 NormalizedPlayerPositionInMap
    {
        get
        {
            if (MapGenerationManager == null) return new Vector2(transform.position.x, transform.position.z);
            return new Vector2(transform.position.x / MapGenerationManager.GenerationSizeX, transform.position.z / MapGenerationManager.GenerationSizeY);
        }
    }

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

    private void Start()
    {
        MapGenerationManager = transform.parent.GetComponent<MapGeneration>();
    }

    void UpdateResourceTarget()
    {
        if(ManagerResources == null)
        {
            Debug.Log("No Resource Manager attached");
            return;
        }

        ResourceDescriptor closest = ManagerResources.GetClosestResource(transform.position);
        if(closest == null)
        {
            Debug.Log("No resource found");
            return;
        }

        if (closest != closestResource) {

            if (closestResource != null && closestResource.ResourceTransform != null)
            {
                // Debug.Log("Changed closest from : " + closestResource.ResourceTransform + " / to : " + closest.ResourceTransform);
                closestResource.ResourceTransform.localScale = Vector3.one;
            }
            closest.ResourceTransform.localScale = Vector3.one * 1.3f;
            closestResource = closest;
        }

    }

    void UpdateEnnemiTarget()
    {
        if (ManagerEnnemis == null)
        {
            Debug.Log("No Ennemi Manager attached");
            return;
        }

        Ennemi closest = ManagerEnnemis.GetClosestEnnemi(transform.position);
        if (closest == null)
        {
            Debug.Log("No ennemi found");
            return;
        }

        if (closest != closestEnnemi)
        {

            if (closestEnnemi != null && closestEnnemi.Controller != null)
            {
                // Debug.Log("Changed closest from : " + closestResource.ResourceTransform + " / to : " + closest.ResourceTransform);
                closestEnnemi.Controller.transform.localScale = Vector3.one;
            }
            closest.Controller.transform.localScale = Vector3.one * 1.3f;
            closestEnnemi = closest;
        }

    }

    void UpdateMiniMapPlayerUI()
    {
        if(MapGenerationManager == null)
        {
            return;
        }

        Vector2 normalized_player_position = NormalizedPlayerPositionInMap;
        Vector2 centered = normalized_player_position - Vector2.one * 0.5f;

        const float RECT_SIZE = 360;
        const float RECT_SCALE = 1.0f;
        const float RECT_RESCALED = RECT_SIZE * RECT_SCALE;

        PlayerMiniMapUI.rectTransform.localPosition = centered * RECT_RESCALED;
    }

    void UpdateMovements()
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
        characterController.Move(movement);

        Ray below = new Ray(transform.position, Vector3.down);
        var hits = Physics.RaycastAll(below, transform.localScale.y * 0.1f);
        bool isGrounded = false;
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Ground")
            {
                isGrounded = true;
                transform.position = hit.point;
                break;
            }
        }
        if (!isGrounded)
        {
            characterController.Move(-movement);
        }
        else if (Input.x != 0.0f || Input.y != 0.0f)
        {
            LookingToward = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = LookingToward;
        }
    }

    void Update()
    {
        UpdateMovements();
        UpdateMiniMapPlayerUI();
        UpdateResourceTarget();
        UpdateEnnemiTarget();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * transform.localScale.y * 0.1f);
    }
}
