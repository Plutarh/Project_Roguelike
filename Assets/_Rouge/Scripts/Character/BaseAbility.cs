using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    public bool IsReady
    {
        get
        {
            return _cooldownTimer <= 0 && _stackLeft > 0;
        }

    }
    public BaseCharacter owner;

    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _cooldownTimer;

    [SerializeField] protected bool _noCooldown;

    [SerializeField] protected int _maxStackCount;
    [SerializeField] protected int _stackLeft;

    [SerializeField] protected List<Transform> _abilityExecutePositions = new List<Transform>();
    [SerializeField] protected int _abilityPositionIndex = 0;

    public virtual void Update()
    {
        CooldownTick(Time.deltaTime);
    }

    public void SetOwner(BaseCharacter newOwner)
    {
        owner = newOwner;
    }

    public virtual void PrepareExecuting()
    {

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
