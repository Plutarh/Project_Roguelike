using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private DamageData damageData;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _delayToGravity = 1.5f;

    private Rigidbody _body;
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GravityTimer();
    }

    void GravityTimer()
    {
        _delayToGravity -= Time.deltaTime;

        if (_delayToGravity <= 0)
        {
            _body.useGravity = true;
        }
    }

    public void SetMoveDirection(Vector3 direction)
    {
        _body.AddForce(direction.normalized * _moveSpeed, ForceMode.VelocityChange);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        IDamageable damageable;
        if (other.transform.TryGetComponent<IDamageable>(out damageable))
        {
            damageable.TakeDamage(damageData);
        }
    }
}
