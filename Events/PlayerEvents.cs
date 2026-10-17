namespace TeamDeathmatch.Lite.Events
{
    using HarmonyLib;
    using InventorySystem.Items;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Firearms.Modules;
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Events.Arguments.PlayerEvents;
    using LabApi.Events.Arguments.ServerEvents;
    using LabApi.Events.CustomHandlers;
    using LabApi.Features.Console;
    using LabApi.Features.Wrappers;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerStatsSystem;
    using TeamDeathmatch.Lite.Features;
    using TeamDeathmatch.Lite.Features.Extensions;
    using Utils.NonAllocLINQ;

    public class PlayerEvents : CustomEventsHandler
    {
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            if (!GameMode.Players.ContainsKey(ev.Player)) GameMode.TryAssignFaction(ev.Player);
        }

        
        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker == null) return;
            if (ev.DamageHandler is ExplosionDamageHandler exp)
            {
                exp.Damage *= Plugins.Plugin.Instance.Config.GrenadeDamageMultiplier;
                return;
            }

            if (ev.DamageHandler is not FirearmDamageHandler handler) return;
            if (!FirearmDamageHandler.HitboxDamageMultipliers.TryGetValue(handler.Hitbox, out float num1)) num1 = 1f;
            if (!Plugins.Plugin.Instance.Config.FirearmStats.TryGetFirst(x => x.Type == handler.Firearm.ItemTypeId, out Plugins.FirearmStats stats))
            {
                stats = new()
                {
                    Type = default,
                    BaseDamage = default,
                    // HurtingEvent haven't processed the damage
                    // Its processed after the event
                    ArmorPenetration = handler._penetration
                };
            }

            if (!stats.HitboxMultiplier.TryGetValue(handler.Hitbox, out float num2)) num2 = num1;

            if (ev.Player is IArmoredRole role && ev.Player.Items.First(x => x is BodyArmorItem) is BodyArmorItem armor)
            {
                AccessTools.FieldRefAccess<FirearmDamageHandler, float>(handler, nameof(FirearmDamageHandler._penetration)) = stats.ArmorPenetration;
            }
            Logger.Info($"DamageBefore modified: {handler.Damage} [{num1}x; {num2}x]");

            if (stats.BaseDamage != 0) handler.Damage = stats.BaseDamage * (num2 / num1);
            else handler.Damage *= (num2 / num1);

            Logger.Info($"DamageAfter modified: {handler.Damage} [{num1}x; {num2}x]");
            if (handler.Hitbox == HitboxType.Headshot) ev.Player.PlayHeadshotSound(ev.Attacker);
        }

        #region Items

        public override void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev)
        {
            var all = ev.SpawnFlags == RoleSpawnFlags.All;
            var canAssign = all || ev.SpawnFlags == RoleSpawnFlags.AssignInventory;
            //var canSetPos = all || ev.SpawnFlags == RoleSpawnFlags.UseSpawnpoint;

            if (canAssign && Plugins.Plugin.Instance.Config.Loadouts.TryGetValue(ev.NewRole.GetFaction(), out var items))
            {
                foreach (var baseItem in items)
                {
                    var item = ev.Player.AddItem(baseItem.Type);
                    if (item is FirearmItem firearm)
                    {
                        ev.Player.AddAmmo(firearm.AmmoType, (ushort)(firearm.MaxAmmo * 3));
                        if (baseItem is Plugins.Firearm info)
                        {
                            firearm.AttachmentsCode = firearm.ValidateAttachmentsCode(info.Attachments);
                        }
                    }
                }

                if (all) ev.SpawnFlags = RoleSpawnFlags.UseSpawnpoint;
            }
        }

        public override void OnPlayerChangingItem(PlayerChangingItemEventArgs ev)
        {
            if (ev.NewItem is not FirearmItem firearm) return;
            if (firearm is ParticleDisruptorItem) return;
            if (firearm is RevolverFirearm || firearm is Scp127Firearm)
            {
                if (!firearm.Modules.ToList().TryGetFirst(x => x is EventBasedEquipperModule, out var module))
                {
                    Logger.Info($"No module found when using {firearm.Type.GetName()}");
                    return;
                }

                module.SendRpc(delegate (NetworkWriter x)
                {
                    x.WriteSubheader(EventBasedEquipperModule.RpcType.FirstTimeTrue);
                    x.WriteUShort(firearm.Serial);
                });
                return;
            }

            // Always recock gun
            firearm.StoredAmmo = firearm.ChamberedAmmo + firearm.StoredAmmo;
            firearm.ChamberedAmmo = 0;
            firearm.Cocked = false;
        }

        public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
        {
            if (ev.FirearmItem is RevolverFirearm || ev.FirearmItem is ParticleDisruptorItem || ev.FirearmItem is Scp127Firearm) return;
            if (ev.FirearmItem.ChamberedAmmo + ev.FirearmItem.StoredAmmo >= ev.FirearmItem.MaxAmmo)
            {
                ev.IsAllowed = false;
                return;
            }
            if (ev.Player.GetAmmo(ev.FirearmItem.AmmoType) <= 0)
            {
                ev.IsAllowed = false;
                return;
            }
            ev.FirearmItem.StoredAmmo += ev.FirearmItem.ChamberedAmmo;
            ev.FirearmItem.ChamberedAmmo = 0;
            ev.FirearmItem.Cocked = false;
        }


        public override void OnPlayerCancelledUsingItem(PlayerCancelledUsingItemEventArgs ev)
        {
            if (ev.UsableItem.CurrentOwner != ev.Player) return;
            ev.Player.CurrentItem = null;
            Timing.CallDelayed(Timing.WaitForOneFrame, () => ev.Player.CurrentItem = ev.UsableItem);
        }

        #endregion

        #region Grenades

        public override void OnPlayerThrewProjectile(PlayerThrewProjectileEventArgs ev)
        {
            if (ev.Projectile is not TimedGrenadeProjectile p) return;

            var fuseTime =
                p.Base is ExplosionGrenade ? Plugins.Plugin.Instance.Config.GrenadeFuseTime :
                p.Base is FlashbangGrenade ? Plugins.Plugin.Instance.Config.FlashbangFuseTime : 2f;

            p.Base.Network_syncTargetTime = 0;
            Timing.CallDelayed(fuseTime, () => p.Base.ServerFuseEnd());
        }

        public override void OnServerDoorDamaging(DoorDamagingEventArgs ev)
        {
            if (ev.DamageType != Interactables.Interobjects.DoorUtils.DoorDamageType.Grenade) return;
            if (ev.Door.Permissions == Interactables.Interobjects.DoorUtils.DoorPermissionFlags.None) return;
            ev.IsAllowed = false;
        }

        #endregion
    }
}
