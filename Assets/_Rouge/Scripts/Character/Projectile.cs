using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private DamageData damageData;
    [SerializeField] private float _moveSpeed;

    [SerializeField] private float _delayToGravity = 1.5f;
    [SerializeField] private float _lifetime = 3;

    [Header("FX")]
    [SerializeField] private GameObject onHitFX;

    private Rigidbody _body;
    private void Awake()
    {
        _body = GetComponent<Rigidbody>();


        // DEBUG
        damageData = new DamageData();
        damageData.damage = 50;
    }

    private void Update()
    {
        GravityTimer();
        LifeTimer();
    }

    void LifeTimer()
    {
        if (_lifetime <= 0)
            Destroy(gameObject);
        else
            _lifetime -= Time.deltaTime;
    }

    void GravityTimer()
    {

        if (_delayToGravity <= 0)
            _body.useGravity = true;
        else
            _delayToGravity -= Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        _body.AddForce(direction.normalized * _moveSpeed, ForceMode.VelocityChange);
    }


    void Hit(IDamageable damageable)
    {
        damageable.TakeDamage(damageData);

        if (onHitFX != null)
            Instantiate(onHitFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        Debug.Log($"Hit with {other.transform.name}");
        IDamageable damageable;
        if (other.transform.TryGetComponent<IDamageable>(out damageable))
        {
            Hit(damageable);
        }
    }
}
