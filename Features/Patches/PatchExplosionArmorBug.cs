namespace TeamDeathmatch.Lite.Features.Patches
{
    using Footprinting;
    using HarmonyLib;
    using InventorySystem.Items.Armor;
    using InventorySystem.Items.ThrowableProjectiles;
    using PlayerRoles;
    using PlayerStatsSystem;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEngine;

    [HarmonyPatch]
    [HarmonyPatchCategory("Patch")]
    public class PatchExplosionArmorBug
    {

        // How is this still not resolved NW
        public static readonly int SpicyArmorPenetration = 100;
        public static int GrenadeArmorPenetration => GameMode.IsActive ? (int)(Plugins.Plugin.Instance.Config.GrenadeArmorPenetration * 100) : 50;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ExplosionDamageHandler), MethodType.Constructor, [ typeof(Footprint), typeof(Vector3), typeof(float), typeof(int), typeof(ExplosionType) ])]
        public static void Postfix(ExplosionDamageHandler __instance, float damage)
        {
            __instance.Damage = damage;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExplosionDamageHandler), nameof(ExplosionDamageHandler.ApplyDamage))]
        public static void Prefix(ExplosionDamageHandler __instance, ReferenceHub ply)
        {
            if (ply.roleManager.CurrentRole is IArmoredRole && BodyArmorUtils.TryGetBodyArmor(ply.inventory, out var bodyArmor))
            {
                // Currently in codebase only exists two, Spicy(Custom) and Grenade
                var num = __instance.ExplosionType == ExplosionType.Custom ? SpicyArmorPenetration : GrenadeArmorPenetration;
                __instance.Damage = BodyArmorUtils.ProcessDamage(bodyArmor.VestEfficacy, __instance.Damage, num);
            }
        }
    }
}
