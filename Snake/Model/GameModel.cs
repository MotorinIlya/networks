using System;
using System.Collections.Generic;
using System.Net;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;

namespace Snake.Model;

public class GameModel
{
    private GamePlayers _gamePlayers;

    private GameConfig _gameConfig;

    private string _gameName = "sss";

    private Map _map;

    private int _playerId = 1;

    private string _playerName;
    public GameModel(string playerName, string gameName, Map map, IPEndPoint endPoint)
    {
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _gamePlayers = new GamePlayers();
        _gamePlayers.Players.Add(CreatePlayer(_playerName, endPoint.Address.ToString(), endPoint.Port, NodeRole.Master, 0));
        _gameConfig = new GameConfig()
        {
            Width = _map.Width,
            Height = _map.Height,
        };
    }

    public GameModel(string playerName, string gameName, Map map, IPEndPoint endPoint, GameAnnouncement config)
    {
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _gamePlayers = config.Players;
        _gameConfig = config.Config;
    }

    private GamePlayer CreatePlayer(string name, string ipAddress, int port, NodeRole role, int score)
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
    public string MainName => _playerName;
    public string GameName => _gameName;
}