using Snake.Controller;
using Snake.Net;

namespace Snake.Model;

public class Game
{
    private GameController _gameController;

    public Game(string name, string gameName, Map map)
    {
        _gameController = new GameController(name, gameName, map);
        _gameController.Start();
    }

    public Game(string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _gameController = new GameController(playerName, gameName, config, peer, map);
    }
}