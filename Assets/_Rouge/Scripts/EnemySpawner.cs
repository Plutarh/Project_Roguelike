using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Zenject;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private int _spawnCount;
    [SerializeField] private AIBase _enemyPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _spawnRadius;

    IEnemyFactory _enemyFactory;


    [SerializeField] private Color colorPicker;


    [Inject]
    public void Construct(IEnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    private void Start()
    {
        if (!isServer) return;
        StartCoroutine(IETestSpawn());
    }

    IEnumerator IETestSpawn()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            SpawnEnemy();
            yield return new WaitForEndOfFrame();
        }
    }

    public AIBase SpawnEnemy()
    {
        var randomedSpawnPosition = _spawnPoint.transform.position + (UnityEngine.Random.insideUnitSphere * _spawnRadius);
        randomedSpawnPosition.y = _spawnPoint.position.y;

        var createdEnemy = _enemyFactory.Create(_enemyPrefab, randomedSpawnPosition);

        NetworkServer.Spawn(createdEnemy.gameObject);

        GlobalEvents.OnEnemySpawned?.Invoke(createdEnemy);

        return createdEnemy;
    }

    private void OnDrawGizmos()
    {
        if (_spawnPoint == null) return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(_spawnPoint.transform.position, _spawnRadius);
    }
}
