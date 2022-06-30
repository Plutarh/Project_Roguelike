using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    private Coroutine _jumpCoroutine;
    private float _jumpLastTime;
    [SerializeField] private float _jumpCooldown = 2;

    [Space]
    [Header("Attack FX")]
    [SerializeField] private ParticlePlayer _lightAttackFX;
    [SerializeField] private Transform _lightAttackFxPosition;

    [SerializeField] private ParticlePlayer _heavyAttackFX;
    [SerializeField] private Transform _heavyAttackFxPosition;


    public enum EAIState
    {
        Idle,
        Chase,
        Attack,
        Patroll,
        Retreat,
        Jump
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
        VisualiseWaypoints();
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("On start client");
        GlobalEvents.OnEnemySpawned?.Invoke(this);
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
        if (_navMeshAgent == null)
        {
            Debug.LogError($"Cannot find navmesh component on {transform.name}", this);
            return;
        }

        _navMeshAgent.speed = _moveSpeed;
        _navMeshAgent.acceleration = _speedChangeRate;
        _navMeshAgent.autoTraverseOffMeshLink = false;
    }

    void StopNavmesh(bool state)
    {
        if (_navMeshAgent.isOnNavMesh)
            _navMeshAgent.isStopped = state;
    }

    public void ChangeState(EAIState newState)
    {
        if (_currentState == newState) return;
        Debug.Log("Change state to " + newState);
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
            case EAIState.Jump:

                Jump();
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

                if (Time.time > _lastIdlePatrollStateChangeTime)
                    ChangeState(EAIState.Patroll);

                break;
            case EAIState.Chase:
                Chasing();

                break;
            case EAIState.Attack:
                _motion = 0;
                Attack();
                break;
            case EAIState.Patroll:
                Patrolling();

                if (Time.time > _lastIdlePatrollStateChangeTime)
                    ChangeState(EAIState.Idle);
                break;
            case EAIState.Retreat:
                Retreating();

                break;
            case EAIState.Jump:
                break;
        }


        if (_currentState != EAIState.Idle)
            CalculateMotionAnimation();
    }

    // TODO смотреть дистанцию до игроков или рандмоного, хз пока
    void FindTarget()
    {
        if (_currentTarget == null)
            _currentTarget = PlayerCharacter.allPlayerCharacters[Random.Range(0, PlayerCharacter.allPlayerCharacters.Count)].PlayerMover;
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

    public LineRenderer pathRenderer;
    public List<Vector3> waypints = new List<Vector3>();
    void VisualiseWaypoints()
    {
        return;
        waypints.Clear();
        var waypoints = _navMeshAgent.path;
        if (waypoints.corners.Length <= 0) return;
        pathRenderer.positionCount = waypoints.corners.Length;
        for (int i = 0; i < waypoints.corners.Length; i++)
        {
            var point = waypoints.corners[i];
            pathRenderer.SetPosition(i, point + Vector3.up * .2f);
            waypints.Add(point);
        }
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

        if (_navMeshAgent.isOnOffMeshLink && Time.time - _jumpLastTime >= _jumpCooldown)
        {
            ChangeState(EAIState.Jump);
            return;
        }
    }



    public override void GroundCheck()
    {
        if (_navMeshAgent == null || _navMeshAgent.isStopped) return;
        _animator.SetBool("Land", _navMeshAgent.isOnNavMesh);
    }

    void Jump()
    {
        if (_jumpCoroutine != null)
        {
            ChangeState(EAIState.Chase);
            return;
        }

        _jumpCoroutine = StartCoroutine(IEJumping());
    }

    IEnumerator IEJumping()
    {
        _animator.CrossFade("Start Jump", 0.1f);
        // Ждем подготовку анимации 
        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(IEMovingOnNavmeshLink(1.5f, 0.6f));

        _animator.CrossFade("Landing", 0.05f);
        _jumpCoroutine = null;
        _jumpLastTime = Time.time;

        // Ждем анимацию для приземления
        yield return new WaitForSecondsRealtime(0.3f);
        _navMeshAgent.CompleteOffMeshLink();
        ChangeState(EAIState.Chase);
    }

    IEnumerator IEMovingOnNavmeshLink(float height, float duration)
    {
        OffMeshLinkData data = _navMeshAgent.currentOffMeshLinkData;
        Vector3 startPosition = _navMeshAgent.transform.position;
        Vector3 endPosition = data.endPos + Vector3.up * _navMeshAgent.baseOffset;

        float normalizedTime = 0;

        while (normalizedTime < 1f)
        {
            float yOffset = height * 4 * (normalizedTime - normalizedTime * normalizedTime);
            _navMeshAgent.transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime) + yOffset * Vector3.up;
            _navMeshAgent.transform.rotation = Quaternion.Lerp(_navMeshAgent.transform.rotation, Quaternion.LookRotation((endPosition - data.startPos).normalized, Vector3.up), Time.deltaTime * 6);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }


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

        //Рандом для зеркальной
        bool mirrorAnimation = Random.value > 0.5f;

        // Рандом между легкой и тяжелой атакой
        if (Random.value > 0.5f)
            _currentCombatName = "Light_Attack" + (mirrorAnimation ? "_M" : string.Empty);
        else
            _currentCombatName = "Heavy_Attack" + (mirrorAnimation ? "_M" : string.Empty);

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

        var fx = Instantiate(_lightAttackFX, _lightAttackFxPosition.transform);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity * Quaternion.AngleAxis(90, Vector3.forward);
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

        var fx = Instantiate(_heavyAttackFX, _heavyAttackFxPosition.transform);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;

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


    public override void TakeDamage(DamageData damageData)
    {
        base.TakeDamage(damageData);
        CmdPlayHitReaction(damageData);
    }

    [Command(requiresAuthority = false)]
    void CmdPlayHitReaction(DamageData damageData)
    {
        RpcPlayHitReaction(damageData);
    }

    [ClientRpc(includeOwner = false)]
    void RpcPlayHitReaction(DamageData damageData)
    {
        if (Health.CurrentHealth <= 0 || Health.IsDead) return;

        string hitReactionAnimationName = string.Empty;

        if (Health.GetDamagePercent(damageData.combatValue) >= 20)
            hitReactionAnimationName = "Medium Hit";
        else
            hitReactionAnimationName = "Light Hit";

        // Для зеркальной анимации
        if (Random.value > 0.5)
            hitReactionAnimationName += "_M";

        _animator.CrossFade(hitReactionAnimationName, 0.1f, 1);
    }

    public override void Death(DamageData damageData)
    {
        base.Death(damageData);
    }

    private void OnDrawGizmos()
    {
        foreach (var item in waypints)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(item, 0.2f);
        }
    }

    [ClientRpc]
    public override void RpcDeath(DamageData damageData)
    {
        StopAllCoroutines();
        base.RpcDeath(damageData);
        _ragdollController.EnableRagdoll(damageData);
    }


}
