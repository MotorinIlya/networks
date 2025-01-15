using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Avalonia.Media;
using Snake.Net;
using Snake.View.Game;

namespace Snake.Model;

public class GameModel
{
    private Peer _peer;
    private Dictionary<IPEndPoint, int> _endPointToId = [];

    private GameConfig _gameConfig;

    private string _gameName = "sss";

    private Map _map;

    private int _playerId = 1;
    private int _mainId = 0;
    private int _deputyId = 0;
    private int _masterId = 0;
    private string _playerName;

    private GameState? _state;

    private NodeRole _nodeRole;

    private int _lastOrderState = 0;

    //create master
    public GameModel(Peer peer, string playerName, string gameName, Map map, IPEndPoint endPoint)
    {
        _peer = peer;
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
        _nodeRole = NodeRole.Master;
        _masterId = _mainId;
    }

    //create joiner
    public GameModel(Peer peer, string playerName, string gameName, Map map, IPEndPoint endPoint, GameAnnouncement config)
    {
        _peer = peer;
        _map = map;
        _playerName = playerName;
        _gameName = gameName;
        _gameConfig = config.Config;
    }

    public void Run(GameWindow window)
    {
        var threadRun = new Thread(() => {
            RunState(window);
        });
        threadRun.Start();
    }

    private void RunState(GameWindow window)
    {
        while (_state is null);
        var bodyMap = new int[_map.Width, _map.Height];
        var snakeList = new List<GameState.Types.Snake>();
        while (_nodeRole == NodeRole.Master)
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
            _map.Update(this, _state);
            _state.StateOrder++;
            SetMasterAndDeputy();
            window.UpdateStatistics(_state);
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
        if (gameId == _mainId && _mainId == _masterId)
        {
            var msg = CreatorMessages.CreateRoleChangeMsg(NodeRole.Viewer, NodeRole.Master, _mainId, _deputyId);
            _peer.AddMsg(msg, GetEndPoint(_deputyId));
        }
        var player = GetPlayer(gameId);
        _nodeRole = NodeRole.Viewer;
        player.Role = NodeRole.Viewer;
    }

    private void SetZombieSnake(int playerId)
    {
        foreach (var snake in _state.Snakes)
        {
            if (snake.PlayerId == playerId)
            {
                snake.State = GameState.Types.Snake.Types.SnakeState.Zombie;
            }
        }
    }

    public void InactivePlayer(int inactiveId)
    {
        var player = GetPlayer(inactiveId);
        _state.Players.Players.Remove(player);

        var inactiveRole = GetPlayer(inactiveId).Role;
        if (GetMain().Role == NodeRole.Normal)
        {
            if (inactiveRole == NodeRole.Master)
            {
                _masterId = _deputyId;
                _deputyId = 0;
            }
        }
        else if (GetMain().Role == NodeRole.Master)
        {
            if (inactiveRole == NodeRole.Deputy)
            {
                var playerId = SetDeputy();
                if (playerId != 0)
                {
                    var msg = CreatorMessages.CreateRoleChangeMsg(MConst.RoleChange.Receiver, 
                                                                    NodeRole.Deputy, 
                                                                    _mainId, 
                                                                    playerId);
                    _peer.AddMsg(msg, GetEndPoint(playerId));
                }
            }
            SetZombieSnake(inactiveId);
        }
        else if (GetMain().Role == NodeRole.Deputy)
        {
            if (inactiveRole == NodeRole.Master)
            {
                _masterId = _mainId;
                var id = SetDeputy();
                CreatorMessages.CreateForAllRoleChangeMsg(_peer, this, _mainId, id);
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
        snake.Points.Insert(1, MConst.OppositeDirectionCoord[snake.HeadDirection]);
        CoordUtils.Sum(snake.Points[0], MConst.TrueDirection[snake.HeadDirection]);
        CoordUtils.NormalizeForMap(snake.Points[0], _gameConfig.Width, _gameConfig.Height);
        GetPlayer(snake.PlayerId).Score++;
        
    }

    private void Move(GameState.Types.Snake snake)
    {
        snake.Points.Insert(1, MConst.OppositeDirectionCoord[snake.HeadDirection]);
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
        _endPointToId[endPoint] = player.Id;
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

    public int AddViewer(string name, IPEndPoint endPoint)
    {
        var player = CreatePlayer(name, endPoint.Address.ToString(), endPoint.Port, NodeRole.Viewer);
        _endPointToId[endPoint] = player.Id;
        _state.Players.Players.Add(player);
        return player.Id;
    }

    public void UpdateMap(GameState state)
    {
        if (state.StateOrder > _lastOrderState)
        {
            _lastOrderState = state.StateOrder;
            _state = state;
            _map.Update(this, state);
            SetMasterAndDeputy();
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

    public GamePlayer? GetPlayer(int id)
    {
        foreach (var player in _state.Players.Players)
        {
            if (player.Id == id)
            {
                return player;
            }
        }
        return null;
    }

    public bool HasPlayer(int id)
    {
        foreach (var player in _state.Players.Players)
        {
            if (player.Id == id)
            {
                return true;
            }
        }
        return false;
    }

    public GameState.Types.Snake? GetSnake(int id)
    {
        foreach (var snake in _state.Snakes)
        {
            if (snake.PlayerId == id)
            {
                return snake;
            }
        }
        return null;
    }

    public void SetMasterAndDeputy()
    {
        var HasDeputy = false;
        foreach (var player in _state.Players.Players)
        {
            if (player.Id == _mainId)
            {
                _nodeRole = player.Role;
            }
            if (player.Role == NodeRole.Master)
            {
                _masterId = player.Id;
            }
            if (player.Role == NodeRole.Deputy)
            {
                _deputyId = player.Id;
                HasDeputy = true;
            }
        }

        //search and create deputy
        if (!HasDeputy)
        {
            SetDeputy();
        }
    }

    public int SetDeputy()
    {
        foreach (var player in _state.Players.Players)
        {
            if (player.Role == NodeRole.Normal)
            {
                player.Role = NodeRole.Deputy;
                return player.Id;
            }
        }
        return 0;
    }

    public void SetMaster(int id)
    {
        _masterId = id;
        GetPlayer(id).Role = NodeRole.Master;
    }

    public void SetRole(NodeRole newRole)
    {
        _nodeRole = newRole;
        GetMain().Role = newRole;
        if (newRole == NodeRole.Master)
        {
            _masterId = _mainId;

        }
        else if (newRole == NodeRole.Deputy)
        {
            _deputyId = _mainId;
        }
    }

    public IPEndPoint GetEndPoint(int id)
    {
        var player = GetPlayer(id);
        return new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port);
    }

    public GamePlayer? GetMaster() => GetPlayer(_masterId);
    public GamePlayer GetMain() => GetPlayer(_mainId);
    public void SetId(int id) => _mainId = id;
    public GameState GetState() => _state;
    public GamePlayers Players => _state.Players;
    public GameConfig Config => _gameConfig;
    public string MainName => _playerName;
    public int MainId => _mainId;
    public string GameName => _gameName;
    public Map GameMap => _map;
    public NodeRole Role => _nodeRole;
    public int StateDelayMs => _gameConfig.StateDelayMs;
    public int EndPointToId(IPEndPoint endPoint)
    {
        var address = endPoint.Address.ToString();
        var port = endPoint.Port;
        foreach (var player in _state.Players.Players)
        {
            if ((string.Compare(player.IpAddress, address) == 0) && (player.Port == port))
            {
                return player.Id;
            }
        }
        throw new Exception();
    }
}