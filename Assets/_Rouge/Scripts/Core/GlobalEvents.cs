using System;

public static class GlobalEvents
{
    public static Action OnPlayerHittedDamageable;
    public static Action<AIBase> OnEnemySpawned;
    public static Action<PlayerMover> OnLocalPlayerInitialized;
}
