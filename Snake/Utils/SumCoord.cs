using Snake.Net;

namespace Snake.Utils;

public static class SumCoords
{
    public static void Sum(GameState.Types.Coord src, GameState.Types.Coord dest)
    {
        src.X += dest.X;
        src.Y += dest.Y;
    }
}