using System;
using System.Collections.Generic;
using System.Net;
using Avalonia.Controls.Documents;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;
using Snake.Utils;

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
        _state = new GameState()
        {
            StateOrder = _lastOrderState
        };
        AddPlayer(_playerName, endPoint, NodeRole.Master, MConst.StartSnakeHead);
        //_gamePlayers.Players.Add(CreatePlayer(_playerName, endPoint.Address.ToString(), endPoint.Port, NodeRole.Master));
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

    public void Run()
    {
        while (_state is null);
        var bodyList = new List<GameState.Types.Coord>();
        while (true)
        {
            var eattenApple = 0;
            bodyList.Clear();
            foreach (var snake in _state.Snakes)
            {
                var gameObject = _map.GetGameObject(SumCoords.GetWithOffset(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]));
                if (gameObject == GameObjects.Apple)
                {
                    Eat(snake);
                    eattenApple++;
                }
                else
                {
                    Move(snake);
                }
                GetCoordsBodySnake(snake, bodyList);
            }
        }
    }

    private void GetCoordsBodySnake(GameState.Types.Snake snake, List<GameState.Types.Coord> bodyList)
    {
        for (var i = 1; i < snake.Points.Count; i++)
        {
            
            bodyList.Add(snake.Points[i]);
        }
    }

    private void Eat(GameState.Types.Snake snake)
    {
        snake.Points.Insert(1, MConst.OppositeDirection[snake.HeadDirection]);
        SumCoords.Sum(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
    }

    private void Move(GameState.Types.Snake snake)
    {
        snake.Points.Insert(1, MConst.OppositeDirection[snake.HeadDirection]);
        SumCoords.Sum(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
        snake.Points.RemoveAt(snake.Points.Count - 1);
    }

    private void AddApple()
    {
        var position = FinderFreePosition.FreePositionApple(_map);
        _state.Foods.Add(position);
    }

    private void RemoveApple(GameState.Types.Coord position)
    {
        _state.Foods.Remove(position);
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
        _state.Snakes.Add(snake);
        _state.Players.Players.Add(player);

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