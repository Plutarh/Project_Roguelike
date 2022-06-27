using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
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
    }

    public override void OnInitialized()
    {
        base.OnInitialized();
        _skinnedMeshRenderers = playerCharacter.transform.root.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
    }

    public override void Update()
    {
        base.Update();
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
        base.Execute();

        TryCreateTrailFX();
        StartCoroutine(IEDashing());
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

            if (ghost != null)
                Destroy(ghost, _ghostLifeTime);
        }
    }

    void TryCreateTrailFX()
    {
        CreateTrailFX();
        CmdCreateTrailFX();
    }

    void CreateTrailFX()
    {
        if (_trailFxPrefab == null) return;

        _createdTrailFx = Instantiate(_trailFxPrefab, playerCharacter.transform.root);
        _createdTrailFx.transform.localPosition = Vector3.zero;
        _createdTrailFx.Play();
    }

    [Command]
    void CmdCreateTrailFX()
    {
        RpcCreateTrailFX();
    }

    [ClientRpc(includeOwner = false)]
    void RpcCreateTrailFX()
    {
        CreateTrailFX();
        StartCoroutine(IELocalPlayingFX());
    }

    void TryDestroyTrailFX()
    {
        DestroyTrailFX();
        CmdDestroyTrailFX();
    }

    void DestroyTrailFX()
    {
        if (_createdTrailFx == null)
        {
            Debug.LogError("Trail FX NULLED", playerCharacter);
            return;
        }

        _createdTrailFx.Stop();
        Destroy(_createdTrailFx, _createdTrailFx.main.duration + 0.3f);
    }

    [Command]
    void CmdDestroyTrailFX()
    {
        RpcDestroyTrailFX();
    }

    [ClientRpc]
    void RpcDestroyTrailFX()
    {
        DestroyTrailFX();
    }

    IEnumerator IEDashing()
    {
        float time = _executeTime;
        float timeForGhost = time / _ghostCount;
        float lastTimeCreatedGhost = 0;

        Vector3 dashDirection = Vector3.zero;

        if (playerCharacter.PlayerMover.MoveDirection != Vector3.zero)
            dashDirection = owner.transform.TransformDirection(playerCharacter.PlayerMover.MoveDirection.normalized);
        else
            dashDirection = owner.transform.forward;

        owner.transform.rotation = Quaternion.LookRotation(dashDirection, Vector3.up);
        playerCharacter.PlayerMover.BlockMovement(true);

        while (time > 0)
        {

            time -= Time.deltaTime;
            playerCharacter.PlayerMover.CharController.Move(dashDirection * _dashForce);

            if (Time.time > lastTimeCreatedGhost)
            {
                CreateGhostMesh();
                lastTimeCreatedGhost = Time.time + timeForGhost;
            }

            yield return new WaitForEndOfFrame();
        }

        playerCharacter.PlayerMover.BlockMovement(false);
        playerCharacter.PlayerMover.Animator.CrossFade("Motion", 5f);

        TryDestroyTrailFX();

        yield break;
    }

    IEnumerator IELocalPlayingFX()
    {
        float time = _executeTime;
        float timeForGhost = time / _ghostCount;
        float lastTimeCreatedGhost = 0;

        while (time > 0)
        {
            time -= Time.deltaTime;

            if (Time.time > lastTimeCreatedGhost)
            {
                CreateGhostMesh();
                lastTimeCreatedGhost = Time.time + timeForGhost;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
