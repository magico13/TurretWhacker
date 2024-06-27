using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Diagnostics.CodeAnalysis;
using TurretWhacker.Config;

namespace TurretWhacker;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class TurretWhacker : BaseUnityPlugin
{
    public static TurretWhacker Instance { get; private set; } = null!;
    internal static PluginConfig PluginConfig { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Invoked by Unity")]
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;
        PluginConfig = new(Config);

        Patch();

        Logger.LogDebug($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.CreateAndPatchAll(typeof(Patches.TurretPatch));

        Logger.LogDebug("Finished patching!");
    }
}
