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

    public static void NormalizeForMap(GameState.Types.Coord coord, int width, int height)
    {
        if (coord.X < 0)
        {
            coord.X += width;
        }
        if (coord.Y < 0)
        {
            coord.Y += height;
        }
        coord.X %= width;
        coord.Y %= height;
    }
}