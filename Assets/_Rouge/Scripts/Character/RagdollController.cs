using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RagdollController : MonoBehaviour
{

    [Header("Root")]
    [SerializeField] private Transform rootBone;

    [Space]
    [SerializeField] private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    [SerializeField] private List<Joint> _joints = new List<Joint>();
    [SerializeField] private List<Collider> _colliders = new List<Collider>();

    private Animator _animator;
    private CharacterController _characterController;
    private NavMeshAgent _navmeshAgent;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        _navmeshAgent = GetComponent<NavMeshAgent>();

        DisableRagdoll();
    }

    public void EnableRagdoll(DamageData damageData)
    {
        if (_animator != null) _animator.enabled = false;
        if (_characterController != null) _characterController.enabled = false;
        if (_navmeshAgent != null) _navmeshAgent.enabled = false;

        _rigidbodies.ForEach(rb => rb.isKinematic = false);
        _colliders.ForEach(c => c.enabled = true);

        ImpactBody(FindClosestRagdollBody(damageData.hitPosition), damageData.velocity);
    }

    void ImpactBody(Rigidbody body, Vector3 velocity)
    {
        if (body == null)
            return;

        body.AddForce(velocity * 8, ForceMode.Impulse);
        // Debug.LogError($"Body {body.name} addforce to {velocity}", body);
    }

    Rigidbody FindClosestRagdollBody(Vector3 position)
    {
        Rigidbody closestBody = null;
        float closestDist = float.MaxValue;

        foreach (var body in _rigidbodies)
        {
            float dist = (position - body.transform.position).sqrMagnitude;

            if (dist < closestDist)
            {
                closestDist = dist;
                closestBody = body;
            }
        }
        return closestBody;
    }

    void DisableRagdoll()
    {
        _rigidbodies.ForEach(rb => rb.isKinematic = true);
        _colliders.ForEach(c => c.enabled = false);
    }


    public void FindAll()
    {
        _rigidbodies.Clear();
        _joints.Clear();
        _colliders.Clear();

        FindRigidbodies();
        FindColliders();
        FindJoints();
    }

    void FindRigidbodies()
    {
        _rigidbodies = rootBone.GetComponentsInChildren<Rigidbody>().ToList();
    }

    void FindJoints()
    {
        _joints = rootBone.GetComponentsInChildren<Joint>().ToList();
    }

    void FindColliders()
    {
        _colliders = rootBone.GetComponentsInChildren<Collider>().ToList();
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(RagdollController)), CanEditMultipleObjects]
public class RagdollControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh"))
        {
            (target as RagdollController).FindAll();
        }
    }
}

#endif
