using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace TurretWhacker;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
//[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
//[LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
public class TurretWhacker : BaseUnityPlugin
{
    public static TurretWhacker Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogInfo("Patching...");

        Harmony.CreateAndPatchAll(typeof(Patches.TurretPatch));

        Logger.LogInfo("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogInfo("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogInfo("Finished unpatching!");
    }


}
