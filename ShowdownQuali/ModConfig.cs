using System;
using BepInEx.Configuration;
using ZeepSDK.Multiplayer;

public class ModConfig
{
    public ConfigEntry<bool> useEndUnixTimestamp;
    public ConfigEntry<int> qualifierDuration;
    public ConfigEntry<int> endUnixTimestamp;
    public ConfigEntry<string> qualiPlaylistName;
    public ConfigEntry<string> lobbyName;
    public ConfigEntry<int> lobbyMaxPlayers;
    public ConfigEntry<bool> lobbyVisibility;

    public int remainingTimeInSeconds;  

    private event EventHandler<SettingChangedEventArgs> OnSettingChanged;

    // Constructor that takes a ConfigFile instance from the main class
    public ModConfig(ConfigFile config)
    {
        useEndUnixTimestamp = config.Bind("Duration", "Use End Unix Timestamp", false, 
                                    "Set the duration of the qualifier using a Unix timestamp instead of the qualifierDuration");

        qualifierDuration = config.Bind("Duration", "Qualifier Duration (sec)", 172800, 
                                    new ConfigDescription("Set a custom duration of the event", new AcceptableValueRange<int>(2400, 172800)));

        endUnixTimestamp = config.Bind("Duration", "End Unix Timestamp", GetTimestampInAdvance(2880), 
                                    new ConfigDescription("Set the end of the qualifier as a unix timestamp", new AcceptableValueRange<int>(GetTimestampInAdvance(1), GetTimestampInAdvance(2880))));

        qualiPlaylistName = config.Bind("Qualifier Level", "Playlist Name", "ShowdownS4QualiPlaylist","Set the playlist name that contains the qualifier track");

        lobbyName         = config.Bind("Lobby", "Lobby Name", "Showdown Season 4 Qualifiers","Set the lobby name");

        lobbyMaxPlayers   = config.Bind("Lobby", "Lobby Max. Players", 64, new ConfigDescription("Set the lobby max. players", new AcceptableValueRange<int>(2, 64)));
        
        lobbyVisibility   = config.Bind("Lobby", "Public Lobby", true, "Set the lobby visibility");

        qualifierDuration.SettingChanged += SettingChangedHandler;
        endUnixTimestamp.SettingChanged += SettingChangedHandler;
        useEndUnixTimestamp.SettingChanged += SettingChangedHandler;
        OnSettingChanged += OnSettingsChanged;

        // Initialize the remaining time on startup
        UpdateRemainingTime();
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
        if(e.ChangedSetting == qualifierDuration && !useEndUnixTimestamp.Value){
            UpdateRemainingTime();
            UpdateTimeVars();
        } else if(e.ChangedSetting == endUnixTimestamp && useEndUnixTimestamp.Value){
            UpdateRemainingTime();
            UpdateTimeVars();
        } else if(e.ChangedSetting == useEndUnixTimestamp){
            UpdateRemainingTime();
            UpdateTimeVars();
        }
    }

    public void UpdateRemainingTime()
    {
        // Determine whether to use endUnixTimestamp or qualifierDuration
        if (useEndUnixTimestamp.Value)
        {
            remainingTimeInSeconds = CalculateRemainingTimeFromTimestamp();
        }
        else
        {
            remainingTimeInSeconds = qualifierDuration.Value;
        }

        // Log the remaining time for debugging
        ModLogger.LogInfo($"Remaining time updated: {remainingTimeInSeconds} seconds");
    }
    
    private int CalculateRemainingTimeFromTimestamp()
    {
        int currentUnixTimestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int remainingTimeInSeconds = endUnixTimestamp.Value - currentUnixTimestamp;

        if (remainingTimeInSeconds < 0)
        {
            ModLogger.LogWarning("The endUnixTimestamp has already passed.");
            return 120; // If the event has already ended, return 120
        }

        return (int)remainingTimeInSeconds;
    }

    private int GetTimestampInAdvance(int extraMinutes){
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        DateTimeOffset timeInAdvance = currentTime.AddMinutes(extraMinutes);
        return (int)timeInAdvance.ToUnixTimeSeconds();
    }

    private void UpdateTimeVars(){
        CountdownManager.CountdownLeft = remainingTimeInSeconds;
        LobbiesManager.SecondsInADay = remainingTimeInSeconds/2;
        ResetLobbyTimerManager.DailyMilliSeconds = remainingTimeInSeconds*1000/2;
    }
    
}
