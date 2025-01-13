using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Snake.Net;

namespace Snake.Model;

public class GameModel
{
    private GameConfig _gameConfig;

    private string _gameName = "sss";

    private Map _map;

    private int _playerId = 1;

    private int _mainId;
    private string _playerName;

    private GameState? _state;

    private int _lastOrderState = 0;
    public GameModel(string playerName, string gameName, Map map, IPEndPoint endPoint)
    {
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _state = new GameState()
        {
            StateOrder = _lastOrderState,
            Players = new GamePlayers()
        };
        AddPlayer(_playerName, endPoint, NodeRole.Master, MConst.StartSnakeHead);
        _mainId = _state.Players.Players[0].Id;
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
        _gameConfig = config.Config;
    }

    public void Run()
    {
        var threadRun = new Thread(RunState);
        threadRun.Start();
    }

    private void RunState()
    {
        while (_state is null);
        var bodyMap = new int[_map.Width, _map.Height];
        var snakeList = new List<GameState.Types.Snake>();
        while (true)
        {
            Array.Clear(bodyMap);
            snakeList.Clear();
            // move snakes and count eatten apples
            foreach (var snake in _state.Snakes)
            {
                var coord = CoordUtils.GetWithOffset(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
                CoordUtils.NormalizeForMap(coord, _gameConfig.Width, _gameConfig.Height);
                var gameObject = _map.GetGameObject(coord);
                if (gameObject == GameObjects.Apple)
                {
                    Eat(snake);
                    RemoveApple(snake.Points[0]);
                }
                else
                {
                    Move(snake);
                }
                GetCoordsBodySnake(snake, bodyMap);
            }

            // verify that snake may be in this position
            foreach (var snake in _state.Snakes)
            {
                var head = snake.Points[0];
                if (bodyMap[head.X, head.Y] == 1)
                {
                    Die(snake);
                }
                else if (bodyMap[head.X, head.Y] == 0)
                {
                    bodyMap[head.X, head.Y] = 2;
                    snakeList.Add(snake);
                }
                else if (bodyMap[head.X, head.Y] == 2)
                {
                    var searchedSnake = SearchSnake(snakeList, snake.Points[0]);
                    Die(searchedSnake);
                    Die(snake);
                }
            }

            // generate new apple
            for (var i = 0; i < _gameConfig.FoodStatic - _state.Foods.Count; i++)
            {
                AddApple();
            }
            _map.Update(_state);
            Thread.Sleep(_gameConfig.StateDelayMs);
        }
    }

    private void Die(GameState.Types.Snake? snake)
    {
        if (snake is not null)
        {
            DeletePlayer(snake.PlayerId);
            _state.Snakes.Remove(snake);
        }
    }

    private void DeletePlayer(int gameId)
    {
        foreach (var player in _state.Players.Players)
        {
            if (player.Id == gameId)
            {
                _state.Players.Players.Remove(player);
            }
        }
    }

    private GameState.Types.Snake? SearchSnake(List<GameState.Types.Snake> list, GameState.Types.Coord coord)
    {
        foreach(var snake in list)
        {
            if (CoordUtils.EqualCoord(snake.Points[0], coord))
            {
                return snake;
            }
        }
        return null;
    }

    private void GetCoordsBodySnake(GameState.Types.Snake snake, int[,] bodyMap)
    {
        var tmpCoord = CoordUtils.GetEqualCoord(snake.Points[0]);
        for (var i = 1; i < snake.Points.Count; i++)
        {
            CoordUtils.Sum(tmpCoord, snake.Points[i]);
            CoordUtils.NormalizeForMap(tmpCoord, _gameConfig.Width, _gameConfig.Height);
            bodyMap[tmpCoord.X, tmpCoord.Y] = 1;
        }
    }

    private void Eat(GameState.Types.Snake snake)
    {
        snake.Points.Insert(1, MConst.OppositeDirection[snake.HeadDirection]);
        CoordUtils.Sum(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
        CoordUtils.NormalizeForMap(snake.Points[0], _gameConfig.Width, _gameConfig.Height);
    }

    private void Move(GameState.Types.Snake snake)
    {
        snake.Points.Insert(1, MConst.OppositeDirection[snake.HeadDirection]);
        CoordUtils.Sum(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
        CoordUtils.NormalizeForMap(snake.Points[0], _gameConfig.Width, _gameConfig.Height);
        snake.Points.RemoveAt(snake.Points.Count - 1);
    }

    private void AddApple()
    {
        var position = FinderFreePosition.FreePositionApple(_map);
        _state.Foods.Add(position);
    }

    private void RemoveApple(GameState.Types.Coord position)
    {
        for (var i = 0; i < _state.Foods.Count; i++)
        {
            if (CoordUtils.EqualCoord(_state.Foods[i], position))
            {
                _state.Foods.RemoveAt(i);
                return;
            }
        }
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
        _state.Players.Players.Add(player);
        var snake = new GameState.Types.Snake
        {
            PlayerId = player.Id,
            State = GameState.Types.Snake.Types.SnakeState.Alive,
            HeadDirection = Direction.Right
        };
        snake.Points.Add(head);
        snake.Points.Add(MConst.Left);
        _state.Snakes.Add(snake);

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

    public void ChangeDirection(Direction newDirection, int playerId)
    {
        foreach(var snake in _state.Snakes)
        {
            if (snake.PlayerId == playerId)
            {
                snake.HeadDirection = newDirection;
                return;
            }
        }
    }

    public void ChangeDirection(Direction newDirection, IPEndPoint endPoint)
    {
        string ipAddres = endPoint.Address.ToString();
        int port = endPoint.Port;
        foreach (var player in _state.Players.Players)
        {
            if (ipAddres == player.IpAddress && port == player.Port)
            {
                ChangeDirection(newDirection, player.Id);
                return;
            }
        }
    }

    public GameState GetState() => _state;
    public GamePlayers Players => _state.Players;
    public GameConfig Config => _gameConfig;
    public string MainName => _playerName;
    public int MainId => _mainId;
    public string GameName => _gameName;
    public Map GameMap => _map;
}