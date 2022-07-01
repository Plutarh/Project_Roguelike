using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class Chest : Interactable
{

    [SerializeField] private Rigidbody _chestLid;
    [SerializeField] private float _lidUpForce = 25;

    public override void Awake()
    {
        base.Awake();

        _chestLid.isKinematic = true;
    }

    public override void Start()
    {
        base.Start();

        if (_isUsed)
            _chestLid.gameObject.SetActive(false);
    }

    public override void Action(NetworkIdentity networkIdentity)
    {
        base.Action(networkIdentity);
        if (_isUsed) return;
        Debug.Log("Chest opened");
        CmdThrowLid();
        CmdSpawnRewards();


        triggerCollider.enabled = false;
    }

    void ThrowLid()
    {
        _isUsed = true;
        _chestLid.isKinematic = false;
        _chestLid.AddForce(Vector3.up * _lidUpForce + -transform.forward, ForceMode.Impulse);
        _chestLid.AddTorque(Random.onUnitSphere * _lidUpForce, ForceMode.Impulse);

        _chestLid.transform.DOScale(Vector3.zero, 2).SetDelay(5).OnComplete(() => Destroy(_chestLid.gameObject));

        ClearPlayers();
    }

    void ClearPlayers()
    {
        for (int i = 0; i < _enteredPlayers.Count; i++)
        {
            var player = _enteredPlayers[i].transform.root.GetComponent<PlayerCharacter>();

            if (player == null) continue;
            player.RemoveInteractable(this);

        }
    }

    [Command(requiresAuthority = false)]
    void CmdThrowLid()
    {
        RpcThrowLid();
    }

    [ClientRpc]
    void RpcThrowLid()
    {
        ThrowLid();
    }

    void SpawnRewards()
    {
        //TODO Если будем дропать какую либо шмотку, надо будет обращаться к контроллеру шмоток и спавнить ее для всех
        // Банальные монетки, опыт и тд, будут локально падать для каждого, кол-во будет рандомным через сервак
        Debug.Log("Chest Drop rewards");
    }

    [Command]
    void CmdSpawnRewards()
    {
        RpcSpawnRewards();
    }

    [ClientRpc]
    void RpcSpawnRewards()
    {
        SpawnRewards();
    }
}
