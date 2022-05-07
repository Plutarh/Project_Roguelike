using UnityEngine;

[System.Serializable]
public class CombatAnimationClipData : AnimationClipData
{
    public float GetTimerToNextCombo
    {
        get => _timerToNextCombo;
    }

    [SerializeField] private float _timerToNextCombo = 0.5f;
}

