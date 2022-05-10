using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnTimedEffect : TimedEffect
{
    IDamageable _target;
    GameObject _whoUse;
    BurnScriptableEffect _burnScriptableEffect;
    ParticleSystem _burningFX;

    float _timeToHit;

    public BurnTimedEffect(ScriptableEffect buff, GameObject targetObj, CombatData combatData) : base(buff, targetObj, combatData)
    {
        _target = targetObj.GetComponent<IDamageable>();
        _whoUse = combatData.whoOwner.gameObject;
    }

    protected override void ApplyEffect()
    {
        if (_target == null) return;
        _burnScriptableEffect = (BurnScriptableEffect)Effect;



        CreateFX();
        currentDuration = totalDuration;
        _timeToHit = _burnScriptableEffect.timerToHit;
    }

    public override void End()
    {
        if (_burningFX == null)
        {
            Debug.LogError("Cannot destroy nulled fx");
            return;
        }
        GameObject.Destroy(_burningFX.gameObject);
    }

    public override void Tick(float delta)
    {
        base.Tick(delta);

        if (IsFinished) return;

        _timeToHit -= delta;
        if (_timeToHit <= 0)
        {
            DamageData damageData = new DamageData();

            damageData.combatValue = (_burnScriptableEffect.damagePercent * combatData.combatValue) / 100f;
            damageData.whoOwner = combatData.whoOwner;

            _target.TakeDamage(damageData);

            _timeToHit = _burnScriptableEffect.timerToHit;
        }
    }

    void CreateFX()
    {
        if (_burnScriptableEffect.burnFX == null) return;

        _burningFX = GameObject.Instantiate(_burnScriptableEffect.burnFX, _target.GetGameObject().transform.position, Quaternion.AngleAxis(90, Vector3.right));
        _burningFX.transform.SetParent(_target.GetGameObject().transform);

        _burningFX.transform.localPosition = Vector3.zero;
    }
}
