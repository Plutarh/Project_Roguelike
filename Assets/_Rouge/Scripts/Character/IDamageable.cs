using UnityEngine;

public interface IDamageable
{
    EPawnTeam GetTeam();
    GameObject GetGameObject();
    void SetTeam(EPawnTeam newTeam);
    void TakeDamage(DamageData damageData);
    void Death();
    void AddEffect(TimedEffect effect);
    void RemoveEffect(TimedEffect effect);
    void EffectsTimeTick();
}

public enum EPawnTeam
{
    AI,
    Player
}
