using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public abstract class ScriptableEffect : ScriptableObject
{
    public float duration;
    public bool unlimitedDuration = false;
    public bool isDurationRefreshed = true;
    public bool isStackable = false;
    public bool BulletBuff;

    public Sprite effectMiniIcon;
    public string effectName;
    public string effectDescription;

    public Color uiBackgroundColor;

    public abstract TimedEffect InitializeEffect(NetworkIdentity target, CombatData damageData);
    // public abstract TimedEffect InitializeEffect(GameObject target, HealData healData);
}
