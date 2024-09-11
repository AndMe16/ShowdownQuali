using System;
using BepInEx.Configuration;
using ZeepSDK.Multiplayer;

public class ModConfig
{
    public ConfigEntry<int> qualifierDuration;

    private event EventHandler<SettingChangedEventArgs> OnSettingChanged;

    // Constructor that takes a ConfigFile instance from the main class
    public ModConfig(ConfigFile config)
    {
        qualifierDuration = config.Bind("Duration", "qualifierDuration (sec)", 172800, 
                                    new ConfigDescription("Set a custom duration of the event", new AcceptableValueRange<int>(2400, 172800)));

        qualifierDuration.SettingChanged += SettingChangedHandler;
        OnSettingChanged += OnSettingsChanged;
    }


    private void SettingChangedHandler(object sender, EventArgs e)
    {
        ConfigEntryBase changedSetting = sender as ConfigEntryBase;

        // Trigger the custom event
        OnSettingChanged?.Invoke(this, new SettingChangedEventArgs(changedSetting));
    }

    private void OnSettingsChanged(object sender, SettingChangedEventArgs e)
    {
        ModLogger.LogInfo($"Setting changed: {e.ChangedSetting.Definition.Key}");
        if(e.ChangedSetting == qualifierDuration){
            CountdownManager.CountdownLeft = qualifierDuration.Value;
            LobbiesManager.SecondsInADay = qualifierDuration.Value/2;
            ResetLobbyTimerManager.DailyMilliSeconds = qualifierDuration.Value*1000/2;
        }
    }
    
}
