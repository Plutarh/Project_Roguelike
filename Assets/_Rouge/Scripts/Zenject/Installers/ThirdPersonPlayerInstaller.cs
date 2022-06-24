using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ThirdPersonPlayerInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField] private PlayerMover playerUnit;
    [SerializeField] private PlayerCamera playerCameraPrefab;
    [SerializeField] private List<Transform> _playerUnitSpawnPoints = new List<Transform>();
    [SerializeField] private InputService inputServicePrefab;

    [Space]
    [Header("Instances")]
    [SerializeField] private PlayerMover _playerInstance;
    [SerializeField] private PlayerCamera _playerCameraInstance;



    public static ThirdPersonPlayerInstaller get;

    private void Awake()
    {
        get = this;
    }

    // Уход из офлайн режима, на онлайн, где зависимости игрока прокидываются, после подключения
    public override void InstallBindings()
    {

    }


    // Биндим локального игрока
    public void BindLocalPlayer(PlayerMover playerInstance)
    {

        _playerInstance = playerInstance;
        BindInputService();
        BindPlayerInstance();
        BindPlayerCameraInstance();

    }

    public List<Vector3> GetPlayerSpawnPositions()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        _playerUnitSpawnPoints.ForEach(pusp => spawnPositions.Add(pusp.position));
        return spawnPositions;
    }

    void BindPlayerInstance()
    {
        Container
            .Bind<PlayerMover>()
            .FromInstance(_playerInstance)
            .AsSingle();

        Container
            .Bind<Transform>()
            .WithId("Player_Transform")
            .FromInstance(_playerInstance.transform)
            .AsSingle();

        Container
            .Bind<PlayerCharacter>()
            .FromInstance(_playerInstance.GetComponent<PlayerCharacter>())
            .AsSingle();

        Container.QueueForInject(_playerInstance);

        // _playerInstance = playerInstance;
    }

    void BindInputService()
    {
        Container
            .Bind<InputService>()
            // .To<InputService>()
            .FromComponentInNewPrefab(inputServicePrefab)
            .AsSingle();
    }

    void BindPlayerCameraInstance()
    {
        var cameraInstance =
         Container.InstantiatePrefabForComponent<PlayerCamera>(playerCameraPrefab, _playerUnitSpawnPoints[0].position, Quaternion.identity, null);

        Container
            .Bind<PlayerCamera>()
            .FromInstance(cameraInstance)
            .AsSingle();

        Container.QueueForInject(cameraInstance);

        _playerCameraInstance = cameraInstance;
    }

    private void OnDrawGizmos()
    {
        if (_playerUnitSpawnPoints.Count > 0 && playerUnit != null)
        {
            Gizmos.color = Color.green;
            foreach (var spawnPos in _playerUnitSpawnPoints)
            {
                Gizmos.DrawMesh(playerUnit.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh,
                -1,
                spawnPos.position,
                spawnPos.rotation);

                Gizmos.DrawSphere(spawnPos.position, 0.3f);
            }

        }
    }
}
