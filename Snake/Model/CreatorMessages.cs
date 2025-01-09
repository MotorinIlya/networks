using Snake.Net;

namespace Snake.Model;

public static class CreatorMessages
{
    public static GameMessage createAnnouncementMsg(GameModel model)
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