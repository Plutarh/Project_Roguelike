using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "Character Animations", menuName = "Character Animation", order = 51)]
public class CharacterAnimationData : ScriptableObject
{
    public RuntimeAnimatorController GetAnimatorController
    {
        get => _animatorController;
    }

    public List<CombatAnimationData> GetCombatAnimations
    {
        get => _animationDatas;
    }



    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private List<CombatAnimationData> _animationDatas = new List<CombatAnimationData>();


    public void Refresh()
    {
        _animationDatas.ForEach(ad => ad.animationClipDatas.ForEach(acd => acd.Refresh()));
    }

    public CombatAnimationData GetCombatAnimationsByType(EAttackType type)
    {
        return _animationDatas.FirstOrDefault(ad => ad.attackType == type);
    }

    public AnimationClipData GetAnimationClip(EAttackType type, ref int clipIndex)
    {
        var foundedCombatData = _animationDatas.FirstOrDefault(ad => ad.attackType == type);

        AnimationClipData foundedClip = new AnimationClipData();

        if (foundedCombatData == null)
        {
            Debug.LogError($"Cannot find combo data by type {type}");
        }
        else
        {
            if (clipIndex > foundedCombatData.animationClipDatas.Count - 1)
            {
                clipIndex = 0;
            }
            foundedClip = foundedCombatData.animationClipDatas[clipIndex];
            // }
            // Debug.LogError($"Index out of range by combo clip. Current index {clipIndex} but total clips count is {foundedCombatData.animationClipDatas.Count}");
            //     foundedClip = foundedCombatData.animationClipDatas[0];
            // }
            // else
            // {
            //     foundedClip = foundedCombatData.animationClipDatas[clipIndex];
            // }
        }

        return foundedClip;
    }
}

[System.Serializable]
public class CombatAnimationData
{
    public EAttackType attackType;
    public List<AnimationClipData> animationClipDatas = new List<AnimationClipData>();
}

[System.Serializable]
public class AnimationClipData
{
    public string GetAnimationName
    {
        get
        {
            if (string.IsNullOrEmpty(_animationClipName))
                Refresh();
            return _animationClipName;
        }
    }

    public float GetTimerToNextCombo
    {
        get => _timerToNextCombo;
    }

    public bool IsStopMovement
    {
        get => _stopMovement;
    }

    public bool IsAllowRootRotation
    {
        get => _allowRootRotation;
    }

    public float GetCrossFadeTime
    {
        get => _crossFade;
    }

    [SerializeField] private string _animationClipName;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private bool _stopMovement;
    [SerializeField] private bool _allowRootRotation;
    [SerializeField] private float _timerToNextCombo = 0.5f;
    [SerializeField] private float _crossFade = 0.1f;

    public void Refresh()
    {
        _animationClipName = animationClip.name;
    }
}

[CustomEditor(typeof(CharacterAnimationData))]
public class CharacterAnimationDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fill Names"))
        {
            (target as CharacterAnimationData).Refresh();
        }
    }
}

[System.Serializable]
public enum EAttackType
{
    Primary,
    Secondary,
    Utility,
    Ultimate
}

