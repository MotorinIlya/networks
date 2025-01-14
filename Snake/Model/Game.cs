using Snake.Controller;
using Snake.Net;
using Snake.View.Game;

namespace Snake.Model;

public class Game
{
    private GameController _gameController;

    private TurnController _turnController;

    //create master
    public Game(GameWindow gameWindow, string name, string gameName, Map map)
    {
        _gameController = new GameController(gameWindow, name, gameName, map);
        _turnController = new TurnController(_gameController.Model, _gameController.GamePeer);
        _gameController.AddPeerObservers(_turnController);
        _gameController.Run();
        _gameController.SearchPlayers();
    }

    //create joiner
    public Game(GameWindow gameWindow, string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _gameController = new GameController(gameWindow, playerName, gameName, config, peer, map);
        _turnController = new TurnController(_gameController.Model, peer);
        _gameController.AddPeerObservers(_turnController);
    }

    public TurnController GetTurnController() => _turnController;
}