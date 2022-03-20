using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        if (_animator != null)
        {
            _animator.enabled = false;
            _characterController.enabled = false;

            _rigidbodies.ForEach(rb => rb.isKinematic = false);
            _colliders.ForEach(c => c.enabled = true);
        }
    }

    void DisableRagdoll()
    {
        _rigidbodies.ForEach(rb => rb.isKinematic = true);
        _colliders.ForEach(c => c.enabled = false);
    }

    public void RagdollAtPart()
    {

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
