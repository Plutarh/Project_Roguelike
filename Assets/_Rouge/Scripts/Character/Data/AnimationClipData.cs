using UnityEngine;

[System.Serializable]
public class AnimationClipData
{


    public string AnimationName
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

    public bool IsAnimationFullbody
    {
        get => _animationFullbody;
    }

    public float CrossFadeTime
    {
        get => _crossFade;
    }

    public float StopMovementTime
    {
        get => _stopMovementTime;
    }

    [SerializeField] private string _animationClipName;
    [SerializeField] private AnimationClip animationClip;
    [SerializeField] private bool _stopMovement;
    [SerializeField] private float _stopMovementTime;
    [SerializeField] private bool _animationFullbody;

    [SerializeField] private float _crossFade = 0.1f;

    public void Refresh()
    {
        _animationClipName = animationClip.name;
    }
}

