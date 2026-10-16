namespace TeamDeathmatch.Lite.Plugins
{
    using LabApi.Events.CustomHandlers;
    using LabApi.Features;
    using LabApi.Features.Wrappers;
    using LabApi.Loader.Features.Plugins;
    using SecretLabNAudio.Core.FileReading;
    using System.Reflection;

    public class Plugin : Plugin<PluginConfig>
    {
        public static Plugin Instance { get; private set; }
        public override string Name { get; } = "LatteMods.TeamDeathmatch.Lite";
        public override string Description { get; } = "Fun game";
        public override string Author { get; } = "RelevantZone";
        public override Version Version => new(1,0,1);
        public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public Plugin() => Instance = this;

        public static Events.ServerEvents ServerEvents { get; private set; } = new();
        public static Events.PlayerEvents PlayerEvents { get; private set; } = new();

        public override void Enable()
        {
            Features.GameMode.Patcher.PatchAllUncategorized();
            ShortClipCache.AddAllFromEmbeddedResources(Assembly.GetExecutingAssembly());
            CustomHandlersManager.RegisterEventsHandler(ServerEvents);
        }

        public override void Disable()
        {
            Features.GameMode.Patcher.UnpatchAll();
            CustomHandlersManager.UnregisterEventsHandler(ServerEvents);
        }
    }
}
