namespace TeamDeathmatch.Lite.Features.Patches
{
    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;
    using LabApi.Features.Console;
    using System.Reflection.Emit;

    [HarmonyPatchCategory("Patch")]
    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.ServerFuseEnd))]
    public class FlashProjectileOwner
    {
        private static bool AlwaysFalse(ReferenceHub _, ReferenceHub __) => false;
        private static bool AlwaysTrue(PlayerRoles.RoleTypeId _, PlayerRoles.RoleTypeId __) => true;

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var opEq = AccessTools.Method(typeof(ReferenceHub), "op_Equality", [typeof(ReferenceHub), typeof(ReferenceHub)]);

            return new CodeMatcher(instructions, generator)
                // allHubs == PreviousOwner
                .MatchStartForward(new CodeMatch(OpCodes.Call, opEq))
                .ThrowIfInvalid("Patch unable to find op_Equality of REFHUB")
                .RemoveInstruction()
                .Insert(CodeMatch.Call(() => AlwaysFalse(default!, default!)))

                // HitboxIdentity.IsDamageable(RoleTypeId, RoleTypeId)
                .MatchStartForward(CodeMatch.Calls(() => HitboxIdentity.IsDamageable(default(PlayerRoles.RoleTypeId), default)))
                .ThrowIfInvalid("Patch unable to find HitboxIdentity::IsDamageable")
                .RemoveInstruction()
                .Insert(CodeMatch.Call(() => AlwaysTrue(default, default)))
                .Instructions();
        }
    }
}
