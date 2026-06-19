using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] TargetMarker TargetMarkerObject;
    [SerializeField] EnnemiManager ManagerEnnemis;
    [SerializeField] ResourceManager ManagerResources;
    [SerializeField] Image HealthBar;
    MapGeneration MapGenerationManager;

    [Header("Parameters")]
    [SerializeField] float Speed;
    [SerializeField] float DashSpeed;
    [SerializeField] float Acceleration;

    Quaternion LookingToward;
    Vector2 Velocity = Vector2.zero;
    Vector2 DashVelocity = Vector2.zero;
    bool IsDashLocked = false;

    CharacterController characterController;
    InputAction ControlMove;
    InputAction ControlDash;

    Inventory inventory;

    [SerializeField] ResourceDescriptor closestResource = null;
    [SerializeField] DG.Tweening.Sequence closestResourceSequence = null;
    [SerializeField] float closestResourceDistance = float.MaxValue;

    [SerializeField] Ennemi closestEnnemi = null;
    [SerializeField] float closestEnnemiDistance = float.MaxValue;
    [SerializeField] float last_attack_time = float.MinValue;

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
        ControlDash = InputSystem.actions.FindAction("ExplorationDash");
        characterController = GetComponent<CharacterController>();
        LookingToward = Quaternion.identity;
        if(INSTANCE == null)
            INSTANCE = this;
    }

    private void Start()
    {
        MapGenerationManager = transform.parent.GetComponent<MapGeneration>();
        inventory = GetComponent<Inventory>();
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

        float distance = Vector3.Distance(closest.ResourceTransform.position, transform.position);

        if(distance < Combat.MELEE_RANGE * 2.0f)
        {
            if (closest != closestResource)
            {
                closestResourceSequence?.Kill();

                closestResourceSequence = DOTween.Sequence();
                closestResourceSequence.Append(closest.ResourceTransform.DOScale(new Vector3(0.95f, 1.05f, 1.0f), 0.15f));
                closestResourceSequence.SetLoops(int.MaxValue, LoopType.Yoyo);

                // TODO : add to shared inventory
                closestResourceSequence.onStepComplete = () => {
                    if (distance < closestEnnemiDistance)
                    {
                        foreach (var loot in closest.Controller.LootByHit)
                        {
                            if (Random.Range(0, 1) < loot.probability)
                            {
                                inventory.AddResource(loot.resourceType, loot.amount);
                            }
                        }
                        closest.Controller.DecrementHit();
                    }
                };

                closestResource = closest;
            }

        }
        else
        {
            closestResourceSequence?.Kill();
            closestResourceSequence = null;
            closestResource = null;
        }

        closestResourceDistance = distance;

    }

    void UpdateEnnemiTarget()
    {
        if (ManagerEnnemis == null)
        {
            Debug.Log("No Ennemi Manager attached");
            return;
        }

        Ennemi closest = ManagerEnnemis.GetClosestEnnemi(transform.position);
        if (closest == null || closest.Controller == null || closest.Controller.IsDestroyed())
        {
            TargetMarkerObject.gameObject.SetActive(false);
            Debug.Log("No ennemi found");
            return;
        }

        closestEnnemi = closest;
       

        if (closestEnnemi != null && closestEnnemi.Combat.IsDestroyed() == false)
        {
            float distance = Vector3.Distance(transform.position, closestEnnemi.Combat.transform.position);
            
            if(distance < Combat.MELEE_RANGE * 1.5f)
            {
                if (TargetMarkerObject.TARGET != closestEnnemi.Combat.transform)
                {
                    TargetMarkerObject.FollowTarget(closest.Controller.transform, 2.0f);
                    TargetMarkerObject.gameObject.SetActive(true);
                }

                if(Time.time - last_attack_time > 1.0 / Combat.MELEE_ATTACK_SPEED)
                {
                    if(Combat.TryAttackTarget(closestEnnemi.Combat.transform))
                        last_attack_time = Time.time;
                }
            }

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

    IEnumerator LockDashForSecond(float time)
    {
        IsDashLocked = true;
        yield return new WaitForSeconds(time);
        IsDashLocked = false;
    }

    void UpdateMovements()
    {
        const float COS45 = 0.70710678f;
        const float SIN45 = 0.70710678f;

        Vector2 Input = ControlMove.ReadValue<Vector2>();
        bool IsDashing = ControlDash.IsPressed();


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

        if (IsDashing && !IsDashLocked)
        { 
            StartCoroutine(LockDashForSecond(.5f));
            DashVelocity = TowardedMove.normalized * DashSpeed;
        }

        Velocity = Vector2.Lerp(Velocity, TowardedMove, Acceleration * Time.deltaTime);
        DashVelocity = Vector2.Lerp(DashVelocity, Vector2.zero, Acceleration * Time.deltaTime);

        Vector2 current_velocity = Velocity + DashVelocity;

        CaliAnimator.SetFloat("Speed", current_velocity.magnitude);

        Vector3 movement = new Vector3(
                current_velocity.x * COS45 + current_velocity.y * SIN45,
                0,
                -current_velocity.x * SIN45 + current_velocity.y * COS45
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
            DashVelocity = Vector2.zero;
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

        HealthBar.fillAmount = Combat.HP / Combat.MAX_HP;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * transform.localScale.y * 0.1f);
    }
}
