using System;
using System.Collections.Generic;
using Snake.Controller;

namespace Snake.Model;

public class GameModel
{
    private List<string> _names;

    private Map _map;
    public GameModel(string name, Map map)
    {
        _names = [name];
        _map = map;
    }
}