namespace TeamDeathmatch.Lite.Events
{
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Events.CustomHandlers;
    using NetworkManagerUtils.Dummies;
    using TeamDeathmatch.Lite.Features;

    public class ServerEvents : CustomEventsHandler
    {
        public override void OnServerRoundStarting(RoundStartingEventArgs ev)
        {
            //GameMode.Enable();
        }
        public override void OnServerWaitingForPlayers()
        {
            GameMode.Disable();
        }
    }
}
