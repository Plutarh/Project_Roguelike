using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float GetCurrentHealth
    {
        get => _currentHealth;
    }

    public float GetMaxHealth
    {
        get => _maxHealth;
    }

    public bool IsDead
    {
        get => _isDead;
    }

    public Action OnHealthDecreased;
    public Action OnHealthIncreased;
    public Action OnDeath;

    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxHealth;
    [SerializeField] private bool _isDead;

    private void Awake() 
    {
        ResetHealth();
    }

    public void ResetHealth()
    {
        _currentHealth = _maxHealth;
    }

    public void DecreaseHealth(float _healthToDecrease)
    {
        _currentHealth -= _healthToDecrease;

        OnHealthDecreased?.Invoke();

        if(_currentHealth <= 0) 
        {
            _currentHealth = 0;
            _isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void IncreaseHealth(float _healthToIncrease)
    {
        _currentHealth += _healthToIncrease;

        if(_currentHealth > _maxHealth) _currentHealth = _maxHealth;

        OnHealthIncreased?.Invoke();
    }

    public void InstaKill()
    {
        _currentHealth = 0;
        _isDead = true;
    }
}
