using HarmonyLib;

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

        //var noEffectChance = 0.1f;
        //var successChance = 0.8f;
        //var criticalSuccessChance = 0.01f;
        var noEffectChance = 0f;
        var successChance = 0f;
        var criticalSuccessChance = 0.99f;

        var terminalObject = __instance.GetComponent<TerminalAccessibleObject>();
        var idInTerminal = terminalObject.objectCode;

        var currentPercent = 0f;

        if (random < (currentPercent += noEffectChance))
        {
            // chance that nothing at all happens
            TurretWhacker.Logger.LogInfo($"Turret {idInTerminal} has been hit! Random value: {random}. No effect.");
        }
        else if (random < (currentPercent += successChance))
        {
            // disable the turret, as per the terminal
            TurretWhacker.Logger.LogInfo($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is being disabled.");
            terminalObject.CallFunctionFromTerminal();
        }
        else if (random < (currentPercent += criticalSuccessChance))
        {
            // permanently disable the turret

            // TODO: Has a bug where it stays stuck on forever. Smaller version of that when disabling it from the terminal, but that resets.
            TurretWhacker.Logger.LogInfo($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is being permanently disabled.");
            __instance.SwitchTurretMode(0);
            __instance.SetToModeClientRpc(0);
            __instance.ToggleTurretEnabled(false);
        }
        else
        {
            // otherwise act as normal
            TurretWhacker.Logger.LogInfo($"Turret {idInTerminal} has been hit! Random value: {random}. Turret is beserk.");
            return true;
        }

        __result = true;
        return false;
    }
}
