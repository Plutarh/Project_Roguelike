using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBase : BaseCharacter
{
    private RagdollController ragdollController;

    [SerializeField] private EAIState currentState;
    [SerializeField] private Pawn currentTarget;

    private NavMeshAgent navMeshAgent;

    [SerializeField] private float _attackDistance;

    public enum EAIState
    {
        Idle,
        Chase,
        Attack,
        Patroll
    }

    public override void Awake()
    {
        base.Awake();

        ragdollController = GetComponent<RagdollController>();
        SetupNavmesh();
    }

    public override void Update()
    {
        base.Update();
        StateMachine();
    }

    void SetupNavmesh()
    {
        navMeshAgent.speed = _moveSpeed;
        navMeshAgent.acceleration = _speedChangeRate;
    }

    void FindTarget()
    {
        currentTarget = FindObjectOfType<Player>();
    }

    float GetDistanceToTarget()
    {
        float distance = 0;

        if (currentTarget != null)
            distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        return distance;
    }


    public void ChangeState(EAIState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
        {
            case EAIState.Idle:
                FindTarget();
                break;
            case EAIState.Chase:
                break;
            case EAIState.Attack:
                break;
            case EAIState.Patroll:
                break;
        }
    }

    void StateMachine()
    {
        switch (currentState)
        {
            case EAIState.Idle:
                break;
            case EAIState.Chase:
                Chasing();
                break;
            case EAIState.Attack:
                Attack();
                break;
            case EAIState.Patroll:
                break;
        }

        _currentMoveSpeed = navMeshAgent.velocity.magnitude;

        float motion = Mathf.Lerp(0, 1, _currentMoveSpeed / _moveSpeed);
        _animator.SetFloat("Motion", motion);
    }

    void Chasing()
    {
        if (currentTarget == null || currentTarget.GetHealth.IsDead)
        {
            ChangeState(EAIState.Idle);
            return;
        }

        if (navMeshAgent.isOnNavMesh == false)
        {
            ChangeState(EAIState.Idle);
            return;
        }

        navMeshAgent.SetDestination(currentTarget.transform.position);
        // _characterController.
    }

    void Attack()
    {
        if (GetDistanceToTarget() > _attackDistance)
        {
            ChangeState(EAIState.Idle);
            return;
        }
    }

    public override void Death()
    {
        base.Death();
        ragdollController.EnableRagdoll();
    }
}
