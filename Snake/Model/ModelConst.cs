using System.Collections.Generic;
using Avalonia.Input;
using Snake.Net;

namespace Snake.Model;


public static class MConst
{
    public static readonly GameState.Types.Coord Up = new()
    {
        X = 0,
        Y = -1
    };

    public static readonly GameState.Types.Coord Down = new()
    {
        X = 0,
        Y = 1
    };

    public static readonly GameState.Types.Coord Right = new()
    {
        X = 1,
        Y = 0
    };

    public static readonly GameState.Types.Coord Left = new()
    {
        X = -1,
        Y = 0
    };

    public static readonly Dictionary<Direction, GameState.Types.Coord> OppositeDirection = new()
    {
        {Direction.Down, Up},
        {Direction.Left, Right},
        {Direction.Right, Left},
        {Direction.Up, Down}
    };

    public static readonly Dictionary<Direction, GameState.Types.Coord> TrueDirection = new()
    {
        {Direction.Down, Down},
        {Direction.Left, Left},
        {Direction.Right, Right},
        {Direction.Up, Up}
    };

    public static readonly GameState.Types.Coord StartSnakeHead = new()
    {
        X = 5,
        Y = 5
    };

    public static readonly Dictionary<Key, Direction> KeyDirection = new()
    {
        {Key.S, Direction.Down},
        {Key.A, Direction.Left},
        {Key.D, Direction.Right},
        {Key.W, Direction.Up}
    };
}