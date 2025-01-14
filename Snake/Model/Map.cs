using Snake.Net;

namespace Snake.Model;

public class Map
{
    private GameObjects[,] _map;

    public Map(int width, int height)
    {
        _map = new GameObjects[width, height];
        FillFloorMap();
    }

    public int Height => _map.GetLength(1);

    public int Width => _map.GetLength(0);

    public GameObjects GetGameObject(int x, int y) => _map[x, y];

    public GameObjects GetGameObject(GameState.Types.Coord coord) => _map[coord.X, coord.Y];

    private void FillFloorMap()
    {
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                _map[x, y] = GameObjects.Floor;
            }
        }
    }

    public void Update(GameModel model, GameState state)
    {
        FillFloorMap();
        foreach (var snake in state.Snakes)
        {
            GameState.Types.Coord? coord = null;
            foreach (var point in snake.Points)
            {
                if (coord is null)
                {
                    coord = CoordUtils.GetEqualCoord(point);
                }
                else
                {
                    CoordUtils.Sum(coord, point);
                    CoordUtils.NormalizeForMap(coord, Width, Height);
                }

                if (snake.PlayerId == model.MainId)
                {
                    _map[coord.X, coord.Y] = GameObjects.MainSnakeBody;
                }
                else
                {
                    _map[coord.X, coord.Y] = GameObjects.SnakeBody;
                }
            }
        }

        foreach (var apple in state.Foods)
        {
            _map[apple.X, apple.Y] = GameObjects.Apple;
        }
    }
}