using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
}

[System.Serializable]
public class CombatAnimationData
{
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

    public bool IsStopMovement
    {
        get => _stopMovement;
    }

    public bool IsAllowRootRotation
    {
        get => _allowRootRotation;
    }

    [SerializeField] private string _animationClipName;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private bool _stopMovement;
    [SerializeField] private bool _allowRootRotation;

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

