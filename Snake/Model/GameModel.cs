using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Snake.Net;
using Snake.Service;
using Snake.Service.Event;
using Snake.View.Game;

namespace Snake.Model;

public class GameModel : Observable
{
    private GameConfig _gameConfig;
    private GameState _state;
    private Map _map;
    private string _gameName = "sss";
    private int _playerId = 1;
    private int _mainId = 0;
    private int _lastOrderState = 0;
    private bool _isRun = false;
    private object _stateLock;
    private object _runLock;

    public GamePlayers Players => _state.Players;
    public GameConfig Config => _gameConfig;
    public GameState State => _state;
    public int MainId => _mainId;
    public string GameName => _gameName;
    public Map GameMap => _map;
    public NodeRole Role => GetPlayer(_mainId).Role;


    //create master
    public GameModel(string playerName, 
                    string gameName, 
                    Map map, 
                    IPEndPoint endPoint, 
                    object stateLock)
    {
        _stateLock = stateLock;
        _map = map;
        _gameName = gameName;
        _state = new GameState()
        {
            StateOrder = _lastOrderState,
            Players = new GamePlayers()
        };
        AddPlayer(playerName, endPoint, NodeRole.Master, MConst.StartSnakeHead);
        _mainId = _state.Players.Players[0].Id;
        _gameConfig = new GameConfig()
        {
            Width = _map.Width,
            Height = _map.Height,
        };
    }

    //create joiner
    public GameModel(string playerName, 
                    string gameName, 
                    Map map, 
                    IPEndPoint endPoint, 
                    GameAnnouncement config, 
                    object stateLock)
    {
        _stateLock = stateLock;
        _map = map;
        _gameName = gameName;
        _gameConfig = config.Config;
        _state = new GameState()
        {
            StateOrder = _lastOrderState
        };
    }


    public GamePlayer GetMain() => GetPlayer(_mainId);
    public GamePlayer GetMaster() => GetPlayer(NodeRole.Master) is GamePlayer player 
                                    ? player
                                    : throw new Exception(); 

    public GamePlayer? GetDeputy() => GetPlayer(NodeRole.Deputy) is GamePlayer player
                                    ? player
                                    : null;
    public IPEndPoint GetEndPoint(int id)
    {
        var player = GetPlayer(id);
        return new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port);
    }

    public int EndPointToId(IPEndPoint endPoint)
    {
        var address = endPoint.Address.ToString();
        var port = endPoint.Port;
        if (Players is GamePlayers players)
        {
            foreach (var player in players.Players)
            {
                if ((string.Compare(player.IpAddress, address) == 0) && (player.Port == port))
                {
                    return player.Id;
                }
            }
        }
        return 0;
    }

    public GamePlayer GetPlayer(int id)
    {
        foreach (var player in Players.Players)
        {
            if (player.Id == id)
            {
                return player;
            }
        }
        throw new Exception();
    }

    public GamePlayer? GetPlayer(NodeRole role)
    {
        foreach (var player in Players.Players)
        {
            if (player.Role == role)
            {
                return player;
            }
        }
        return null;
    }

    public void SetId(int id) => _mainId = id;

    public void Run()
    {
        var threadRun = new Thread(RunState);
        threadRun.Start();
    }

    private void RunState()
    {
        _isRun = true;
        Trace.WriteLine("Run state is work");
        var bodyMap = new int[_map.Width, _map.Height];
        var snakeList = new List<GameState.Types.Snake>();
        while (_isRun)
        {
            Array.Clear(bodyMap);
            snakeList.Clear();
            lock (_stateLock)
            {
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
                SearchDeputy();
                if (!_isRun)
                {
                    break;
                }
                Update(new ModelEvent(ModelAction.UpdateStatistics, 0));
            }
            Thread.Sleep(_gameConfig.StateDelayMs);
        }
    }

    public void InactivePlayer(int inactiveId)
    {
        var player = GetPlayer(inactiveId);
        var inactiveRole = player.Role;

        if (GetMain().Role == NodeRole.Normal)
        {
            if (inactiveRole == NodeRole.Master)
            {
                if (GetDeputy() is GamePlayer deputy)
                {
                    deputy.Role = NodeRole.Master;
                }
            }
        }
        else if (GetMain().Role == NodeRole.Master)
        {
            if (inactiveRole == NodeRole.Deputy)
            {
                var playerId = SetDeputy();
                if (playerId != 0)
                {
                    Update(new ModelEvent(ModelAction.SendRoleMsgRecvDeputy, playerId));
                }
            }
            SetZombieSnake(inactiveId);
        }
        else if (GetMain().Role == NodeRole.Deputy)
        {
            if (inactiveRole == NodeRole.Master)
            {
                var id = SetDeputy();
                GetMain().Role = NodeRole.Master;
                Update(new ModelEvent(ModelAction.SendRoleMsgSendMaster, id));
            }
        }
        _state.Players.Players.Remove(player);
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

    public int AddViewer(string name, IPEndPoint endPoint)
    {
        var player = CreatePlayer(name, endPoint.Address.ToString(), endPoint.Port, NodeRole.Viewer);
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

    public GameState.Types.Snake GetSnake(int id)
    {
        foreach (var snake in _state.Snakes)
        {
            if (snake.PlayerId == id)
            {
                return snake;
            }
        }
        throw new Exception();
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

    public void SetOtherRole(int id, NodeRole nodeRole)
    {
        if (GetPlayer(id) is GamePlayer player)
        {
            player.Role = nodeRole;
        }
    }

    public void SetRole(NodeRole newRole)
    {
        GetMain().Role = newRole;
    }

    private GameState.Types.Snake SearchSnake(List<GameState.Types.Snake> list, GameState.Types.Coord coord)
    {
        foreach(var snake in list)
        {
            if (CoordUtils.EqualCoord(snake.Points[0], coord))
            {
                return snake;
            }
        }
        throw new Exception();
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

    private void Die(GameState.Types.Snake snake)
    {
        DeletePlayer(snake.PlayerId);
        _state.Snakes.Remove(snake);
    }

    private void DeletePlayer(int gameId)
    {
        // if we is master and we delete our
        if (gameId == _mainId && _mainId == GetMaster().Id)
        {
            if (GetDeputy() is GamePlayer deputy)
            {
                Update(new ModelEvent(ModelAction.SendRoleMsgViewerMaster, deputy.Id));
                SetOtherRole(gameId, NodeRole.Master);
            }
            _isRun = false;
            Update(new ModelEvent(ModelAction.Stop, 0));
        }
        var player = GetPlayer(gameId);
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

    private void SearchDeputy()
    {
        // if not found deputy
        if (GetDeputy() is null)
        {
            // if we have normal
            if (GetPlayer(NodeRole.Normal) is GamePlayer player)
            {
                player.Role = NodeRole.Deputy;
            }
        }
    }
}