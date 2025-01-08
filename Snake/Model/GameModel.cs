using System;
using System.Collections.Generic;
using System.Net;
using Snake.Controller;
using Snake.Net;

namespace Snake.Model;

public class GameModel
{
    private GamePlayers _gamePlayers;

    private GameConfig _gameConfig;

    private string _gameName = "sss";

    private Map _map;

    private int _playerId = 1;

    private string _main_name;
    public GameModel(string name, Map map, IPEndPoint endPoint)
    {
        _map = map;
        _main_name = name;
        _gamePlayers = new GamePlayers();
        _gamePlayers.Players.Add(CreatePlayer(_main_name, endPoint.Address.ToString(), endPoint.Port, NodeRole.Master, 0));
        _gameConfig = new GameConfig()
        {
            Width = _map.Width,
            Height = _map.Height,
        };
    }

    public GamePlayer CreatePlayer(string name, string ipAddress, int port, NodeRole role, int score)
    {
        var player = new GamePlayer()
        {
            Id = _playerId,
            Name = name,
            IpAddress = ipAddress,
            Port = port,
            Role = role,
            Score = score
        };
        _playerId++;
        return player;
    }

    public GamePlayers Players => _gamePlayers;
    public GameConfig Config => _gameConfig;
    public string MainName => _main_name;
    public string GameName => _gameName;
}