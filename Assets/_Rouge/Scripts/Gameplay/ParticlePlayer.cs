using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> _particleSystems = new List<ParticleSystem>();
    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    private Vector2 _randomAngle;

    [SerializeField]
    private Vector3 _angleToRotate;

    private void OnValidate()
    {
        FindParticles();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

    }
    private void Awake()
    {
        FindParticles();


    }

    private void Start()
    {
        ApplyRandomRotation();
    }

    void ApplyRandomRotation()
    {
        transform.localRotation *= Quaternion.AngleAxis(Random.Range(_randomAngle.x, _randomAngle.y), _angleToRotate.normalized);
    }

    void FindParticles()
    {
        if (_particleSystem == null)
            _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystems.Count == 0)
            _particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
    }

    private void LateUpdate()
    {
        CheckForDestroy();
    }

    public void Play(float delay = 0)
    {
        StartCoroutine(IEPlayParticles(delay));
    }

    IEnumerator IEPlayParticles(float delay = 0)
    {
        yield return new WaitForSecondsRealtime(delay);

        foreach (var particle in _particleSystems)
        {
            if (!particle.isPlaying)
                particle.Play();
        }
    }

    void CheckForDestroy()
    {
        if (_particleSystem.IsAlive(true) == false)
            Destroy(gameObject, 1);
    }
}
