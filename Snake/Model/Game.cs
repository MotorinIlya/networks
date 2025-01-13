using Snake.Controller;
using Snake.Net;

namespace Snake.Model;

public class Game
{
    private GameController _gameController;

    private TurnController _turnController;

    public Game(string name, string gameName, Map map)
    {
        _gameController = new GameController(name, gameName, map);
        _turnController = new TurnController(_gameController.Model);
        _gameController.AddPeerObservers(_turnController);
        _gameController.Run();
        _gameController.SearchPlayers();
    }

    public Game(string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _gameController = new GameController(playerName, gameName, config, peer, map);
        _turnController = new TurnController(_gameController.Model);
        _gameController.AddPeerObservers(_turnController);
    }

    public TurnController GetTurnController() => _turnController;
}