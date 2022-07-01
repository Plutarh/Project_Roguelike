using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Interactable : NetworkBehaviour
{

    private Rigidbody _body;

    [SerializeField] protected List<NetworkIdentity> _enteredPlayers = new List<NetworkIdentity>();
    [SerializeField] protected Collider triggerCollider;

    [SyncVar]
    [SerializeField] protected bool _isUsed;

    public virtual void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {

    }

    public virtual void ShowText()
    {

    }

    public virtual void HideText()
    {

    }

    public virtual void Action(NetworkIdentity whoUsed)
    {

    }

    public virtual void OnPlayerEnter(PlayerCharacter playerCharacter)
    {
        if (_enteredPlayers.Contains(playerCharacter.netIdentity)) return;

        _enteredPlayers.Add(playerCharacter.netIdentity);
        playerCharacter.AddInteractable(this);

    }

    public virtual void OnPlayerExit(PlayerCharacter playerCharacter)
    {
        if (!_enteredPlayers.Contains(playerCharacter.netIdentity)) return;

        _enteredPlayers.Remove(playerCharacter.netIdentity);
        playerCharacter.RemoveInteractable(this);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isUsed) return;
        if (other == null) return;

        var player = other.transform.root.GetComponent<PlayerCharacter>();

        if (player == null) return;

        OnPlayerEnter(player);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;

        var player = other.transform.root.GetComponent<PlayerCharacter>();

        if (player == null) return;

        OnPlayerExit(player);
    }
}
