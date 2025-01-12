using Snake.Net;

namespace Snake.Utils;

public static class SumCoords
{
    public static void Sum(GameState.Types.Coord src, GameState.Types.Coord offset)
    {
        src.X += offset.X;
        src.Y += offset.Y;
    }

    public static GameState.Types.Coord GetWithOffset(GameState.Types.Coord src, GameState.Types.Coord offset)
    {
        var coord = new GameState.Types.Coord
        {
            X = src.X,
            Y = src.Y
        };
        Sum(coord, offset);
        return coord;
    }
}