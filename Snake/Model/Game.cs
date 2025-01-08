using Snake.Controller;
using Snake.Net;

namespace Snake.Model;

public class Game
{
    private Peer _peer;

    private GameController _gameController;

    public Game(string name, Map map)
    {
        _peer = new Peer();
        _gameController = new GameController(name, map, _peer.IpEndPoint);
    }

    public void Start()
    {
        _peer.Start();
    }
}