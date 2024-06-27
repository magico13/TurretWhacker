using BepInEx.Configuration;

namespace TurretWhacker.Config;
public class PluginConfig
{
    public ConfigEntry<float> NoEffectChance { get; }
    public ConfigEntry<float> SuccessChance { get; }
    public ConfigEntry<float> CriticalSuccessChance { get; }

    public PluginConfig(ConfigFile config)
    {
        config.SaveOnConfigSet = false;

        NoEffectChance = config.Bind(
            "General",
            "NoEffectChance",
            0.1f,
            "The chance that nothing at all happens when a turret it hit.");
        
        SuccessChance = config.Bind(
            "General",
            "SuccessChance",
            0.8f,
            "The chance that the turret is disabled when hit.");
        
        CriticalSuccessChance = config.Bind(
            "General",
            "CriticalSuccessChance",
            0.01f,
            "The chance that the turret is permanently disabled when hit.");

        config.Save();
        config.SaveOnConfigSet = true;
    }


}
