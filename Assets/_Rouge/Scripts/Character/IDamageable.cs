using Mirror;
using UnityEngine;

public interface IDamageable
{
    EPawnTeam GetTeam();
    GameObject GetGameObject();
    NetworkIdentity GetNetworkIdentity();
    void SetTeam(EPawnTeam newTeam);
    void TakeDamage(DamageData damageData);
    void Death(DamageData damageData);
    void AddEffect(TimedEffect effect);
    void RemoveEffect(TimedEffect effect);
    void EffectsTimeTick();
}

public enum EPawnTeam
{
    AI,
    Player
}
