using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RougeUtility
{
    public static Vector3 GetExplosionVelocity(Vector3 fromPosition, Vector3 targetPosition, float explosionRadius)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 direction = targetPosition - fromPosition;

        float explosionNormalized = 1f - direction.magnitude / explosionRadius;
        float force = explosionRadius * explosionRadius * explosionNormalized * Mathf.PI;
        velocity = direction.normalized * force;

        return velocity;
    }
}
