using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitVisualizer : MonoBehaviour
{
    SkinnedMeshRenderer _skinnedMeshRenderer;
    MeshRenderer _meshRenderer;
    Pawn _pawn;
    Animator _animator;
    RuntimeAnimatorController _runtimeAnimatorController;

    const string LIGHT_HIT_REACTION_CLIP_NAME = "Light Hit Reaction";
    const string MEDIUM_HIT_REACTION_CLIP_NAME = "Medium Hit Reaction";

    [SerializeField] private bool _canPlayAnimation;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (_skinnedMeshRenderer == null)
            _meshRenderer = GetComponentInChildren<MeshRenderer>();

        _pawn = transform.root.gameObject.GetComponent<Pawn>();

        if (_pawn == null)
        {
            Debug.LogError("No pawn for dissolver");
            return;
        }

        _animator = GetComponent<Animator>();
        _runtimeAnimatorController = _animator.runtimeAnimatorController;

        _pawn.Health.OnHealthDecreased += VisualizeHit;

        _canPlayAnimation = IsAnimationHitReactionContains();
    }

    void VisualizeHit(DamageData damageData)
    {
        StartCoroutine(IEVisualizating());
        PlayAnimationHitReaction(damageData);
    }


    void PlayAnimationHitReaction(DamageData damageData)
    {
        if (!_canPlayAnimation) return;
        if (_pawn.Health.CurrentHealth <= 0 || _pawn.Health.IsDead) return;

        string hitReactionAnimationName = string.Empty;

        // Если урон выше 20% от общего хп, то покажем среднию импакт анимацию 
        if (_pawn.Health.GetDamagePercent(damageData.combatValue) >= 20)
            hitReactionAnimationName = LIGHT_HIT_REACTION_CLIP_NAME;
        else
            hitReactionAnimationName = MEDIUM_HIT_REACTION_CLIP_NAME;

        // Для зеркальной анимации
        if (Random.value > 0.5)
            hitReactionAnimationName += "_M";

        _animator.CrossFade(hitReactionAnimationName, 0.1f, 1);
    }

    // Проверяем есть ли у нас хит реакшены, если есть оба, то челик сможет их проиграть, если нет, то будем игнорировать
    bool IsAnimationHitReactionContains()
    {
        bool ligthHitAnimation = false;
        bool mediumHitAnimation = false;

        foreach (var clip in _runtimeAnimatorController.animationClips)
        {
            if (clip.name.ToLower().Contains(LIGHT_HIT_REACTION_CLIP_NAME.ToLower()))
                ligthHitAnimation = true;
            else if (clip.name.ToLower().Contains(MEDIUM_HIT_REACTION_CLIP_NAME.ToLower()))
                mediumHitAnimation = true;
        }
        return ligthHitAnimation && mediumHitAnimation;
    }

    IEnumerator IEVisualizating()
    {
        MaterialPropertyBlock hit = new MaterialPropertyBlock();

        Color baseColor = Color.cyan;

        if (_skinnedMeshRenderer != null)
            baseColor = _skinnedMeshRenderer.sharedMaterial.GetColor("_BaseColor");
        else if (_meshRenderer != null)
            baseColor = _meshRenderer.sharedMaterial.GetColor("_BaseColor");

        float colorMultiplier = 2.2f;

        float timeIn = 0.08f;
        float timerIn = timeIn;
        Color visualColor = baseColor;

        while (timerIn > 0)
        {
            timerIn -= Time.deltaTime;
            visualColor = Color.Lerp(baseColor * colorMultiplier, baseColor, timerIn / timeIn);
            hit.SetColor("_BaseColor", visualColor);

            if (_skinnedMeshRenderer != null)
                _skinnedMeshRenderer.SetPropertyBlock(hit);
            else if (_meshRenderer != null)
                _meshRenderer.SetPropertyBlock(hit);

            yield return new WaitForEndOfFrame();
        }

        float timeOut = 0.1f;
        float timerOut = timeOut;

        while (timerOut > 0)
        {
            timerOut -= Time.deltaTime;
            visualColor = Color.Lerp(baseColor, baseColor * colorMultiplier, timerOut / timeOut);
            hit.SetColor("_BaseColor", visualColor);

            if (_skinnedMeshRenderer != null)
                _skinnedMeshRenderer.SetPropertyBlock(hit);
            else if (_meshRenderer != null)
                _meshRenderer.SetPropertyBlock(hit);

            yield return new WaitForEndOfFrame();
        }


    }

    private void OnDestroy()
    {
        _pawn.Health.OnHealthDecreased -= VisualizeHit;
    }
}
