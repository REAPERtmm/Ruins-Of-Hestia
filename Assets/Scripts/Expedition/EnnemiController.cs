
using System.Collections.Generic;
using UnityEngine;

public enum EnnemiState : int
{
    Idle = 0,
    Roaming = 1,
    Focus = 2,
}

public class EnnemiController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Animator Animation;
    [SerializeField] Transform Model;
    [SerializeField] CapsuleCollider Collider;
    [SerializeField] CharacterController CharController;
    [SerializeField] CombatController Combat;
    [SerializeField] List<Transform> Targets;

    [Header("Parameters")]
    [SerializeField] float RoamingMovementSpeed;
    [SerializeField] float FocusMovementSpeed;

    [SerializeField] float RoamingMaxDuration;
    [SerializeField] float IdleMaxDuration;
    [SerializeField] float RoamingMinDuration;
    [SerializeField] float IdleMinDuration;

    [SerializeField] float DetectionRadiusIdle;
    [SerializeField] float DetectionRadiusRoaming;
    [SerializeField] float FocusMinDistance;

    public Vector2Int InitRoom;

    bool CollideWithEdgeOfMap;
    Vector3 MovementThisFrame;
    EnnemiState CurrentState;
    float TimeStateStarted = 0;
    float MaxTime = 0;

    // Idle Variables

    // Roaming Variables
    Vector2 RoamingDirection;

    // Focus Variables
    Transform Target;

    void Move(Vector2 Direction)
    {
        const float COS45 = 0.70710678f;
        const float SIN45 = 0.70710678f;

        float UsedSpeed = CurrentState == EnnemiState.Focus ? FocusMovementSpeed : RoamingMovementSpeed;

        Vector3 movement = new Vector3(
                Direction.x * COS45 + Direction.y * SIN45,
                0,
                -Direction.x * SIN45 + Direction.y * COS45
                ) * UsedSpeed * Time.deltaTime;

        MovementThisFrame = movement;
        CharController.Move(MovementThisFrame);

        Ray below = new Ray(transform.position, Vector3.down);
        var hits = Physics.RaycastAll(below, transform.localScale.y * 0.1f);
        CollideWithEdgeOfMap = true;
        foreach (var hit in hits)
        {
            if (hit.collider.tag == "Ground")
            {
                CollideWithEdgeOfMap = false;
                transform.position = hit.point;
                break;
            }
        }
        if (CollideWithEdgeOfMap)
        {
            CharController.Move(-MovementThisFrame);
        }
        else if (Direction.x != 0.0f || Direction.y != 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
        }
    }

    void TransitionStateTo(EnnemiState state)
    {
        TimeStateStarted = Time.time;
        CurrentState = state;
    }

    void TransitionToIdle()
    {
        TransitionStateTo(EnnemiState.Idle);
        MaxTime = Random.Range(IdleMinDuration, IdleMaxDuration);
    }

    void TransitionToRoaming()
    {
        TransitionStateTo(EnnemiState.Roaming);
        float angle = Random.Range(0, Mathf.PI * 2.0f);
        RoamingDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        MaxTime = Random.Range(RoamingMinDuration, RoamingMaxDuration);
    }

    void TransitionToFocus(Transform target)
    {
        TransitionStateTo(EnnemiState.Focus);
        Target = target;
    }

    void UpdateIdle()
    {
        foreach (var target in Targets)
        {
            float DistanceToTarget = (transform.position - target.position).magnitude;

            if (DistanceToTarget < DetectionRadiusIdle)
            {
                TransitionToFocus(target);
                return;
            }
        }

        if (Time.time - TimeStateStarted > MaxTime)
        {
            TransitionToRoaming();
            return;
        }

    }

    void UpdateRoaming()
    {
        Move(RoamingDirection);

        if (CollideWithEdgeOfMap)
        {
            TransitionToIdle();
            return;
        }

        foreach (var target in Targets)
        {
            float DistanceToTarget = (transform.position - target.position).magnitude;

            if (DistanceToTarget < DetectionRadiusRoaming)
            {
                TransitionToFocus(target);
                return;
            }
        }

        if (Time.time - TimeStateStarted > MaxTime)
        {
            TransitionToIdle();
            return;
        }

    }

    void UpdateFocus()
    {
        Vector3 TargetDirection = Target.position - transform.position;
        Vector2 NormalizedTargetDirection = new Vector2(TargetDirection.x, TargetDirection.z);
        NormalizedTargetDirection = NormalizedTargetDirection.normalized;
        Move(NormalizedTargetDirection);

        float DistanceToTarget = (transform.position - Target.position).magnitude;

        if (DistanceToTarget > FocusMinDistance)
        {
            TransitionToIdle();
            return;
        }
    }

    private void Start()
    {
        Targets.Add(PlayerController.INSTANCE.transform);
        // TODO : Add Iris

        PlayerController.INSTANCE.Combat.RegisterTarget(transform);

        for (int i = 0; i < Targets.Count; i++)
        {
            Combat.RegisterTarget(Targets[i]);
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            default:
            case EnnemiState.Idle: UpdateIdle(); break;
            case EnnemiState.Roaming: UpdateRoaming(); break;
            case EnnemiState.Focus: UpdateFocus(); break;
        }

        Animation.SetInteger("State", (int)CurrentState);
    }

    private void OnDrawGizmos()
    {
        if (MapGeneration.INSTANCE != null && MapGeneration.INSTANCE.EnnemiAI == false) return;
        Color IDLE_COLOR = Color.white;
        Color ROAMING_COLOR = Color.aquamarine;
        Color FOCUS_COLOR = Color.red;

        switch (CurrentState)
        {
            default:
            case EnnemiState.Idle: Gizmos.color = IDLE_COLOR; break;
            case EnnemiState.Roaming: Gizmos.color = ROAMING_COLOR; break;
            case EnnemiState.Focus: Gizmos.color = FOCUS_COLOR; break;
        }
        Gizmos.DrawSphere(transform.position + Vector3.up * 2f, 0.2f);

        Gizmos.color = IDLE_COLOR;
        Gizmos.DrawWireSphere(transform.position, DetectionRadiusIdle);

        Gizmos.color = ROAMING_COLOR;
        Gizmos.DrawWireSphere(transform.position, DetectionRadiusRoaming);

        Gizmos.color = FOCUS_COLOR;
        Gizmos.DrawWireSphere(transform.position, FocusMinDistance);
    }
}
