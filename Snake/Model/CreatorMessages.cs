using Snake.Net;

namespace Snake.Model;

public static class CreatorMessages
{
    public static GameMessage CreateAckMsg()
    {
        return new GameMessage()
        {
            Ack = new GameMessage.Types.AckMsg()
        };
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