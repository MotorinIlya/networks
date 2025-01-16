using Avalonia.Input;
using Snake.Controller;
using Snake.Net;
using Snake.View.Game;

namespace Snake.Model;

public class Game
{
    private GameController _gameController;
    private TurnController _turnController;


    public TurnController TurnController => _turnController;
    public GameState GameState => _gameController.GameState;

    //create master
    public Game(GameWindow gameWindow, string name, string gameName, Map map)
    {
        _gameController = new GameController(gameWindow, name, gameName, map);
        _turnController = new TurnController(_gameController.Model, _gameController.GamePeer);
        _gameController.AddPeerObservers(_turnController);
    }

    //create joiner
    public Game(GameWindow gameWindow, string playerName, string gameName, GameAnnouncement config, Peer peer, Map map)
    {
        _gameController = new GameController(gameWindow, playerName, gameName, config, peer, map);
        _turnController = new TurnController(_gameController.Model, peer);
        _gameController.AddPeerObservers(_turnController);
    }

    public void Update(Key e)
    {
        _turnController.Update(e);
    }

    public void Run()
    {
        _gameController.Run();
    }
}