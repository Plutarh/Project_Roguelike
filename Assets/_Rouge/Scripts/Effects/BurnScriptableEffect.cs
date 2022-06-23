using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEffects/BurnEffect")]
public class BurnScriptableEffect : ScriptableEffect
{
    public float timerToHit;
    public float damagePercent;

    public ParticleSystem burnFX;

    public override TimedEffect InitializeEffect(NetworkIdentity obj, CombatData combatData)
    {
        return new BurnTimedEffect(this, obj, combatData);
    }

    public override TimedEffect InitializeEffect(NetworkIdentity obj)
    {
        return new BurnTimedEffect(this, obj);
    }
}
