using System.Collections;
using Zenject;

public class UIPlayerHealthBar : UIHealthBar
{
    PlayerMover _player;


    private void Awake()
    {
        GlobalEvents.OnLocalPlayerInitialized += Initialize;
    }

    void Initialize(PlayerMover player)
    {
        _player = player;
        SetPawn(_player);
    }

    private void OnDestroy()
    {
        GlobalEvents.OnLocalPlayerInitialized -= Initialize;
    }

}
