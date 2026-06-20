
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] CombatController Combat;
    [SerializeField] List<CombatController> Targets;

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
    [SerializeField] EnnemiState CurrentState;
    float TimeStateStarted = 0;
    float MaxTime = 0;

    // Idle Variables

    // Roaming Variables
    Vector2 RoamingDirection;

    // Focus Variables
    [SerializeField] Transform Target;

    void Move(Vector2 Direction)
    {
        float UsedSpeed = CurrentState == EnnemiState.Focus ? FocusMovementSpeed : RoamingMovementSpeed;

        Vector3 movement = new Vector3(
                Direction.x, 0, Direction.y
                ) * UsedSpeed * Time.deltaTime;

        Animation.SetFloat("Speed", UsedSpeed);

        Vector3 old_position = transform.position;
        Vector3 next_position = transform.position + movement;

        Ray below = new Ray(next_position, Vector3.down);
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
            transform.position = old_position;
        }
        else if (Direction.x != 0.0f || Direction.y != 0.0f)
        {
            transform.position = next_position;
            transform.rotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
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
        Animation.SetBool("IsAgressive", false);
    }

    void TransitionToRoaming()
    {
        TransitionStateTo(EnnemiState.Roaming);
        float angle = Random.Range(0, Mathf.PI * 2.0f);
        RoamingDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        MaxTime = Random.Range(RoamingMinDuration, RoamingMaxDuration);
        Animation.SetBool("IsAgressive", false);
    }

    void TransitionToFocus(Transform target)
    {
        TransitionStateTo(EnnemiState.Focus);
        Target = target;
        Animation.SetBool("IsAgressive", true);
    }

    void UpdateIdle()
    {
        Animation.SetFloat("Speed", 0.0f);
        foreach (var target in Targets)
        {
            if (target.IsDestroyed() || target.ALIVE == false) continue;
            float DistanceToTarget = (transform.position - target.transform.position).magnitude;

            if (DistanceToTarget < DetectionRadiusIdle)
            {
                TransitionToFocus(target.transform);
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
            if (target.IsDestroyed() || target.ALIVE == false) continue;
            float DistanceToTarget = (transform.position - target.transform.position).magnitude;

            if (DistanceToTarget < DetectionRadiusRoaming)
            {
                TransitionToFocus(target.transform);
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
        CombatController TargetCombat = Target?.GetComponent<CombatController>();
        bool IS_TARGET_DEAD_OR_INANIMATE = TargetCombat == null || TargetCombat.ALIVE == false;
        if (Target == null || Target.IsDestroyed() || IS_TARGET_DEAD_OR_INANIMATE)
        {
            TransitionToIdle();
            return;
        }
        float DistanceToTarget = (transform.position - Target.position).magnitude;

        if(Combat.MELEE_RANGE * 0.9f > DistanceToTarget || Combat.IsAttacking)
        {
            Combat.AttackClosest();
            Animation.SetFloat("Speed", 0.0f);
        }
        else
        {
            Vector3 TargetDirection = Target.position - transform.position;
            Vector2 NormalizedTargetDirection = new Vector2(TargetDirection.x, TargetDirection.z);
            NormalizedTargetDirection = NormalizedTargetDirection.normalized;
            Move(NormalizedTargetDirection);

        }

        if (DistanceToTarget > FocusMinDistance)
        {
            TransitionToIdle();
            return;
        }
    }

    private void Start()
    {
        Targets.Add(PlayerController.INSTANCE.GetComponent<CombatController>());
        // TODO : Add Iris

        PlayerController.INSTANCE.Combat.RegisterTarget(GetComponent<CombatController>());

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
