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

    private GameState? _state;

    private int _lastOrderState = 0;
    public GameModel(string playerName, string gameName, Map map, IPEndPoint endPoint)
    {
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _gamePlayers = new GamePlayers();
        _gamePlayers.Players.Add(CreatePlayer(_playerName, endPoint.Address.ToString(), endPoint.Port, NodeRole.Master));
        _gameConfig = new GameConfig()
        {
            Width = _map.Width,
            Height = _map.Height,
        };
        _state = new GameState();
        
    }

    public GameModel(string playerName, string gameName, Map map, IPEndPoint endPoint, GameAnnouncement config)
    {
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _gamePlayers = config.Players;
        _gameConfig = config.Config;
    }

    private GamePlayer CreatePlayer(string name, string ipAddress, int port, NodeRole role)
    {
        var player = new GamePlayer()
        {
            Id = _playerId,
            Name = name,
            IpAddress = ipAddress,
            Port = port,
            Role = role,
            Score = 0
        };
        _playerId++;
        return player;
    }

    public int AddPlayer(string name, IPEndPoint endPoint, NodeRole role, GameState.Types.Coord head)
    {
        var player = CreatePlayer(name, endPoint.Address.ToString(), endPoint.Port, role);
        _gamePlayers.Players.Add(player);
        var snake = new GameState.Types.Snake
        {
            PlayerId = player.Id,
            State = GameState.Types.Snake.Types.SnakeState.Alive,
            HeadDirection = Direction.Right
        };
        snake.Points.Add(head);
        snake.Points.Add(MConst.Left);

        return player.Id;
    }

    public void UpdateMap(GameState state)
    {
        if (state.StateOrder > _lastOrderState)
        {
            _lastOrderState = state.StateOrder;
            _state = state;
            _map.Update(state);
        }
    }

    public GameState GetState() => _state;
    public GamePlayers Players => _gamePlayers;
    public GameConfig Config => _gameConfig;
    public string MainName => _playerName;
    public string GameName => _gameName;
    public Map GameMap => _map;
}