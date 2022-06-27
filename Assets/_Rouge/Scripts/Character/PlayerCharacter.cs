using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    public int PrimaryAttackIndex => _currentPrimaryAttackIndex;
    public List<BaseAbility> AllAbilities => _allAbilities;
    public Transform rootBone;
    public PlayerMover PlayerMover => _player;

    protected PlayerMover _player;

    [Header("Other")]
    public CharacterAnimationData characterAnimationData;

    [Header("Abilities Data")]
    [SerializeField] private List<AbilityScriptable> _allAbilitiesData = new List<AbilityScriptable>();

    [Header("Runtime initialized abilities")]

    [SerializeField] protected BaseAbility _primaryAbility;

    [SerializeField] protected BaseAbility _secondaryAbility;

    [SerializeField] protected BaseAbility _utilityAbility;

    [SerializeField] protected BaseAbility _ultimateAbility;

    [SerializeField] private List<BaseAbility> _allAbilities = new List<BaseAbility>();

    [Header("Combat")]
    [SerializeField] private float _comboAttackTimeout = 0.8f;
    [SerializeField] private float _lastPrimaryAttackTime = 0;
    [SerializeField] private string _currentCombatName;

    [SerializeField] private Transform _abilitiesParent;

    [SerializeField] protected int _currentPrimaryAttackIndex = 0;

    public static List<PlayerCharacter> allPlayerCharacters = new List<PlayerCharacter>();


    public bool _abilitiesInitialized;

    public Action OnAbilitiesInited;

    public virtual void Awake()
    {
        allPlayerCharacters.Add(this);
    }

    public virtual void Start()
    {
        if (_player == null) _player = GetComponent<PlayerMover>();
    }

    public void SetPlayer(PlayerMover player)
    {
        _player = player;
    }

    public virtual void InitializeLocalCoreComponents()
    {
        _player.InputService.OnAttackButtonClicked += OnAttackButtonClicked;

        ResetPrimaryAttack();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdCreateAbilities();

        NetNaming();
    }

    void NetNaming()
    {
        string netName = isServer ? "_Host" : "_Client";
        transform.name = $"Player_{netId}_{netName}";
    }

    public virtual void Update()
    {
        if (!isLocalPlayer) return;

        NetNaming();
        PrimaryAttackResetTimer();
    }

    [Command]
    void CmdCreateAbilities()
    {

        foreach (var abilityData in _allAbilitiesData)
        {
            var createdAbility = CreateAbility(abilityData);
            NetworkServer.Spawn(createdAbility.gameObject);
            createdAbility.netIdentity.AssignClientAuthority(netIdentity.connectionToClient);
            InitializeAbility(createdAbility.netIdentity);

        }
        OnAbilitiesInitialized();
    }


    [ClientRpc]
    public void InitializeAbility(NetworkIdentity abilityNet)
    {
        if (abilityNet == null)
        {
            Debug.LogError("Ability networkdId nulled");
            return;
        }

        var ability = abilityNet.gameObject.GetComponent<BaseAbility>();

        switch (ability.AttackType)
        {
            case EAttackType.Primary:

                _primaryAbility = ability;
                break;
            case EAttackType.Secondary:

                _secondaryAbility = ability;
                break;
            case EAttackType.Utility:

                _utilityAbility = ability;
                break;
            case EAttackType.Ultimate:

                _ultimateAbility = ability;
                break;
        }

        _allAbilities.Add(ability);
    }

    [ClientRpc]
    public virtual void OnAbilitiesInitialized()
    {
        _abilitiesInitialized = true;
        OnAbilitiesInited?.Invoke();
    }


    BaseAbility CreateAbility(AbilityScriptable abilityScriptable)
    {
        var createdAbility = Instantiate(abilityScriptable.abilityComponent, null);
        createdAbility.transform.position = Vector3.zero;

        if (isServer)
            NetworkServer.Spawn(createdAbility.gameObject);

        if (_player == null)
        {
            Debug.LogError($"Player null - {transform.name}", this);
        }

        createdAbility.SetOwner(netIdentity);
        return createdAbility;
    }

    public DamageData CreateDamageData(float damageMultiplyer)
    {
        DamageData damageData = new DamageData();
        damageData.whoOwner = netIdentity;
        damageData.combatValue = _player.Characteristics.GetTypedValue(ECharacteristicType.Damage) * damageMultiplyer;
        return damageData;
    }


    public virtual void AnimPreparePrimaryAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_primaryAbility == null)
        {
            Debug.LogError("Primary ability not initalized", this);
            return;
        }
        _primaryAbility.PrepareExecuting(CreateDamageData(_primaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartPrimaryAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_primaryAbility == null)
        {
            Debug.LogError("Primary ability not initalized", this);
            return;
        }
        _primaryAbility.Execute();
    }

    public virtual void AnimPrepareSecondaryAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_secondaryAbility == null)
        {
            Debug.LogError("Secondary ability not initalized", this);
            return;
        }
        _secondaryAbility.PrepareExecuting(CreateDamageData(_secondaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartSecondaryAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_secondaryAbility == null)
        {
            Debug.LogError("Secondary ability not initalized", this);
            return;
        }
        _secondaryAbility.Execute();
    }

    public virtual void AnimPrepareUtilitySkill_1()
    {
        if (!isLocalPlayer) return;
        if (_utilityAbility == null)
        {
            Debug.LogError("Utility ability not initalized", this);
            return;
        }
        _utilityAbility.PrepareExecuting(CreateDamageData(_utilityAbility.DamageMultiplyer));
    }

    public virtual void AnimStartUtilitySkill_1()
    {
        if (!isLocalPlayer) return;
        if (_utilityAbility == null)
        {
            Debug.LogError("Utility ability not initalized", this);
            return;
        }
        _utilityAbility.Execute();
    }

    public virtual void AnimPrepareUltimateAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_ultimateAbility == null)
        {
            Debug.LogError("Ultimate ability not initalized", this);
            return;
        }
        _ultimateAbility.PrepareExecuting(CreateDamageData(_secondaryAbility.DamageMultiplyer));
    }

    public virtual void AnimStartUltimateAttack_1()
    {
        if (!isLocalPlayer) return;
        if (_ultimateAbility == null)
        {
            Debug.LogError("Ultimate ability not initalized", this);
            return;
        }
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
        if (!_player.isLocalPlayer) return;

        if (GetAttackLayerAnimationTime() < 0.8f) return;
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

        NetworkAnimationData animation = new NetworkAnimationData();

        animation.animationName = nextCombatAnimationClip.AnimationName;
        animation.crossFade = nextCombatAnimationClip.CrossFadeTime;
        animation.fullbody = nextCombatAnimationClip.IsAnimationFullbody ? 0 : 1;


        CmdCombatAnimation(animation);

        _player.Animator.CrossFade(nextCombatAnimationClip.AnimationName, nextCombatAnimationClip.CrossFadeTime, nextCombatAnimationClip.IsAnimationFullbody ? 0 : 1);

        _currentCombatName = nextCombatAnimationClip.AnimationName;
    }

    struct NetworkAnimationData
    {
        public string animationName;
        public float crossFade;
        public int fullbody;
    }

    [Command]
    void CmdCombatAnimation(NetworkAnimationData networkAnimation)
    {
        RpcCombatAnimation(networkAnimation);
    }

    [ClientRpc(includeOwner = false)]
    void RpcCombatAnimation(NetworkAnimationData networkAnimation)
    {
        _player.Animator.CrossFade(networkAnimation.animationName, networkAnimation.crossFade, networkAnimation.fullbody);
    }

    private void OnDestroy()
    {
        allPlayerCharacters.Remove(this);

        if (isLocalPlayer)
        {
            _player.InputService.OnAttackButtonClicked -= OnAttackButtonClicked;
        }




    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        DestroyPlayerObjects();
    }

    public void DestroyPlayerObjects()
    {
        for (int i = 0; i < AllAbilities.Count; i++)
        {
            var ability = AllAbilities[i];
            if (ability == null || ability.gameObject == null) continue;
            Destroy(ability.gameObject);
        }
    }


}

[System.Serializable]
public class AbilityData
{
    public string name;
    public string description;
    public Sprite icon;
}


