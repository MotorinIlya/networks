using Snake.Model;

namespace Snake.Controller;

public class GameController
{
    private GameModel _gameModel;

    public GameController(string name, Map map)
    {
        _gameModel = new GameModel(name, map);
    }
}