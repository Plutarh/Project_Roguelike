using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour, IDamageable
{
    public Health GetHealth
    {
        get => _health;
    }

    [SerializeField] private Health _health;

    public virtual void Awake()
    {
        _health = GetComponent<Health>();
    }

    public virtual void Start()
    {

    }


    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void TakeDamage(DamageData damageData)
    {
        if (_health == null) Debug.LogError("Health component NULLED");
        if (damageData == null) Debug.LogError("Damage data NULLED");

        _health.DecreaseHealth(damageData.damage);

        if (_health.IsDead) Death();
    }

    public virtual void Death()
    {

    }
}
