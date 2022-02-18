using UnityEngine;
using Zenject;

public class ThirdPersonPlayerInstaller : MonoInstaller
{
    [Header("Prefabs")]
    [SerializeField] private Player playerUnit;
    [SerializeField] private PlayerCamera playerCameraPrefab;
    [SerializeField] private Transform playerUnitSpawnPoint;
    [SerializeField] private InputService inputServicePrefab;

    [Space]
    [Header("Instances")]
    [SerializeField] private Player _playerInstance;
    [SerializeField] private PlayerCamera _playerCameraInstance;

    public override void InstallBindings()
    {
        BindInputService();
        BindPlayerInstance();
        BindPlayerCameraInstance();
    }

    void BindPlayerInstance()
    {
        var playerInstance = Container
            .InstantiatePrefabForComponent<Player>(playerUnit, playerUnitSpawnPoint.position, Quaternion.identity, null);

        Container
            .Bind<Player>()
            .FromInstance(playerInstance)
            .AsSingle();

        Container
            .Bind<Transform>()
            .WithId("Player_Transform")
            .FromInstance(playerInstance.transform);

        Container.QueueForInject(playerInstance);

        _playerInstance = playerInstance;
    }

    void BindInputService()
    {
        Container
            .Bind<IInputService>()
            .To<InputService>()
            .FromComponentInNewPrefab(inputServicePrefab)
            .AsSingle();
    }

    void BindPlayerCameraInstance()
    {
        var cameraInstance =
         Container.InstantiatePrefabForComponent<PlayerCamera>(playerCameraPrefab, playerUnitSpawnPoint.position, Quaternion.identity, null);

        Container
            .Bind<PlayerCamera>()
            .FromInstance(cameraInstance)
            .AsSingle();

        Container.QueueForInject(cameraInstance);

        _playerCameraInstance = cameraInstance;
    }

    private void OnDrawGizmos() {
        if(playerUnitSpawnPoint != null && playerUnit != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawMesh(playerUnit.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh,-1,playerUnitSpawnPoint.transform.position,playerUnitSpawnPoint.transform.rotation);
        }
    }
}
