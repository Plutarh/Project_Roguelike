using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class MeleeDamageCollider : MonoBehaviour
{
    [SerializeField] private List<DamageCollider> damageColliders = new List<DamageCollider>();

    NetworkIdentity _owner;

    List<IDamageable> _damagedObjects = new List<IDamageable>();

    DamageData _damageData;

    public enum EMeleeColliderType
    {
        LightAttack,
        HeavyAttack
    }

    private void Awake()
    {
        DisableDamageCollider();
    }

    public void SetOwner(NetworkIdentity newOwner)
    {
        _owner = newOwner;
    }

    public void EnableDamageCollider(EMeleeColliderType type, DamageData damageData)
    {
        if (_owner == null)
        {
            Debug.LogError("Need setup owner, try to find it myself");
            _owner = transform.root.GetComponent<NetworkIdentity>();

            if (_owner == null)
                return;
        }

        _damageData = damageData;
        _damageData.whoOwner = _owner;

        var typedCollider = damageColliders.FirstOrDefault(dc => dc.type == type);

        if (typedCollider == null) return;

        typedCollider.collider.SetActive(true);
        _damagedObjects.Clear();
    }

    public void DisableDamageCollider()
    {
        damageColliders.ForEach(dc => dc.collider.SetActive(false));
        _damagedObjects.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other == null || other.transform.root == transform.root) return;



        var damageable = other.GetComponent<IDamageable>();

        if (damageable == null || damageable == (_owner.GetComponent<IDamageable>())) return;
        if (damageable.GetTeam() == _owner.GetComponent<Pawn>().GetTeam()) return;

        if (_damagedObjects.Contains(damageable) == false)
        {

            _damagedObjects.Add(damageable);
            damageable.TakeDamage(_damageData);
        }
    }
}

[System.Serializable]
public class DamageCollider
{
    public GameObject collider;
    public MeleeDamageCollider.EMeleeColliderType type;
}
