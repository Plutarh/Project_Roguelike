using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIAbilitiesPanel : MonoBehaviour
{
    [SerializeField] private UIAbilityFrame _abilityFramePrefab;
    [SerializeField] private List<UIAbilityFrame> _abilitiesFrames = new List<UIAbilityFrame>();

    [SerializeField] private Transform _abilityFramesParent;

    PlayerCharacter _playerCharacter;

    // [Inject]
    // public void Construct(PlayerCharacter playerChar)
    // {
    //     _playerCharacter = playerChar;

    //     _playerCharacter.OnAbilitiesInitialized += CreateAbilities;
    // }


    private void Start()
    {
        CreateAbilities();
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        RefreshAbilityFrames();
    }

    void RefreshAbilityFrames()
    {
        /*
        for (int i = 0; i < _playerCharacter.AllAbilities.Count; i++)
        {
            var charAbility = _playerCharacter.AllAbilities[i] >;

            float cooldown = charAbility.CooldownTimer / charAbility.Cooldown;
            float cooldownLerp = Mathf.Lerp(0, 1, cooldown);

            _abilitiesFrames[i].SetCooldownProgress(cooldownLerp);
        }*/
    }

    public void CreateAbilities()
    {
        if (_playerCharacter == null)
        {
            Debug.LogError("Cannot find player character reference");
            return;
        }

        _abilityFramePrefab.gameObject.SetActive(false);

        foreach (var ability in _playerCharacter.AllAbilities)
        {
            // var createdAbility = Instantiate(_abilityFramePrefab, _abilityFramesParent);

            // createdAbility.SetAbilityIcon(ability.abilityScriptable.abilityData.icon);

            // createdAbility.gameObject.SetActive(true);
            // _abilitiesFrames.Add(createdAbility);
        }

        RefreshAbilityFrames();
    }

    // private void OnDestroy()
    // {
    //     _playerCharacter.OnAbilitiesInitialized -= CreateAbilities;
    // }
}
