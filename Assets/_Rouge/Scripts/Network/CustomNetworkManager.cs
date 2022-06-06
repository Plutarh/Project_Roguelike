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

    public override void Start()
    {
        base.Start();

    }

    IEnumerator IEWaitToSpawnPlayer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ActivatePlayerSpawn();
    }

    public void OnCreateCharacter(NetworkConnectionToClient connection, SpawnPositionMessage message)
    {
        GameObject playerObject = Instantiate(playerPrefab, message.spawnPosition, Quaternion.identity);

        _player = playerObject;

        NetworkServer.AddPlayerForConnection(connection, playerObject);
        Debug.Log("On Create Character");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("On Start Server");
        NetworkServer.RegisterHandler<SpawnPositionMessage>(OnCreateCharacter);


    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("On Client connect");
        _playerConnected = true;

        StartCoroutine(IEWaitToSpawnPlayer());
    }

    public void ActivatePlayerSpawn()
    {
        var spawnPositions = _playerInstaller.GetPlayerSpawnPositions();
        Vector3 position = spawnPositions[Random.Range(0, spawnPositions.Count)];

        SpawnPositionMessage message = new SpawnPositionMessage()
        {
            spawnPosition = position
        };

        NetworkClient.Send(message);
        _playerSpawned = true;

        Debug.Log($"{message.spawnPosition}");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        if (_player != null)
            NetworkServer.Destroy(_player);
    }
}

public struct SpawnPositionMessage : NetworkMessage
{
    public Vector3 spawnPosition;
}
