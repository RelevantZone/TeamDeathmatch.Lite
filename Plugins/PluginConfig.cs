namespace TeamDeathmatch.Lite.Plugins
{
    using InventorySystem.Items.Firearms.Attachments;
    using System.ComponentModel;
    using UnityEngine;

    public class PluginConfig
    {
        #region Permissions

        #endregion

        #region
        public float FlashbangFuseTime { get; set; } = 1.6f;

        public float FlashbangBlindingTime { get; set; } = 1.8f;
        #endregion

        #region Grenades

        public float GrenadeFuseTime { get; set; } = 1.88f;
        public float GrenadeDamageMultiplier { get; set; } = 0.22f;

        [Range(0f, 1f)]
        public float GrenadeArmorPenetration { get; set; } = 0.5f;

        #endregion

        #region Firearm Statistics

        public HashSet<FirearmStats> FirearmStats { get; set; } = [
            new() {
                Type = ItemType.GunCOM15,
                BaseDamage = 35f,
                ArmorPenetration = 0.5f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f
                }
            },
            new() {
                Type = ItemType.GunCOM18,
                BaseDamage = 30f,
                ArmorPenetration = 0.5f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f
                }
            },
            new() {
                Type = ItemType.GunRevolver,
                BaseDamage = 86f,
                ArmorPenetration = 0.93f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f,
                    [HitboxType.Body] = 0.8f
                }
            },
            new() {
                Type = ItemType.GunFSP9,
                BaseDamage = 30f,
                ArmorPenetration = 0.625f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f,
                    [HitboxType.Body] = 1.2f
                }
            },
            new() {
                Type = ItemType.GunCrossvec,
                BaseDamage = 28f,
                ArmorPenetration = 0.625f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f,
                    [HitboxType.Body] = 1.1f
                }
            },
            new() {
                Type = ItemType.GunAK,
                BaseDamage = 37f,
                ArmorPenetration = 0.8f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f,
                    [HitboxType.Body] = 1.4f
                }
            },
            new() {
                Type = ItemType.GunE11SR,
                BaseDamage = 32f,
                ArmorPenetration = 0.75f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 4f,
                    [HitboxType.Body] = 1.5f
                }
            },
            new() {
                Type = ItemType.GunFRMG0,
                BaseDamage = 20f,
                ArmorPenetration = 0.42f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 3f,
                    [HitboxType.Body] = 1.4f
                }
            },
            new() {
                Type = ItemType.GunLogicer,
                BaseDamage = 23f,
                ArmorPenetration = 0.52f,
                HitboxMultiplier = new() {
                    [HitboxType.Headshot] = 3f,
                    [HitboxType.Body] = 1.4f
                }
            }
        ];

        #endregion

        #region Default Loadouts
        public Dictionary<PlayerRoles.Faction, Item[]> Loadouts { get; set; } = new()
        {
            [PlayerRoles.Faction.FoundationStaff] = [
                new() { Type = ItemType.ArmorLight },
                new Firearm() {
                    Type = ItemType.GunCOM15,
                    Attachments = [AttachmentName.SoundSuppressor]
                }
            ],
            [PlayerRoles.Faction.FoundationEnemy] = [
                new() { Type = ItemType.ArmorLight },
                new Firearm() {
                    Type = ItemType.GunCOM18,
                    Attachments = []
                }
            ]
        };
        #endregion
    }

    public class Item
    {
        public required ItemType Type { get; set; }
    }

    public class Firearm : Item
    {
        public required AttachmentName[] Attachments { get; set; }
    }

    public class FirearmStats : Item
    {
        public float BaseDamage { get; set; }
        [Range(0, 1)]
        public float ArmorPenetration { get; set; } = 0.5f;
        public Dictionary<HitboxType, float> HitboxMultiplier { get; set; } = [];
    }
}
