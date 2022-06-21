using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    [Header("Custom Settings")]
    [SerializeField] private ThirdPersonPlayerInstaller _playerInstaller;
    [SerializeField] private bool _playerSpawned;
    [SerializeField] private bool _playerConnected;


    GameObject _player;

    public void OnCreateCharacter(NetworkConnectionToClient connection, SpawnPositionMessage message)
    {
        Debug.Log("<color=blue> On Create Character </color>");
        _player = Instantiate(playerPrefab, message.spawnPosition, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(connection, _player);



        Debug.Log($"player with ID {_player.GetComponent<NetworkBehaviour>().netId} spawned on {message.spawnPosition}");

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("<color=blue> On Start Server </color>");
        NetworkServer.RegisterHandler<SpawnPositionMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("<color=blue>On Client connect </color>");
        _playerConnected = true;

        ActivatePlayerSpawn();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();


    }

    public void ActivatePlayerSpawn()
    {
        Debug.Log("<color=blue> On ActivatePlayerSpawn </color>");
        var spawnPositions = _playerInstaller.GetPlayerSpawnPositions();
        Vector3 position = spawnPositions[Random.Range(0, spawnPositions.Count)];

        SpawnPositionMessage message = new SpawnPositionMessage()
        {
            spawnPosition = position
        };

        NetworkClient.Send(message);
        _playerSpawned = true;

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Debug.Log("On Server Disconnect");
        base.OnServerDisconnect(conn);

        if (_player != null)
            NetworkServer.Destroy(_player);
    }
}

public struct SpawnPositionMessage : NetworkMessage
{
    public Vector3 spawnPosition;
}
