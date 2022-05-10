using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HitEffects/BurnEffect")]
public class BurnScriptableEffect : ScriptableEffect
{
    public float timerToHit;
    public float damagePercent;

    public ParticleSystem burnFX;

    public override TimedEffect InitializeEffect(GameObject obj, CombatData combatData)
    {
        return new BurnTimedEffect(this, obj, combatData);
    }
}
