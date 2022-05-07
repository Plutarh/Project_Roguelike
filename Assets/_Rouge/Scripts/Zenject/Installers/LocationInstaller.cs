using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LocationInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindEnemyFactory();
    }

    void BindEnemyFactory()
    {
        Container
            .Bind<IEnemyFactory>()
            .To<EnemyFactory>()
            .AsSingle();
    }
}
