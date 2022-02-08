using UnityEngine;
using Zenject;

public class ThirdPersonPlayerInstaller : MonoInstaller
{
    [SerializeField] private Player playerUnit;
    [SerializeField] private Transform playerUnitSpawnPoint;

    public override void InstallBindings()
    {
        BindPlayerInstance();
    }

    void BindPlayerInstance()
    {
        var playerInstance =
          Container.InstantiatePrefabForComponent<Player>(playerUnit, playerUnitSpawnPoint.position, Quaternion.identity, null);

        Container.
            Bind<Player>().
            FromInstance(playerInstance).
            AsSingle();

        Container.QueueForInject(playerInstance);
    }
}