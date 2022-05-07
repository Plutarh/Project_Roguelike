public interface IDamageable
{
    EPawnTeam GetTeam();
    void SetTeam(EPawnTeam newTeam);
    void TakeDamage(DamageData damageData);
    void Death();
}

public enum EPawnTeam
{
    AI,
    Player
}
