using System.Collections;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int PrimaryAttackIndex
    {
        get => _currentPrimaryAttackIndex;
    }


    protected Player _player;

    [Header("Other")]
    public CharacterAnimationData characterAnimationData;


    [Header("Abilities Data")]
    [SerializeField] protected AbilityScriptable _primaryAbilityData;
    [SerializeField] protected AbilityScriptable _secondaryAbilityData;
    [SerializeField] protected AbilityScriptable _utilityAbilityData;
    [SerializeField] protected AbilityScriptable _ultimateAbilityData;

    [Header("Runtime initialized abilities")]
    [SerializeField] protected BaseAbility _primaryAbility;
    [SerializeField] protected BaseAbility _secondaryAbility;
    [SerializeField] protected BaseAbility _utilityAbility;
    [SerializeField] protected BaseAbility _ultimateAbility;


    [Header("Combat")]
    [SerializeField] private float _comboAttackTimeout = 0.8f;
    [SerializeField] private float _lastPrimaryAttackTime = 0;
    [SerializeField] private string _currentCombatName;

    Transform _abilitiesParent;

    [SerializeField] protected int _currentPrimaryAttackIndex = 0;

    public virtual void Awake()
    {
        _player = GetComponent<Player>();
        InitializeAbilities();

        InputEvents.OnAttackButtonClicked += OnAttackButtonClicked;

        ResetPrimaryAttack();
    }

    public virtual void Update()
    {
        PrimaryAttackResetTimer();
        TryToPrimaryAttack();
    }

    void InitializeAbilities()
    {
        if (_abilitiesParent == null)
        {
            _abilitiesParent = new GameObject("Abilities").transform;
            _abilitiesParent.SetParent(transform);
            _abilitiesParent.SetAsFirstSibling();
        }

        _primaryAbility = Instantiate(_primaryAbilityData.abilityComponent, _abilitiesParent);
        _primaryAbility.transform.localPosition = Vector3.zero;
        _primaryAbility.SetOwner(_player);

        _utilityAbility = Instantiate(_utilityAbilityData.abilityComponent, _abilitiesParent);
        _utilityAbility.transform.localPosition = Vector3.zero;
        _utilityAbility.SetOwner(_player);
    }

    public virtual void AnimPreparePrimaryAttack_1()
    {
        _primaryAbility.SetAbilityExecutePositionIndex(_currentPrimaryAttackIndex);
        _primaryAbility.PrepareExecuting();
    }

    public virtual void AnimStartPrimaryAttack_1()
    {
        _primaryAbility.Execute();
        _player.OnAttackAnimation?.Invoke();
    }

    public virtual void AnimPrepareSecondaryAttack_1()
    {

    }

    public virtual void AnimStartSecondaryAttack_1()
    {

    }
    public virtual void AnimPrepareUtilitySkill_1()
    {
        _utilityAbility.PrepareExecuting();
    }

    public virtual void AnimStartUtilitySkill_1()
    {
        _utilityAbility.Execute();
        _player.OnAttackAnimation?.Invoke();
    }

    public virtual void AnimPrepareUltimateAttack_1()
    {

    }

    public virtual void AnimStartUltimateAttack_1()
    {

    }



    IEnumerator IEWaitToUnblockMovement(float waitTime)
    {
        if (waitTime <= 0) yield break;

        yield return new WaitForSecondsRealtime(waitTime);
        _player.BlockMovement(false);
    }


    public void ResetPrimaryAttack()
    {
        _currentPrimaryAttackIndex = -1;
    }




    void OnAttackButtonClicked(EAttackType type)
    {
        // FastRotateToCameraForward();
        _player.SetBattleState();

        switch (type)
        {
            case EAttackType.Primary:
                TryToPrimaryAttack();
                break;
            case EAttackType.Secondary:
                TryToSecondaryAttack();
                break;
            case EAttackType.Utility:
                TryToUtility();
                break;
            case EAttackType.Ultimate:
                TryToUltimate();
                break;
        }
    }

    float GetAttackLayerAnimationTime()
    {
        if (_player.Animator.GetCurrentAnimatorStateInfo(1).IsName(_currentCombatName))
            return _player.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime;
        else if (_player.Animator.GetCurrentAnimatorStateInfo(0).IsName(_currentCombatName))
            return _player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        else
            return 1;
    }

    void PrimaryAttackResetTimer()
    {
        if (Time.time - _lastPrimaryAttackTime > _comboAttackTimeout)
        {
            ResetPrimaryAttack();
        }
    }

    void TryToPrimaryAttack()
    {
        if (_player.InputService.GetFire() == false) return;
        PrimaryAttack();
    }

    void PrimaryAttack()
    {
        _player.InputService.ResetFire();

        if (_currentPrimaryAttackIndex < 0)
        {
            _currentPrimaryAttackIndex = 0;
        }
        else
        {
            if (GetAttackLayerAnimationTime() > 0.65f)
                _currentPrimaryAttackIndex++;
            else
                return;
        }

        _lastPrimaryAttackTime = Time.time;

        PlayCombatAnimation(EAttackType.Primary, ref _currentPrimaryAttackIndex);
    }

    void TryToSecondaryAttack()
    {
        if (_player.InputService.GetSecondaryFire() == false) return;
        SecondaryAttack();
    }

    void SecondaryAttack()
    {
        _player.InputService.ResetSecondaryFire();

        int tmpIndex = 0;
        PlayCombatAnimation(EAttackType.Secondary, ref tmpIndex);
    }

    void TryToUtility()
    {
        if (_player.InputService.GetUtility() == false) return;
        UtilitySkill();
    }

    void UtilitySkill()
    {
        _player.InputService.ResetUtility();

        int tmpIndex = 0;
        PlayCombatAnimation(EAttackType.Utility, ref tmpIndex);
    }

    void TryToUltimate()
    {
        if (_player.InputService.GetUltimate() == false) return;
        UltimateAttack();
    }

    void UltimateAttack()
    {
        _player.InputService.ResetUltimate();

        int tmpIndex = 0;
        PlayCombatAnimation(EAttackType.Ultimate, ref tmpIndex);
    }


    void PlayCombatAnimation(EAttackType attackType, ref int attackIndex)
    {
        CombatAnimationClipData nextCombatAnimationClip = new CombatAnimationClipData();

        nextCombatAnimationClip = characterAnimationData.GetCombatAnimationClip(attackType, ref attackIndex);

        _comboAttackTimeout = nextCombatAnimationClip.GetTimerToNextCombo;

        _player.BlockMovement(nextCombatAnimationClip.IsStopMovement);
        StartCoroutine(IEWaitToUnblockMovement(nextCombatAnimationClip.StopMovementTime));

        _player.Animator.CrossFade(nextCombatAnimationClip.AnimationName, nextCombatAnimationClip.CrossFadeTime, nextCombatAnimationClip.IsAnimationFullbody ? 0 : 1);
        _currentCombatName = nextCombatAnimationClip.AnimationName;

        Debug.LogError("Cross Fade to " + nextCombatAnimationClip.AnimationName);
    }

    private void OnDestroy()
    {
        InputEvents.OnAttackButtonClicked -= OnAttackButtonClicked;
    }
}

[System.Serializable]
public class AbilityData
{
    public string name;
    public string description;
    public Sprite icon;
    public float cooldown;
}

[System.Serializable]
public class AbilityModifedData : AbilityData
{
    public float cooldownTimer;
    public bool IsReady
    {
        get => cooldownTimer <= 0;
    }

    public void Tick(float deltaTime)
    {
        if (cooldownTimer < 0) return;

        cooldownTimer -= deltaTime;

        if (cooldownTimer < 0)
            cooldownTimer = cooldown;
    }
}

public class BaseAction
{

}
