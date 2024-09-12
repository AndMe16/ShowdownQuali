using System;
using BepInEx.Configuration;
using ZeepSDK.Multiplayer;

public class ModConfig
{
    public ConfigEntry<int> endUnixTimestamp;
    public ConfigEntry<string> qualiPlaylistName;
    public ConfigEntry<string> lobbyName;
    public ConfigEntry<int> lobbyMaxPlayers;
    public ConfigEntry<bool> lobbyVisibility;
 

    private event EventHandler<SettingChangedEventArgs> OnSettingChanged;

    // Constructor that takes a ConfigFile instance from the main class
    public ModConfig(ConfigFile config)
    {
        endUnixTimestamp = config.Bind("Duration", "End Unix Timestamp", GetTimestampInAdvance(2880), 
                                    "Set the end of the qualifier as a unix timestamp");

        qualiPlaylistName = config.Bind("Qualifier Level", "Playlist Name", "ShowdownS4QualiPlaylist","Set the playlist name that contains the qualifier track");

        lobbyName         = config.Bind("Lobby", "Lobby Name", "Showdown Season 4 Qualifiers","Set the lobby name");

        lobbyMaxPlayers   = config.Bind("Lobby", "Lobby Max. Players", 64, new ConfigDescription("Set the lobby max. players", new AcceptableValueRange<int>(2, 64)));
        
        lobbyVisibility   = config.Bind("Lobby", "Public Lobby", true, "Set the lobby visibility");

        endUnixTimestamp.SettingChanged += SettingChangedHandler;
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
        if(e.ChangedSetting == endUnixTimestamp){
            CountdownManager.UpdatingCountDownLeft();
            LobbiesManager.LobbyTime(CountdownManager.CountdownLeft + 60);
        }
    }

    public int GetTimestampInAdvance(int extraMinutes){
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        DateTimeOffset timeInAdvance = currentTime.AddMinutes(extraMinutes);
        return (int)timeInAdvance.ToUnixTimeSeconds();
    }

    
}
