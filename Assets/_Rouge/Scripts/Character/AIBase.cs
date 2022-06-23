using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class AIBase : BaseCharacter
{

    [SerializeField] private EAIState _currentState;
    [SerializeField] private Pawn _currentTarget;

    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _lastAttackTime;
    [SerializeField] private float _chasingDistance;
    [SerializeField] private float _chaseBreakDistance;


    [SerializeField] private CreatureSkinmeshData _skinmeshData;

    [SerializeField] private float _lastPatrollTime;
    [SerializeField] private float _patrollDelay;
    private float _lastIdlePatrollStateChangeTime;

    Vector3 _spawnPosition;
    Vector3 _targetMovePosition;
    float _motion;

    private RagdollController _ragdollController;
    private NavMeshAgent _navMeshAgent;


    private string _currentCombatName;

    private MeleeDamageCollider _meleeDamageCollider;

    [SyncVar]
    [SerializeField] private int _skinIndex;

    public enum EAIState
    {
        Idle,
        Chase,
        Attack,
        Patroll,
        Retreat
    }


    // [Inject]
    // public void Construct(PlayerMover player)
    // {
    //     _currentTarget = player;
    // }

    public override void Awake()
    {
        base.Awake();

        SetTeam(EPawnTeam.AI);


    }

    public override void Update()
    {
        if (Health.IsDead) return;
        base.Update();
        if (!isServer) return;

        StateMachine();
        UpdateMotionAnimation();
    }

    public override void Start()
    {
        base.Start();
        if (!isServer) return;

        FindRandomSkinIndex();
        SelectSkin();

        _spawnPosition = transform.position;

        if (Random.value > 0.5f)
            ChangeState(EAIState.Patroll);
        else
            ChangeState(EAIState.Idle);

        _currentTarget = PlayerCharacter.allPlayerCharacters[Random.Range(0, PlayerCharacter.allPlayerCharacters.Count)].PlayerMover;
    }

    public override void InitComponents()
    {
        base.InitComponents();
        _ragdollController = GetComponent<RagdollController>();


        _meleeDamageCollider = GetComponentInChildren<MeleeDamageCollider>();
        _meleeDamageCollider.SetOwner(netIdentity);

        SetupNavmesh();
    }


    void SelectSkin()
    {
        if (_skinmeshData == null || _skinmeshData.RandomSkinMesh == null)
        {
            Debug.LogError("Cannot get skin mesh", this);
            return;
        }
        if (_skinnedMeshRenderer == null)
        {
            Debug.LogError("Cannot apply skin. Skin mesh renderer NULL ref");
            return;
        }

        _skinnedMeshRenderer.sharedMesh = _skinmeshData.GetSkinMeshByIndex(_skinIndex);
    }

    void FindRandomSkinIndex()
    {
        if (!isServer) return;
        _skinIndex = Random.Range(0, _skinmeshData.Skins.Count);
    }

    void SetupNavmesh()
    {
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null) return;

        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.acceleration = _speedChangeRate;
    }

    void StopNavmesh(bool state)
    {
        if (_navMeshAgent.isOnNavMesh)
            _navMeshAgent.isStopped = state;
    }

    public void ChangeState(EAIState newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;

        switch (_currentState)
        {
            case EAIState.Idle:

                StopNavmesh(true);
                FindTarget();
                _lastIdlePatrollStateChangeTime = Time.time + Random.Range(10, 20);
                break;
            case EAIState.Chase:
                StopNavmesh(false);
                break;
            case EAIState.Attack:
                Attack();
                break;
            case EAIState.Patroll:
                _lastIdlePatrollStateChangeTime = Time.time + Random.Range(10, 20);
                StopNavmesh(false);
                break;
            case EAIState.Retreat:
                var retreatPosition = FindRandomPoint(_spawnPosition);
                if (retreatPosition == Vector3.zero)
                    retreatPosition = _spawnPosition;
                MoveToPosition(retreatPosition);
                break;
        }
    }

    void StateMachine()
    {
        if (_navMeshAgent == null) return;

        switch (_currentState)
        {
            case EAIState.Idle:
                FindTarget();
                _motion = 0;

                if (Time.time > _lastIdlePatrollStateChangeTime) ChangeState(EAIState.Patroll);
                break;
            case EAIState.Chase:
                Chasing();
                CalculateMotionAnimation();
                break;
            case EAIState.Attack:
                _motion = 0;
                Attack();
                break;
            case EAIState.Patroll:
                Patrolling();
                CalculateMotionAnimation();
                if (Time.time > _lastIdlePatrollStateChangeTime) ChangeState(EAIState.Idle);
                break;
            case EAIState.Retreat:
                Retreating();
                CalculateMotionAnimation();
                break;
        }

    }

    // TODO смотреть дистанцию до игроков или рандмоного, хз пока
    void FindTarget()
    {
        if (GetDistanceToTarget() > _chasingDistance) return;

        ChangeState(EAIState.Chase);
    }

    float GetDistanceToTarget()
    {
        float distance = 0;

        if (_currentTarget != null)
            distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        return distance;
    }

    float GetDistanceToTargetMovePosition()
    {
        float distance = 0;

        if (_currentTarget != null)
            distance = Vector3.Distance(transform.position, _targetMovePosition);

        return distance;
    }

    public void MoveToPosition(Vector3 targetPosition)
    {
        _targetMovePosition = targetPosition;
        _navMeshAgent.SetDestination(_targetMovePosition);
    }


    void UpdateMotionAnimation()
    {
        _animator.SetFloat("Motion", _motion);
    }

    void CalculateMotionAnimation()
    {
        _currentMoveSpeed = _navMeshAgent.velocity.magnitude;
        _motion = Mathf.Lerp(0, 1, _currentMoveSpeed / _moveSpeed);
    }

    void Retreating()
    {
        if (GetDistanceToTarget() < _chasingDistance)
        {
            ChangeState(EAIState.Chase);
            return;
        }

        if (GetDistanceToTargetMovePosition() < 1)
        {
            ChangeState(Random.value > 0.5f ? EAIState.Idle : EAIState.Patroll);
        }
    }

    void Patrolling()
    {
        if (GetDistanceToTarget() < _chasingDistance)
        {
            ChangeState(EAIState.Chase);
            return;
        }

        if (Time.time < _lastPatrollTime + _patrollDelay) return;

        var randomPoint = FindRandomPoint(transform.position);

        if (randomPoint == Vector3.zero) return;
        if (Vector3.Distance(_spawnPosition, randomPoint) > 15) return;

        _lastPatrollTime = Time.time;

        MoveToPosition(randomPoint);

        _patrollDelay = Vector3.Distance(transform.position, randomPoint) / _moveSpeed + Random.Range(5, 10);
    }

    Vector3 FindRandomPoint(Vector3 from)
    {
        Vector3 randomPoint = from + Random.insideUnitSphere * Random.Range(5, 30);

        NavMeshHit navmeshHit;
        bool isCorrectPath = (NavMesh.SamplePosition(randomPoint, out navmeshHit, 1f, NavMesh.AllAreas));

        if (isCorrectPath == false) randomPoint = Vector3.zero;
        else Debug.DrawLine(transform.position + Vector3.up, randomPoint + Vector3.up, Color.red, 1);

        return randomPoint;
    }

    void Chasing()
    {
        if (_navMeshAgent.isOnNavMesh && _navMeshAgent.isStopped) return;

        if (_currentTarget == null || _currentTarget.Health.IsDead)
        {
            ChangeState(EAIState.Idle);
            return;
        }

        if (_navMeshAgent.isOnNavMesh == false)
        {
            ChangeState(EAIState.Idle);
            return;
        }

        if (GetDistanceToTarget() > _chaseBreakDistance)
        {
            ChangeState(EAIState.Retreat);
            return;
        }

        if (GetDistanceToTarget() <= _attackDistance)
        {
            ChangeState(EAIState.Attack);
            return;
        }


        MoveToPosition(_currentTarget.transform.position);
        // _characterController.
    }


    bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackDelay;
    }

    float GetCombatAnimationTime()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_currentCombatName))
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        else
            return 1;
    }

    void Attack()
    {
        RotationToTarget();
        StopNavmesh(true);
        if (GetCombatAnimationTime() < 0.95f || CanAttack() == false) return;

        if (GetDistanceToTarget() > _attackDistance)
        {
            ChangeState(EAIState.Chase);
            return;
        }

        _lastAttackTime = Time.time;

        // Менять комбо херню тут
        if (Random.value > 0.5f)
        {
            _currentCombatName = "Attack";
        }
        else
        {
            _currentCombatName = "Attack0";
        }

        _animator.CrossFade(_currentCombatName, 0.15f);
        RpcAttackAnimation(_currentCombatName);
    }

    [ClientRpc(includeOwner = false)]
    void RpcAttackAnimation(string clipName)
    {
        _animator.CrossFade(clipName, 0.15f);
    }

    void RotationToTarget()
    {
        if (_currentTarget == null) return;

        transform.rotation = Quaternion.Lerp(transform.rotation
            , Quaternion.LookRotation((_currentTarget.transform.position - transform.position).normalized, Vector3.up)
            , Time.deltaTime * 2.2f);
    }

    public void AnimLightAttack()
    {
        if (_meleeDamageCollider == null)
        {
            Debug.LogError($"{name} cannot use damage colliders, null ref", this);
            return;
        }

        DamageData lightDamageData = new DamageData();

        lightDamageData.combatValue = Random.Range(1, 3);

        _meleeDamageCollider.EnableDamageCollider(MeleeDamageCollider.EMeleeColliderType.LightAttack, lightDamageData);
    }

    public void AnimHeavyAttack()
    {
        if (_meleeDamageCollider == null)
        {
            Debug.LogError($"{name} cannot use damage colliders, null ref", this);
            return;
        }

        DamageData heavyDamageData = new DamageData();

        heavyDamageData.combatValue = Random.Range(5, 10);

        _meleeDamageCollider.EnableDamageCollider(MeleeDamageCollider.EMeleeColliderType.HeavyAttack, heavyDamageData);
    }

    public void AnimAttackEnd()
    {
        if (_meleeDamageCollider == null)
        {
            Debug.LogError($"{name} cannot use damage colliders, null ref", this);
            return;
        }

        _meleeDamageCollider.DisableDamageCollider();
    }


    public override void Death()
    {
        base.Death();
    }

    // [Command(requiresAuthority = false)]
    // public override void CmdDeath()
    // {
    //     base.CmdDeath();
    // }

    [ClientRpc]
    public override void RpcDeath()
    {
        base.RpcDeath();
        _ragdollController.EnableRagdoll();
    }


}
