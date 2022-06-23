using System;
using Mirror;
using UnityEngine;


public class Health : NetworkBehaviour
{
    public float CurrentHealth
    {
        get => _currentHealth;
    }

    public float MaxHealth
    {
        get => _maxHealth;
    }

    public bool IsDead
    {
        get => _isDead;
    }

    public Action<DamageData> OnHealthDecreased;
    public Action OnHealthIncreased;

    [SyncVar]
    [SerializeField] private float _currentHealth;
    [SyncVar]
    [SerializeField] private float _maxHealth;
    [SyncVar]
    [SerializeField] private bool _isDead;

    private void OnValidate()
    {
        _currentHealth = _maxHealth;
    }

    private void Awake()
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }


    [Command(requiresAuthority = false)]
    void CmdDecreaseHealth(DamageData damageData)
    {
        Debug.LogError("call server decrease health " + damageData.combatValue);
        ServerDecreaseHealth(damageData);
    }

    [Server]
    void ServerDecreaseHealth(DamageData damageData)
    {
        _currentHealth -= damageData.combatValue;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
        }

        OnHealthDecreased?.Invoke(damageData);
    }

    public void DecreaseHealth(DamageData damageData)
    {
        if (damageData.combatValue == 0) return;

        _currentHealth -= damageData.combatValue;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _isDead = true;
        }

        OnHealthDecreased?.Invoke(damageData);
        // CmdDecreaseHealth(damageData);
    }

    public void IncreaseHealth(float _healthToIncrease)
    {
        if (_healthToIncrease == 0) return;

        _currentHealth += _healthToIncrease;

        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;

        OnHealthIncreased?.Invoke();
    }

    public float GetHealth01()
    {
        if (_maxHealth == 0 || _currentHealth == 0)
            return 0;

        float value = Mathf.Clamp(_currentHealth / _maxHealth, 0f, 1f);
        return value;
    }


}
