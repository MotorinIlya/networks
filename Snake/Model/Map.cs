using System.Globalization;
using Snake.Net;
using Snake.Utils;

namespace Snake.Model;

public class Map
{
    private GameObjects[,] _map;

    public Map(int width, int height)
    {
        _map = new GameObjects[width, height];
        fillFloorMap();
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

    public void Update(GameState state)
    {
        fillFloorMap();
        foreach (var snake in state.Snakes)
        {
            GameState.Types.Coord? coord = null;
            foreach (var point in snake.Points)
            {
                if (coord is null)
                {
                    coord = point;
                }
                else
                {
                    SumCoords.Sum(coord, point);
                }
                _map[coord.X, coord.Y] = GameObjects.SnakeBody;
            }
        }

        foreach (var apple in state.Foods)
        {
            _map[apple.X, apple.Y] = GameObjects.Apple;
        }
    }
}