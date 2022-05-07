using UnityEngine;

public interface IEnemyFactory
{
    AIBase Create(AIBase ai, Vector3 position);
}
