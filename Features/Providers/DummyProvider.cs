namespace TeamDeathmatch.Lite.Features.Providers
{
    using LabApi.Features.Wrappers;
    using NetworkManagerUtils.Dummies;
    using UnityEngine;


    public class DummyProvider : MonoBehaviour, IRootDummyActionProvider
    {
        public bool DummyActionsDirty { get; set; }

        public void PopulateDummyActions(Action<DummyAction> actionAdder, Action<string> categoryAdder)
        {
            DummyActionsDirty = false;
            
            categoryAdder("TeamDeathmatch.Commands");
            PopulateCommandsAction(actionAdder);
            categoryAdder("TeamDeathmatch.Loadouts");
            PopulateLoadoutActions(actionAdder);
            categoryAdder("TeamDeathmatch.Respawning");
            PopulateRespawningActions(actionAdder);
        }

        public static void PopulateCommandsAction(Action<DummyAction> actionAdder)
        {
            // Enabling tools
            actionAdder(new DummyAction("GameMode::Enable", GameMode.Enable));
            
            // Disable tools
            actionAdder(new DummyAction("GameMode::Disable", GameMode.Disable));

            // Map Cleanup
            actionAdder(new DummyAction("Cleanup Map", GameMode.MapCleanupAll));
        }

        public static void PopulateLoadoutActions(Action<DummyAction> actionAdder)
        {

            // Give Primaries

            actionAdder(new DummyAction("Give SMGs [FSP9 & CrossVec]", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var item = player.AddItem(GameMode.GetFactionWeaponSMGs(GameMode.Players[player]));
                    if (item is FirearmItem firearm)
                    {
                        player.AddAmmo(firearm.AmmoType, (ushort)(firearm.MaxAmmo * 3));
                    }
                }
            }));

            actionAdder(new DummyAction("Give Rifles [AK & E11SR]", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var item = player.AddItem(GameMode.GetFactionWeaponRifles(GameMode.Players[player]));
                    if (item is FirearmItem firearm)
                    {
                        player.AddAmmo(firearm.AmmoType, (ushort)(firearm.MaxAmmo * 3));
                    }
                }
            }));

            actionAdder(new DummyAction("Give Shotguns", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var item = player.AddItem(ItemType.GunShotgun);
                    if (item is FirearmItem firearm)
                    {
                        player.AddAmmo(firearm.AmmoType, (ushort)(30));
                    }
                }
            }));

            actionAdder(new DummyAction("Give Revolvers", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var item = player.AddItem(ItemType.GunRevolver);
                    if (item is FirearmItem firearm)
                    {
                        player.AddAmmo(firearm.AmmoType, (ushort)(18));
                    }
                }
            }));

            // Remove Guns
            actionAdder(new DummyAction("Remove All Primaries", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var primary_items = player.Items
                    .Where(x => x is FirearmItem)
                    .Where(x =>
                        // Is Rifle
                        (x.Type == ItemType.GunAK || x.Type == ItemType.GunE11SR) ||
                        // Is SMGs
                        (x.Type == ItemType.GunFSP9 || x.Type == ItemType.GunCrossvec) ||
                        // Is Shotguns
                        (x.Type == ItemType.GunShotgun)
                    ).ToArray();

                    primary_items.ForEach((x) => player.RemoveItem(x));
                }
            }));

            actionAdder(new DummyAction("Remove All Secondaries", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var primary_items = player.Items
                    .Where(x => x is FirearmItem)
                    .Where(x =>
                        (x.Type == ItemType.GunCOM15 || x.Type == ItemType.GunCOM18) ||
                        x.Type == ItemType.GunRevolver
                    ).ToArray();

                    primary_items.ForEach((x) => player.RemoveItem(x));
                }

                
            }));

            // Armors
            actionAdder(new DummyAction("Use Light Armor", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var items = player.Items.Where(x => x is BodyArmorItem).ToArray();
                    items.ForEach(x =>  player.RemoveItem(x));
                    player.AddItem(ItemType.ArmorLight);
                }
            }));

            actionAdder(new DummyAction("Use Combat Armor", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var items = player.Items.Where(x => x is BodyArmorItem).ToArray();
                    items.ForEach(x => player.RemoveItem(x));
                    player.AddItem(ItemType.ArmorCombat);
                }
            }));

            actionAdder(new DummyAction("Use Heavy Armor", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var items = player.Items.Where(x => x is BodyArmorItem).ToArray();
                    items.ForEach(x => player.RemoveItem(x));
                    player.AddItem(ItemType.ArmorHeavy);
                }
            }));

            actionAdder(new DummyAction("Remove Body Armor", () =>
            {
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    var items = player.Items.Where(x => x is BodyArmorItem).ToArray();
                    items.ForEach(x => player.RemoveItem(x));
                }
            }));
        }

        public static void PopulateRespawningActions(Action<DummyAction> actionAdder)
        {
            // Assigning Teams
            actionAdder(new DummyAction("AssignAll", GameMode.AssignAll));
            actionAdder(new DummyAction("Rebalance", GameMode.Rebalance));

            // Role Spawning
            actionAdder(new DummyAction("Spawn Teams [ClassDs & Scientists]", () =>
            {
                GameMode.RemoveInvalids();
                foreach (var player in GameMode.Players.Keys.ToArray())
                {
                    if (player.IsAlive) player.ClearInventory();
                    var role = GameMode.GetFactionRoleLCZ(GameMode.Players[player]);
                    player.SetRole(role, flags: PlayerRoles.RoleSpawnFlags.All);
                }
            }));

            actionAdder(new DummyAction("Spawn Teams [Chaos & MTFs]", () =>
            {
                GameMode.RemoveInvalids();
                foreach (var player in GameMode.Players.Keys)
                {
                    if (player.IsAlive) player.ClearInventory();
                    var role = GameMode.GetFactionRole(GameMode.Players[player]);
                    player.SetRole(role, flags: PlayerRoles.RoleSpawnFlags.All);
                }
            }));
        }
    }
}
