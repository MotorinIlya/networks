namespace Snake.Service.Event;


public enum ModelAction
{
    Stop,
    SendRoleMsgViewerMaster,
    SendRoleMsgRecvDeputy,
    SendRoleMsgSendMaster,
    UpdateStatistics,
    UpdateLastInteraction,
    DeleteInactivePlayer,
}