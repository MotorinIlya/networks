using System.Globalization;

namespace Snake.Model;

public class Map
{
    private GameObjects[,] _map;
    public Map(int width, int height)
    {
        _map = new GameObjects[width, height];
        fillFloorMap();
    }

    public void AddSnake()
    {
        
    }

    public void RemoveSnake()
    {

    }

    public void AddApple()
    {

    }

    public int Height => _map.GetLength(1);

    public int Width => _map.GetLength(0);

    public GameObjects GetGameObject(int x, int y) => _map[x, y];

    public GameObjects GetGameObject(Net.GameState.Types.Coord coord) => _map[coord.X, coord.Y];

    private void fillFloorMap()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                _map[x, y] = GameObjects.Floor;
            }
        }
    }
}