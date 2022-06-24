using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAbilitiesPanel : MonoBehaviour
{
    [SerializeField] private UIAbilityFrame _abilityFramePrefab;
    [SerializeField] private List<UIAbilityFrame> _abilitiesFrames = new List<UIAbilityFrame>();

    [SerializeField] private Transform _abilityFramesParent;

    PlayerCharacter _playerCharacter;
    bool _isInitialized;

    private void Awake()
    {
        GlobalEvents.OnLocalPlayerInitialized += Initialize;
    }

    public void Initialize(PlayerMover playerMover)
    {
        _playerCharacter = playerMover.GetComponent<PlayerCharacter>();
        _playerCharacter.OnAbilitiesInited += CreateAbilities;
        _isInitialized = true;
    }

    private void LateUpdate()
    {
        if (!_isInitialized) return;
        RefreshAbilityFrames();
    }

    void RefreshAbilityFrames()
    {
        for (int i = 0; i < _playerCharacter.AllAbilities.Count; i++)
        {
            var charAbility = _playerCharacter.AllAbilities[i];

            float cooldown = charAbility.CooldownTimer / charAbility.Cooldown;
            float cooldownLerp = Mathf.Lerp(0, 1, cooldown);

            _abilitiesFrames[i].SetCooldownProgress(cooldownLerp);
        }
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
            var createdAbility = Instantiate(_abilityFramePrefab, _abilityFramesParent);

            createdAbility.SetAbilityIcon(ability.abilityScriptable.abilityData.icon);

            createdAbility.gameObject.SetActive(true);
            _abilitiesFrames.Add(createdAbility);
        }

        RefreshAbilityFrames();
    }

    private void OnDestroy()
    {
        GlobalEvents.OnLocalPlayerInitialized -= Initialize;
        _playerCharacter.OnAbilitiesInited -= CreateAbilities;
    }
}
