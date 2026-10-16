namespace TeamDeathmatch.Lite.Features.Extensions
{
    using InventorySystem.Items.Armor;
    using LabApi.Features.Wrappers;
    using PlayerRoles;
    using SecretLabNAudio.Core;
    using SecretLabNAudio.Core.Extensions;
    using SecretLabNAudio.Core.Pools;

    public static class PlayerExtensions
    {
        public static SpeakerSettings ProximitySettings = new()
        {
            Volume = 2f,
            MinDistance = 1f,
            MaxDistance = 10f
        };

        public static void PlayHeadshotSound(this Player player, Player attacker)
        {
            var clipName = "Sounds.Player.headshot_flesh";
            if (player.RoleBase is HumanRole role && player.Inventory.TryGetBodyArmor(out var bodyArmor) && bodyArmor.HelmetEfficacy > 0)
            {
                clipName = "Sounds.Player.headshot_kevlar";
            }

            AudioPlayerPool.RentGloballyAudible()
                .WithVolume(1f)
                .WithSendEngine(new SecretLabNAudio.Core.SendEngines.SpecificPlayerSendEngine(attacker))
                .UseShortClip((clipName, false))
                .PoolOnEnd();

            AudioPlayerPool.Rent(ProximitySettings)
                .WithFilteredSendEngine(x => x != attacker)
                .UseShortClip((clipName, false))
                .PoolOnEnd();
        }
    }
}
