using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "Character Animations", menuName = "Character/Player/Attack Combos", order = 51)]
public class CharacterAnimationData : ScriptableObject
{
    public string GetCharacterName
    {
        get => _characterName;
    }

    public RuntimeAnimatorController GetAnimatorController
    {
        get => _animatorController;
    }

    public List<CombatAnimationData> GetCombatAnimations
    {
        get => _combatAnimationDatas;
    }


    [SerializeField] private string _characterName;
    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private List<CombatAnimationData> _combatAnimationDatas = new List<CombatAnimationData>();


    public void Refresh()
    {
        foreach (var ad in GetCombatAnimations)
        {
            ad.attackTypeName = ad.attackType.ToString();
            foreach (var acd in ad.animationClipDatas)
            {
                acd.Refresh();
            }
        }

        //_animationDatas.ForEach(ad => ad.animationClipDatas.ForEach(acd => acd.Refresh()));
    }

    public CombatAnimationData GetCombatAnimationsByType(EAttackType type)
    {
        return GetCombatAnimations.FirstOrDefault(ad => ad.attackType == type);
    }

    public CombatAnimationClipData GetCombatAnimationClip(EAttackType type, ref int clipIndex)
    {
        var foundedCombatData = GetCombatAnimations.FirstOrDefault(ad => ad.attackType == type);

        CombatAnimationClipData foundedClip = new CombatAnimationClipData();

        if (foundedCombatData == null)
        {
            Debug.LogError($"Cannot find combo data by type {type}");
        }
        else
        {
            if (clipIndex > foundedCombatData.animationClipDatas.Count - 1)
                clipIndex = 0;

            foundedClip = foundedCombatData.animationClipDatas[clipIndex];
        }

        return foundedClip;
    }
}
#if UNITY_EDITOR
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
#endif

[System.Serializable]
public enum EAttackType
{
    Primary,
    Secondary,
    Utility,
    Ultimate
}

