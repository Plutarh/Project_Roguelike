using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class TimedEffect
{
    internal float currentDuration;
    public bool unlimitedDuration = false;
    internal float totalDuration;
    public ScriptableEffect Effect { get; }
    protected readonly NetworkIdentity targetGO;
    protected readonly NetworkIdentity whoUsed;

    public bool IsFinished;
    public bool IsPaused;

    public CombatData combatData;

    public TimedEffect(ScriptableEffect effect, NetworkIdentity targetObj, CombatData combat)
    {
        Effect = effect;
        targetGO = targetObj;
        whoUsed = combat.whoOwner;
        combatData = combat;

    }

    public virtual void Tick(float delta)
    {
        if (Effect.unlimitedDuration)
            return;
        if (totalDuration <= 0)
            totalDuration = currentDuration;

        currentDuration -= delta;

        if (currentDuration <= 0)
        {
            End();
            IsFinished = true;
            IsPaused = false;
        }
    }

    public void Activate()
    {
        if (currentDuration <= 0)
        {
            ApplyEffect();
            currentDuration += Effect.duration;
        }

        if (Effect.isDurationRefreshed && currentDuration > 0)
        {
            currentDuration = Effect.duration;
        }
    }

    public virtual void Reactivate()
    {

    }

    protected abstract void ApplyEffect();
    public abstract void End();
}
