using Snake.Net;

namespace Snake.Model;

public static class CreatorMessages
{

    public static GameMessage CreateStateMsg(GameModel model)
    {
        var gameState = model.GetState();
        var stateMsg = new GameMessage.Types.StateMsg
        {
            State = gameState
        };
        var msg = new GameMessage()
        {
            State = stateMsg
        };
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
            Error = errorMsg
        };
        return msg;
    }
    
    public static GameMessage CreateJoinMsg(string player_name, string game_name)
    {
        var joinMsg = new GameMessage.Types.JoinMsg
        {
            GameName = game_name,
            PlayerName = player_name,
            RequestedRole = NodeRole.Normal
        };
        var msg = new GameMessage()
        {
            Join = joinMsg
        };
        return msg;
    }

    public static GameMessage CreateAckMsg(int receiverId)
    {
        var ackMsg = new GameMessage.Types.AckMsg();
        var msg = new GameMessage()
        {
            Ack = ackMsg,
            ReceiverId = receiverId
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