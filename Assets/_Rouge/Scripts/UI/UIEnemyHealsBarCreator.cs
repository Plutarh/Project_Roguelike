using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnemyHealsBarCreator : MonoBehaviour
{

    [SerializeField]
    private UIEnemyHealthBar _baseEnemyHealthBarPrefab;

    [SerializeField]
    private UIEnemyHealthBarsMover _enemyHealthBarsMover;

    private void Awake()
    {
        GlobalEvents.OnEnemySpawned += CreateEnemyHealthBar;
    }

    void CreateEnemyHealthBar(AIBase pawn)
    {
        if (_baseEnemyHealthBarPrefab == null)
        {
            Debug.LogError("Cant create enemy health bar, prefab NULL");
            return;
        }

        var createdHealthBar = Instantiate(_baseEnemyHealthBarPrefab, transform);

        createdHealthBar.SetPawn(pawn);

        _enemyHealthBarsMover.AddNewHealthBar(createdHealthBar);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnEnemySpawned -= CreateEnemyHealthBar;
    }
}
