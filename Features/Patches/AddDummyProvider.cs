namespace TeamDeathmatch.Lite.Features.Patches
{
    using HarmonyLib;

    [HarmonyPatch(typeof(NetworkManagerUtils.Dummies.DummyUtils), nameof(NetworkManagerUtils.Dummies.DummyUtils.SpawnDummy))]
    public class AddDummyProvider
    {
        [HarmonyPostfix]
        public static void Postfix(ReferenceHub __result)
        {
            if (__result.nicknameSync.DisplayName == "TDM")
            __result.gameObject.AddComponent<Providers.DummyProvider>();
        }
    }
}
