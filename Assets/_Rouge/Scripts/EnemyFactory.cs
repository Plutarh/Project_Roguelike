using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyFactory : IEnemyFactory
{
    DiContainer _diContainer;

    public EnemyFactory(DiContainer diContainer)
    {
        _diContainer = diContainer;
    }

    public AIBase Create(AIBase ai, Vector3 spawnPos)
    {
        return _diContainer.InstantiatePrefabForComponent<AIBase>(ai
            , spawnPos
            , Quaternion.AngleAxis(Random.Range(-180, 180)
            , Vector3.up)
            , null);
    }
}
