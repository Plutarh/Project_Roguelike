using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Characteristics))]
[RequireComponent(typeof(Health))]
public class Pawn : NetworkBehaviour, IDamageable
{
    public Characteristics Characteristics => _characteristics;
    public Health Health => _health;

    public Transform HeadBone
    {
        get
        {
            if (_headBone == null)
                return transform;
            else
                return _headBone;
        }
    }

    [SerializeField] private Characteristics _characteristics;
    [SerializeField] private Health _health;
    [SerializeField] private EPawnTeam _myTeam;
    [SerializeField] private Transform _headBone;

    Dictionary<ScriptableEffect, TimedEffect> _timedEffects = new Dictionary<ScriptableEffect, TimedEffect>();

    public List<Effects> DebugAllEffects = new List<Effects>();

    public Action OnDeath;

    public virtual void Awake()
    {
        _characteristics = GetComponent<Characteristics>();
        _health = GetComponent<Health>();
    }

    public virtual void Start()
    {

    }


    public virtual void Update()
    {
        EffectsTimeTick();

        DebugAllEffects.Clear();
        foreach (KeyValuePair<ScriptableEffect, TimedEffect> item in _timedEffects)
        {
            Effects newEffect = new Effects();
            newEffect.effectName = item.Key.effectName;
            newEffect.effectDuration = item.Value.currentDuration;
            DebugAllEffects.Add(newEffect);
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual Vector3 GetAimDirection()
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetAimPoint()
    {
        return Vector3.zero;
    }

    public void SetTeam(EPawnTeam newTeam)
    {
        _myTeam = newTeam;
    }

    public EPawnTeam GetTeam()
    {
        return _myTeam;
    }

    public virtual void TakeDamage(DamageData damageData)
    {
        ClientTakeDamage(damageData);
    }

    [Client]
    public void ClientTakeDamage(DamageData damageData)
    {
        CmdTakeDamage(damageData);
    }

    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(DamageData damageData)
    {
        if (Health.IsDead) return;

        if (_characteristics == null)
        {
            Debug.LogError("Health component NULLED", this);
            return;
        }

        if (damageData == null)
        {
            Debug.LogError("Damage data NULLED", this);
            return;
        }

        Health.DecreaseHealth(damageData);

        if (Health.IsDead) Death();
    }

    public virtual void Death()
    {
        CmdDeath();
    }

    [Command(requiresAuthority = false)]
    public virtual void CmdDeath()
    {
        RpcDeath();
    }

    [ClientRpc]
    public virtual void RpcDeath()
    {
        OnDeath?.Invoke();
    }

    public void AddEffect(TimedEffect effect)
    {
        // Если уже содержит, то обновим таймер
        if (_timedEffects.ContainsKey(effect.Effect))
        {
            _timedEffects[effect.Effect].Activate();
        }
        else
        {
            _timedEffects.Add(effect.Effect, effect);
            effect.Activate();

            // TODO ad UI event to create in UI panel effect ICON;
        }
    }

    public void RemoveEffect(TimedEffect effect)
    {
        if (_timedEffects.ContainsKey(effect.Effect) == false)
            return;

        _timedEffects[effect.Effect].End();
        _timedEffects.Remove(effect.Effect);
    }

    public void EffectsTimeTick()
    {
        if (_timedEffects.Count <= 0) return;

        for (int i = 0; i < _timedEffects.Values.Count; i++)
        {
            var item = _timedEffects.ElementAt(i);
            var effect = item.Value;

            if (Health.IsDead)
            {
                effect.IsFinished = true;
                effect.End();
            }

            effect.Tick(Time.deltaTime);

            if (item.Key.unlimitedDuration)
                continue;

            //TODO if need add ui progression;

            if (effect.IsFinished)
            {
                //TODO event to remove ui progressionns;
                _timedEffects.Remove(effect.Effect);
            }
        }
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public NetworkIdentity GetNetworkIdentity()
    {
        return netIdentity;
    }
}

[System.Serializable]
public class Effects
{
    public string effectName;
    public float effectDuration;
}
