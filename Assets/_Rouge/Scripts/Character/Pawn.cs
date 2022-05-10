using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Characteristics))]
[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour, IDamageable
{
    public Characteristics Characteristics
    {
        get => _characteristics;
    }

    public Health Health
    {
        get => _health;
    }

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

    }

    public void AddEffect(TimedEffect effect)
    {

        // Если уже содержит, то обновим таймер
        if (_timedEffects.ContainsKey(effect.Effect))
        {
            // if (effect.Effect.BulletBuff)
            // {
            //     currentEffects[newEffect.Effect].End();
            //     currentEffects.Remove(newEffect.Effect);
            //     currentEffects.Add(newEffect.Effect, newEffect);
            // }
            // else
            _timedEffects[effect.Effect].Activate();
        }
        else
        {
            _timedEffects.Add(effect.Effect, effect);
            effect.Activate();
            // if (!effect.Effect.BulletBuff)
            // {
            //     effect.Activate();
            // }
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
}
