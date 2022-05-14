using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    public int PrimaryAttackIndex => _currentPrimaryAttackIndex;

    public List<BaseAbility> AllAbilities => _allAbilities;

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

    private List<BaseAbility> _allAbilities = new List<BaseAbility>();


    [Header("Combat")]
    [SerializeField] private float _comboAttackTimeout = 0.8f;
    [SerializeField] private float _lastPrimaryAttackTime = 0;
    [SerializeField] private string _currentCombatName;

    Transform _abilitiesParent;

    [SerializeField] protected int _currentPrimaryAttackIndex = 0;

    public Action OnAbilitiesInitialized;

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
    }

    void InitializeAbilities()
    {
        if (_abilitiesParent == null)
        {
            _abilitiesParent = new GameObject("Abilities").transform;
            _abilitiesParent.SetParent(transform);
            _abilitiesParent.SetAsFirstSibling();
        }

        _primaryAbility = CreateAbility(_primaryAbilityData);
        _allAbilities.Add(_primaryAbility);

        _secondaryAbility = CreateAbility(_secondaryAbilityData);
        _allAbilities.Add(_secondaryAbility);

        _utilityAbility = CreateAbility(_utilityAbilityData);
        _allAbilities.Add(_utilityAbility);

        _ultimateAbility = CreateAbility(_ultimateAbilityData);
        _allAbilities.Add(_ultimateAbility);

        OnAbilitiesInitialized?.Invoke();
    }

    BaseAbility CreateAbility(AbilityScriptable abilityScriptable)
    {
        var createdAbility = Instantiate(abilityScriptable.abilityComponent, _abilitiesParent);
        createdAbility.transform.localPosition = Vector3.zero;
        createdAbility.SetOwner(_player);
        return createdAbility;
    }

    public DamageData CreateDamageData(float damageMultiplyer)
    {
        DamageData damageData = new DamageData();
        damageData.whoOwner = _player;
        damageData.combatValue = _player.Characteristics.GetTypedValue(ECharacteristicType.Damage) * damageMultiplyer;
        return damageData;
    }

    public virtual void AnimPreparePrimaryAttack_1()
    {
        _primaryAbility.SetAbilityExecutePositionIndex(_currentPrimaryAttackIndex);
        _primaryAbility.PrepareExecuting(CreateDamageData(_primaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartPrimaryAttack_1()
    {
        _primaryAbility.Execute();

    }

    public virtual void AnimPrepareSecondaryAttack_1()
    {
        _secondaryAbility.PrepareExecuting(CreateDamageData(_secondaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartSecondaryAttack_1()
    {
        _secondaryAbility.Execute();
    }

    public virtual void AnimPrepareUtilitySkill_1()
    {
        _utilityAbility.PrepareExecuting(CreateDamageData(_utilityAbility.DamageMultiplyer));
    }

    public virtual void AnimStartUtilitySkill_1()
    {
        _utilityAbility.Execute();

    }

    public virtual void AnimPrepareUltimateAttack_1()
    {
        _ultimateAbility.PrepareExecuting(CreateDamageData(_secondaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartUltimateAttack_1()
    {
        _ultimateAbility.Execute();
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

        if (GetAttackLayerAnimationTime() < 0.8f) return;
        _player.SetBattleState();
        Debug.Log($"Current state - {_currentCombatName} Combat time " + GetAttackLayerAnimationTime());
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

        _player.OnAttackAnimation?.Invoke();
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
        if (_primaryAbility.IsReady == false) return;
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
        if (_secondaryAbility.IsReady == false) return;
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
        if (_utilityAbility.IsReady == false) return;
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
        if (_ultimateAbility.IsReady == false) return;
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

        if (nextCombatAnimationClip.IsStopMovement)
            StartCoroutine(IEWaitToUnblockMovement(nextCombatAnimationClip.StopMovementTime));

        _player.Animator.CrossFade(nextCombatAnimationClip.AnimationName, nextCombatAnimationClip.CrossFadeTime, nextCombatAnimationClip.IsAnimationFullbody ? 0 : 1);
        _currentCombatName = nextCombatAnimationClip.AnimationName;

        Debug.Log("<color=green>Cross Fade to </color>" + nextCombatAnimationClip.AnimationName);
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
}


