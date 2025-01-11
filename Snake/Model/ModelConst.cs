using System.Collections.Generic;
using Avalonia.Media;
using Snake.Net;

namespace Snake.Model;


public static class MConst
{
    public static GameState.Types.Coord Up = new()
    {
        X = 0,
        Y = -1
    };

    public static GameState.Types.Coord Down = new()
    {
        X = 0,
        Y = 1
    };

    public static GameState.Types.Coord Right = new()
    {
        X = 1,
        Y = 0
    };

    public static GameState.Types.Coord Left = new()
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

    public static readonly Dictionary<GameObjects, IBrush> ColorObject = new()
    {
        {GameObjects.Apple, Brushes.OrangeRed},
        {GameObjects.Floor, Brushes.Black},
        {GameObjects.SnakeBody, Brushes.SeaGreen}
    };
}