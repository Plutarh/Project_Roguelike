using Zenject;

public class UIPlayerHealthBar : UIHealthBar
{
    Player _player;
    [Inject]
    public void Construct(Player playerChar)
    {
        _player = playerChar;

    }

    private void Awake()
    {
        SetPawn(_player);
    }
}
