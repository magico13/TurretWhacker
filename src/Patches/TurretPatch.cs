using HarmonyLib;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

namespace TurretWhacker.Patches;
public class TurretPatch
{
    [HarmonyPatch(typeof(Turret), "IHittable.Hit")]
    [HarmonyPrefix]
    public static bool Hit(Turret __instance, ref bool __result)
    {
        // if the turret's mode is beserk, behave as normal
        if (__instance.turretMode == TurretMode.Berserk || !__instance.turretActive)
        {
            return true;
        }

        var random = UnityEngine.Random.Range(0f, 1f);

        var noEffectChance = TurretWhacker.PluginConfig.NoEffectChance.Value;
        var successChance = TurretWhacker.PluginConfig.SuccessChance.Value;
        var criticalSuccessChance = TurretWhacker.PluginConfig.CriticalSuccessChance.Value;

        var terminalObject = __instance.GetComponent<TerminalAccessibleObject>();
        var idInTerminal = terminalObject.objectCode;

        var currentPercent = 0f;

        if (random < (currentPercent += noEffectChance))
        {
            // chance that nothing at all happens
            TurretWhacker.Logger.LogDebug($"Turret {idInTerminal} has been hit! Random value: {random}. No effect.");
        }
        else if (random < (currentPercent += successChance))
        {
            // disable the turret, as per the terminal
            TurretWhacker.Logger.LogDebug($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is being disabled.");
            __instance.StartCoroutine(DisableTurretFromTerminal(__instance, terminalObject));
        }
        else if (random < (currentPercent += criticalSuccessChance))
        {
            // permanently disable the turret
            TurretWhacker.Logger.LogDebug($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is being permanently disabled.");

            // start a coroutine to disable it next frame
            // this should allow the Update to run and allow it to fully shut down
            __instance.StartCoroutine(DisableTurretPermanently(__instance));
        }
        else
        {
            // otherwise act as normal
            TurretWhacker.Logger.LogDebug($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is beserk.");
            return true;
        }

        // Warning: Setting false will stop the original method (good) but also interfere with other patches (bad)
        __result = true;
        return false;
    }

    public static IEnumerator DisableTurretFromTerminal(Turret turret, TerminalAccessibleObject terminalObject)
    {
        turret.SwitchTurretMode(0);
        turret.SetToModeClientRpc(0);
        // wait a frame
        yield return 0;
        terminalObject.CallFunctionFromTerminal();
    }

    public static IEnumerator DisableTurretPermanently(Turret turret)
    {
        turret.SwitchTurretMode(0);
        turret.SetToModeClientRpc(0);
        // wait a frame
        yield return 0;
        turret.ToggleTurretEnabled(false);
    }
}
