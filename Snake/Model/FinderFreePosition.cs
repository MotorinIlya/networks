using System;
using System.Collections.Generic;
using Snake.Net;

namespace Snake.Model;

public static class FinderFreePosition
{
    private static Random _random = new();

    
    public static GameState.Types.Coord? FreePositionSnake(Map map)
    {
        for(var x = 0; x < map.Width; x++)
        {
            for(var y = 0; y < map.Height; y++)
            {
                if (IsFreePosition(map, x, y))
                {
                    return new GameState.Types.Coord
                    {
                        X = x,
                        Y = y
                    };
                }
            }
        }
        return null;
    }

    private static bool IsFreePosition(Map map, int x, int y)
    {
        for (var i = x - 2; i <= x + 2; i++)
        {
            for (var j = y - 2; j <= y + 2; j++)
            {
                var tmp1 = i;
                var tmp2 = j;
                if (tmp1 < 0 || tmp1 >= map.Width)
                {
                    tmp1 += map.Width;
                    tmp1 %= map.Width;
                }
                if (tmp2 < 0 || tmp2 >= map.Height)
                {
                    tmp2 += map.Height;
                    tmp2 %= map.Height;
                }
                if (map.GetGameObject(tmp1, tmp2) != GameObjects.Floor)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static GameState.Types.Coord FreePositionApple(Map map)
    {
        var listFreePosition = new List<GameState.Types.Coord>();
        for (var x = 0; x < map.Width; x++)
        {
            for (var y = 0; y < map.Height; y++)
            {
                if (map.GetGameObject(x, y) == GameObjects.Floor)
                {
                    listFreePosition.Add(new GameState.Types.Coord()
                    {
                        X = x,
                        Y = y
                    });
                }
            }
        }
        return listFreePosition[_random.Next(0, listFreePosition.Count - 1)];
    }
}