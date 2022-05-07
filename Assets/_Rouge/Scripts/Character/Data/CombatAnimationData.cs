using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatAnimationData
{
    [HideInInspector] public string attackTypeName;
    public EAttackType attackType;
    public List<CombatAnimationClipData> animationClipDatas = new List<CombatAnimationClipData>();
}

