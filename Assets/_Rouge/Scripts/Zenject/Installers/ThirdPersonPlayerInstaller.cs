using UnityEngine;
using Zenject;

public class ThirdPersonPlayerInstaller : MonoInstaller
{
    [SerializeField] private Player playerUnit;
    [SerializeField] private PlayerCamera playerCameraPrefab;
    [SerializeField] private Transform playerUnitSpawnPoint;


    private Player _playerInstance;
    private PlayerCamera _playerCameraInstance;

    

    public override void InstallBindings()
    {
        BindPlayerInstance();
        BindPlayerCameraInstance();
    }

    void BindPlayerInstance()
    {
        var playerInstance =
          Container.InstantiatePrefabForComponent<Player>(playerUnit, playerUnitSpawnPoint.position, Quaternion.identity, null);

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

    void BindPlayerCameraInstance()
    {
        var cameraInstance =
         Container.InstantiatePrefabForComponent<PlayerCamera>(playerCameraPrefab, playerUnitSpawnPoint.position, Quaternion.identity, null);

        Container
            .Bind<PlayerCamera>()
            .FromInstance(cameraInstance)
            .AsSingle();


        //Container.QueueForInject(cameraInstance);



        _playerCameraInstance = cameraInstance;
    }
}
