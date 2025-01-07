using Snake.Controller;
using Snake.Net;

namespace Snake.Model;

public class Game
{
    private Peer _peer;

    private GameController _gameController;

    public Game(string name, Map map)
    {
        _gameController = new GameController(name, map);
        _peer = new Peer();
    }
}