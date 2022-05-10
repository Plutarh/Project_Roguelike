using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    public bool IsReady
    {
        get => _cooldownTimer <= 0;
    }

    public BaseCharacter owner;

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

    public virtual void Awake()
    {

    }

    public virtual void Update()
    {
        CooldownTick(Time.deltaTime);
    }

    public void SetOwner(BaseCharacter newOwner)
    {
        owner = newOwner;
    }

    public virtual void PrepareExecuting(DamageData damageData = null)
    {
        _damageData = damageData;
    }

    public virtual void Execute()
    {
        if (!IsReady) return;
        _stackLeft--;
        RefreshCooldown();
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
