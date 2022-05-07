using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Characteristics))]
[RequireComponent(typeof(Health))]
public class Pawn : MonoBehaviour, IDamageable
{
    public Characteristics Characteristics
    {
        get => _characteristics;
    }

    public Health Health
    {
        get => _health;
    }

    public Transform HeadBone
    {
        get
        {
            if (_headBone == null)
                return transform;
            else
                return _headBone;
        }
    }

    [SerializeField] private Characteristics _characteristics;
    [SerializeField] private Health _health;
    [SerializeField] private EPawnTeam _myTeam;
    [SerializeField] private Transform _headBone;

    public virtual void Awake()
    {
        _characteristics = GetComponent<Characteristics>();
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

    public virtual Vector3 GetAimDirection()
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetAimPoint()
    {
        return Vector3.zero;
    }

    public void SetTeam(EPawnTeam newTeam)
    {
        _myTeam = newTeam;
    }

    public EPawnTeam GetTeam()
    {
        return _myTeam;
    }

    public virtual void TakeDamage(DamageData damageData)
    {
        if (_characteristics == null)
        {
            Debug.LogError("Health component NULLED", this);
            return;
        }

        if (damageData == null)
        {
            Debug.LogError("Damage data NULLED", this);
            return;
        }

        Health.DecreaseHealth(damageData);

        if (Health.IsDead) Death();
    }

    public virtual void Death()
    {

    }
}
