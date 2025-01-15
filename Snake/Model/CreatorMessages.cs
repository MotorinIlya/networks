using Snake.Net;

namespace Snake.Model;

public static class CreatorMessages
{
    private static long _msgSeq = 0;
    public static GameMessage CreatePingMsg()
    {
        var pingMsg = new GameMessage.Types.PingMsg();
        var msg = new GameMessage()
        {
            Ping = pingMsg,
            MsgSeq = _msgSeq
        };
        _msgSeq++;
        return msg;
    }

    public static GameMessage CreateRoleChangeMsg(NodeRole sender, 
                                                    NodeRole receiver, 
                                                    int senderId, 
                                                    int receiverId)
    {
        var roleChangeMsg = new GameMessage.Types.RoleChangeMsg
        {
            SenderRole = sender,
            ReceiverRole = receiver
        };
        var msg = new GameMessage()
        {
            RoleChange = roleChangeMsg,
            MsgSeq = _msgSeq,
            SenderId = senderId,
            ReceiverId = receiverId
        };
        _msgSeq++;
        return msg;
    }

    public static GameMessage CreateRoleChangeMsg(MConst.RoleChange roleChange, 
                                                    NodeRole nodeRole, 
                                                    int senderId, 
                                                    int receiverId)
    {
        GameMessage.Types.RoleChangeMsg roleChangeMsg;
        if (roleChange == MConst.RoleChange.Sender)
        {
            roleChangeMsg = new()
            {
                SenderRole = nodeRole
            };
        }
        else
        {
            roleChangeMsg = new()
            {
                ReceiverRole = nodeRole
            };
        }
        var msg = new GameMessage()
        {
            RoleChange = roleChangeMsg,
            MsgSeq = _msgSeq,
            SenderId = senderId,
            ReceiverId = receiverId
        };
        _msgSeq++;
        return msg;
    }

    public static void CreateForAllRoleChangeMsg(Peer peer, GameModel model, int senderId, int deputyId)
    {
        foreach (var player in model.Players.Players)
        {
            if (player.Id == deputyId)
            {
                var msg = CreateRoleChangeMsg(NodeRole.Master, NodeRole.Deputy, senderId, deputyId);
                peer.AddMsg(msg, model.GetEndPoint(player.Id));
            }
            else
            {
            var msg = CreateRoleChangeMsg(MConst.RoleChange.Sender, NodeRole.Master, senderId, player.Id);
            peer.AddMsg(msg, model.GetEndPoint(player.Id));
            }
        }
    }
    
    public static GameMessage CreateSteerMsg(Direction direction)
    {
        var steerMsg = new GameMessage.Types.SteerMsg
        {
            Direction = direction
        };
        var msg = new GameMessage()
        {
            Steer = steerMsg,
            MsgSeq = _msgSeq
        };
        _msgSeq++;
        return msg;
    }
    
    public static GameMessage CreateStateMsg(GameModel model)
    {
        var gameState = model.GetState();
        var stateMsg = new GameMessage.Types.StateMsg
        {
            State = gameState,
        };
        var msg = new GameMessage()
        {
            State = stateMsg,
            MsgSeq = _msgSeq
        };
        _msgSeq++;
        return msg;
    }
    
    public static GameMessage CreateErrorMsg(string error)
    {
        var errorMsg = new GameMessage.Types.ErrorMsg
        {
            ErrorMessage = error
        };
        var msg = new GameMessage()
        {
            Error = errorMsg,
            MsgSeq = _msgSeq
        };
        _msgSeq++;
        return msg;
    }
    
    public static GameMessage CreateJoinMsg(string player_name, string game_name, NodeRole role)
    {
        var joinMsg = new GameMessage.Types.JoinMsg
        {
            GameName = game_name,
            PlayerName = player_name,
            RequestedRole = role
        };
        var msg = new GameMessage()
        {
            Join = joinMsg,
            MsgSeq = _msgSeq
        };
        _msgSeq++;
        return msg;
    }

    public static GameMessage CreateAckMsg(int senderId, int receiverId, long msgSeq)
    {
        var msg = CreateAckMsg(senderId, msgSeq);
        msg.ReceiverId = receiverId;
        return msg;
    }

    public static GameMessage CreateAckMsg(int senderId, long msgSeq)
    {
        var ackMsg = new GameMessage.Types.AckMsg();
        var msg = new GameMessage()
        {
            Ack = ackMsg,
            SenderId = senderId,
            MsgSeq = msgSeq
        };
        return msg;
    }

    public static GameMessage CreateAnnouncementMsg(GameModel model)
    {
        var annMsg = new GameMessage.Types.AnnouncementMsg();
        annMsg.Games.Add(createGameAnnouncement(model));

        var msg = new GameMessage()
        {
            Announcement = annMsg
        };
        return msg;
    }

    private static GameAnnouncement createGameAnnouncement(GameModel model)
    {
        var gamePlayers = model.Players;
        return new GameAnnouncement()
        {
            Players = gamePlayers,
            Config = model.Config,
            GameName = model.GameName
        };
    }
}