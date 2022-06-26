using Mirror;
using UnityEngine;

[System.Serializable]
public class CombatData
{
    public NetworkIdentity whoOwner;
    public bool isCritical;
    public float combatValue;
    public Vector3 hitPosition;
    public Vector3 velocity;
}
