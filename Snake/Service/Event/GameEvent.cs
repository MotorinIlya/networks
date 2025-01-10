using Snake.Net;

namespace Snake.Service.Event;

public class GameEvent
{
    private GameMessage _message;

    public GameEvent(GameMessage message)
    {
        _message = message;
    }

    public GameMessage Message => _message;
}