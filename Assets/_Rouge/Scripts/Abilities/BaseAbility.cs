using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public abstract class BaseAbility : NetworkBehaviour
{
    public bool IsReady => _cooldownTimer <= 0;
    public float CooldownTimer => _cooldownTimer;
    public float Cooldown => _cooldown;
    public EAttackType AttackType => _attackType;

    public AbilityScriptable abilityScriptable;

    [SyncVar(hook = nameof(OnOwnerSetup))]
    public NetworkIdentity owner;
    public PlayerCharacter playerCharacter;

    public float DamageMultiplyer
    {
        get => _damageMultiplier;
        set => _damageMultiplier = value;
    }

    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _cooldownTimer;

    [SerializeField] protected bool _noCooldown;

    [SerializeField] protected int _maxStackCount;
    [SerializeField] protected int _stackLeft;

    [SerializeField] protected List<Transform> _abilityExecutePositions = new List<Transform>();
    [SerializeField] protected int _abilityPositionIndex = 0;

    [SerializeField] protected float _damageMultiplier = 1;

    [SerializeField] protected List<ScriptableEffect> _effects = new List<ScriptableEffect>();

    protected DamageData _damageData;


    [SerializeField] private EAttackType _attackType;

    public virtual void Awake()
    {
        transform.name += "_Ability";
    }

    public void Start()
    {

    }

    public virtual void Update()
    {
        CooldownTick(Time.deltaTime);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

    }

    public void SetOwner(NetworkIdentity newOwner)
    {
        owner = newOwner;
    }

    void OnOwnerSetup(NetworkIdentity oldNet, NetworkIdentity newNet)
    {
        FindPlayerCharacter(newNet.netId);
    }

    void FindPlayerCharacter(uint netId)
    {
        playerCharacter = PlayerCharacter.allPlayerCharacters.FirstOrDefault(pc => pc.netId == netId);

        if (playerCharacter == null)
        {
            Debug.LogError($"No player character with: {netId} id");
        }

        OnInitialized();
    }

    public virtual void OnInitialized()
    {

    }

    public virtual void PrepareExecuting(DamageData damageData = null)
    {
        if (!owner.isLocalPlayer) return;
        _damageData = damageData;
    }

    public virtual void Execute()
    {
        if (!IsReady) return;
        if (!owner.isLocalPlayer) return;
        _stackLeft--;
        RefreshCooldown();
    }

    public void AddEffectToDamageable(IDamageable pawn)
    {
        foreach (var effect in _effects)
        {
            pawn.AddEffect(effect.InitializeEffect(pawn.GetGameObject(), _damageData));
        }
    }

    public virtual void SetAbilityExecutePositions(List<Transform> newExecutePositions)
    {
        _abilityExecutePositions = newExecutePositions;
    }

    public virtual void SetAbilityExecutePositionIndex(int index = 0)
    {
        _abilityPositionIndex = index;
    }

    public virtual void CooldownTick(float deltaTime)
    {
        if (_noCooldown) return;
        if (_cooldownTimer <= 0) return;
        if (_stackLeft >= _maxStackCount) return;

        _cooldownTimer -= deltaTime;

        if (_cooldownTimer <= 0)
        {
            AddStack();
        }

    }

    void RefreshCooldown()
    {
        _cooldownTimer = _cooldown;
    }

    public virtual void AddStack()
    {
        _stackLeft++;

        if (_stackLeft >= _maxStackCount)
            _stackLeft = _maxStackCount;
    }
}
