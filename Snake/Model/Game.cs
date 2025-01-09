using Snake.Controller;

namespace Snake.Model;

public class Game
{
    private GameController _gameController;

    public Game(string name, Map map)
    {
        _gameController = new GameController(name, map);
        _gameController.Start();
    }
}