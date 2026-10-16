

namespace TeamDeathmatch.Lite.Features
{
    using CommandSystem.Commands.RemoteAdmin.Cleanup;
    using Decals;
    using HarmonyLib;
    using LabApi.Events.CustomHandlers;
    using LabApi.Features.Wrappers;
    using NetworkManagerUtils.Dummies;
    using System.Reflection;
    using UnityEngine;
    using Utils.Networking;
    using Logger = LabApi.Features.Console.Logger;
    public static class GameMode
    {
        public static bool IsActive { get; private set; } = false;
        public static int MaxImbalance { get; private set; } = 2;
        public static readonly Dictionary<Player, PlayerRoles.Faction> Players = [];
        public static Harmony Patcher { get; internal set; } = new($"{Assembly.GetExecutingAssembly()}-{Guid.NewGuid()}");
        public static IEnumerable<Player> ReadyList => Player.ReadyList.Where(x => !x.IsDummy);

        public static void Enable()
        {
            Patcher.PatchCategory("Patch");

            CustomHandlersManager.RegisterEventsHandler(Plugins.Plugin.PlayerEvents);
            IsActive = true;
        }

        public static void Disable()
        {
            Players.Clear();
            Patcher.UnpatchCategory("Patch");

            CustomHandlersManager.UnregisterEventsHandler(Plugins.Plugin.PlayerEvents);
            IsActive = false;
        }


        #region Assigning Teams

        public static int CountOf(PlayerRoles.Faction faction) => Players.Values.Count(x => x == faction);
        public static int CountAlive(PlayerRoles.Faction faction) => Players.Keys.Count(x => x.IsAlive && x.Faction == faction);

        public static int CountDiff() => CountOf(PlayerRoles.Faction.FoundationStaff) - CountOf(PlayerRoles.Faction.FoundationEnemy);

        public static void SetFaction(Player player, PlayerRoles.Faction faction) => Players[player] = faction;
        public static void TryAssignFaction(Player player) =>
            SetFaction(player, CountDiff() >= 0 ? PlayerRoles.Faction.FoundationStaff : PlayerRoles.Faction.FoundationEnemy);

        public static PlayerRoles.Faction GetEnemyFaction(PlayerRoles.Faction f) => f switch
        {
            PlayerRoles.Faction.FoundationEnemy => PlayerRoles.Faction.FoundationStaff,
            PlayerRoles.Faction.FoundationStaff => PlayerRoles.Faction.FoundationEnemy,
            _ => default
        };

        public static PlayerRoles.RoleTypeId GetFactionRole(PlayerRoles.Faction f) => f switch
        {
            PlayerRoles.Faction.FoundationEnemy => PlayerRoles.RoleTypeId.ChaosConscript,
            PlayerRoles.Faction.FoundationStaff => PlayerRoles.RoleTypeId.NtfSergeant,
            _ => default
        };

        public static PlayerRoles.RoleTypeId GetFactionRoleLCZ(PlayerRoles.Faction f) => f switch
        {
            PlayerRoles.Faction.FoundationEnemy => PlayerRoles.RoleTypeId.ClassD,
            PlayerRoles.Faction.FoundationStaff => PlayerRoles.RoleTypeId.Scientist,
            _ => default
        };

        public static ItemType GetFactionWeaponRifles(PlayerRoles.Faction f) => f switch
        {
            PlayerRoles.Faction.FoundationEnemy => ItemType.GunAK,
            PlayerRoles.Faction.FoundationStaff => ItemType.GunE11SR,
            _ => default
        };

        public static ItemType GetFactionWeaponSMGs(PlayerRoles.Faction f) => f switch
        {
            PlayerRoles.Faction.FoundationEnemy => ItemType.GunFSP9,
            PlayerRoles.Faction.FoundationStaff => ItemType.GunCrossvec,
            _ => default
        };

        public static void AssignAll()
        {
            Players.Clear();
            var shuffled = ReadyList.OrderBy(_ => Random.value).ToList();
            int team_first_max = (shuffled.Count + 1) / 2;
            var faction1 = Random.value > 0.5f ? PlayerRoles.Faction.FoundationStaff : PlayerRoles.Faction.FoundationEnemy;
            var faction2 = GetEnemyFaction(faction1);

            for (var i = 0; i < shuffled.Count; i++)
            {
                SetFaction(shuffled.ElementAt(i), i < team_first_max ? faction1 : faction2);
            }
            return;
        }

        public static void Rebalance()
        {
            if (!ReadyList.Any()) return;
            var diff = CountOf(PlayerRoles.Faction.FoundationStaff) - CountOf(PlayerRoles.Faction.FoundationEnemy);
            if (Math.Abs(diff) <= MaxImbalance) return;

            var from = diff >= 0 ? PlayerRoles.Faction.FoundationStaff : PlayerRoles.Faction.FoundationEnemy;
            var to = GetEnemyFaction(from);
            var players = Players.AsEnumerable()
                .Where(x => x.Value == from)
                .OrderBy(_ => Random.value)
                .Select(x => x.Key)
                .ToList();

            foreach (var player in players)
            {
                Players[player] = to;
            }
        }

        public static void RemoveInvalids()
        {
            if (Players.Count == 0) return;

            var ready = new HashSet<Player>(Player.ReadyList);
            foreach (var player in Players.Keys.Where(p => !ready.Contains(p)).ToList())
            {
                Players.Remove(player);
            }
        }
        #endregion

        #region Map Cleanup

        public static void MapCleanDecal(DecalPoolType decalType, int amount) => new DecalCleanupMessage(decalType, amount).SendToAuthenticated();
        public static void MapCleanDecal(DecalPoolType decalType) => MapCleanDecal(decalType, int.MaxValue);
        public static void MapCleanDecals() => EnumUtils<DecalPoolType>.Values.Except([DecalPoolType.None])
            .ToArray().ForEach(MapCleanDecal);

        public static void MapCleanRagdolls() => Ragdoll.List.ToArray().ForEach(x => x.Destroy());
        public static void MapCleanItems() => Pickup.List.ToArray().ForEach(x => x.Destroy());

        public static void MapCleanupAll()
        {
            MapCleanItems();
            MapCleanRagdolls();
            MapCleanDecals();
        }

        #endregion
    }
}
