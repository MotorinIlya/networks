using System.Net;
using Snake.Net;

namespace Snake.Service.Event;

public class GameEvent : ObserverEvent
{
    private GameMessage _message;

    private IPEndPoint _endPoint;

    public GameEvent(GameMessage message, IPEndPoint endPoint)
    {
        _message = message;
        _endPoint = endPoint;
    }

    public GameMessage Message => _message;

    public IPEndPoint IpEndPoint => _endPoint;
}