using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireMageDash : BaseAbility
{
    [SerializeField] private ParticleSystem _trailFxPrefab;
    [SerializeField] private ParticleSystem _createdTrailFx;
    [SerializeField] private float _executeTime = 2;
    [SerializeField] private float _dashForce = 10;

    [Header("Ghost Settings")]
    private List<SkinnedMeshRenderer> _skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    [SerializeField] private Material _ghostTrailMaterial;
    [SerializeField] private int _ghostCount = 6;
    [SerializeField] private float _ghostLifeTime = 0.6f;

    public override void Awake()
    {
        base.Awake();
        _skinnedMeshRenderers = transform.root.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

    }

    public override void Update()
    {
        base.Update();
    }

    void CreateGhostMesh()
    {
        foreach (var skin in _skinnedMeshRenderers)
        {
            Mesh bakedMesh = new Mesh();
            skin.BakeMesh(bakedMesh);

            GameObject ghost = new GameObject($"{transform.root.name}_Fire_Ghost");

            var ghostMesh = ghost.AddComponent<SkinnedMeshRenderer>();

            ghostMesh.sharedMesh = bakedMesh;
            ghostMesh.material = _ghostTrailMaterial;

            ghost.transform.position = skin.transform.position;
            ghost.transform.rotation = skin.transform.rotation;

            Destroy(ghost, _ghostLifeTime);
        }
    }

    public override void PrepareExecuting(DamageData damageData)
    {
        base.PrepareExecuting(damageData);

    }

    public override void AddStack()
    {
        base.AddStack();
    }

    public override void Execute()
    {
        // if (!IsReady) return;
        base.Execute();


        PlayTrailFx();
        StartCoroutine(IEDash());
    }

    void PlayTrailFx()
    {
        if (_trailFxPrefab == null) return;

        _createdTrailFx = Instantiate(_trailFxPrefab, transform.root.transform);
        _createdTrailFx.transform.localPosition = Vector3.zero;
        _createdTrailFx.Play();
    }

    IEnumerator IEDash()
    {

        float time = _executeTime;
        owner.BlockMovement(true);

        float timeForGhost = time / _ghostCount;
        float lastTimeCreatedGhost = 0;

        var dashDirection = owner.transform.TransformDirection(owner.MoveDirection.normalized);
        owner.transform.rotation = Quaternion.LookRotation(dashDirection, Vector3.up);

        if (dashDirection == Vector3.zero) dashDirection = owner.transform.forward;

        while (time > 0)
        {

            time -= Time.deltaTime;
            owner.CharController.Move(dashDirection * _dashForce);

            if (Time.time > lastTimeCreatedGhost)
            {
                CreateGhostMesh();
                lastTimeCreatedGhost = Time.time + timeForGhost;
            }

            yield return new WaitForEndOfFrame();
        }

        owner.BlockMovement(false);
        owner.Animator.CrossFade("Motion", 5f);

        if (_createdTrailFx != null)
        {
            _createdTrailFx.Stop();
            Destroy(_createdTrailFx, _createdTrailFx.main.duration + 0.3f);
        }

        yield break;
    }
}
