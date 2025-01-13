using Snake.Net;

namespace Snake.Model;

public static class CoordUtils
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

    public static bool EqualCoord(GameState.Types.Coord first, GameState.Types.Coord second)
    {
        return (first.X == second.X) && (first.Y == second.Y);
    }

    public static GameState.Types.Coord GetEqualCoord(GameState.Types.Coord coord)
    {
        return new GameState.Types.Coord
        {
            X = coord.X,
            Y = coord.Y
        };
    }
}