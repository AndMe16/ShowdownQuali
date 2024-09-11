using System;
using System.Collections;
using ShowdownQuali;
using UnityEngine;
using ZeepkistClient;
using ZeepkistNetworking;

public class LobbiesManager{

    // Lobby time
    private static int secondsInADay = Plugin.modConfig.qualifierDuration.Value/2;

    public static int SecondsInADay
    {
        set { secondsInADay = value; }
    }

    // Lobby Joinmessage
    private static readonly string joinMessageColor = "orange";
    private static readonly string joinMessage = "This is the qualifier round! To compete in the event, you must register on our Discord. Join here: dsc.gg/zeepkist-showdown"; 
    private static readonly string joinMessageEnded = "The qualifier is over! Check our Discord for more info! dsc.gg/zeepkist-showdown";

    // Lobby Info
    private static string lobbyID;
    public static string LobbyID
    {
        get { return lobbyID; }
        set { lobbyID = value; }
    }

    // Showdown status
    private static bool showdownStarted = false;
    public static bool ShowdownStarted
    {
        get { return showdownStarted; }
        set { showdownStarted = value; }
    }
    private static bool playlistSet = false;
    public static bool PlaylistSet
    {
        get { return playlistSet; }
        set { playlistSet = value; }
    }

    private static bool showDownResume = false;

    public static bool ShowDownResume
    {
        get { return showDownResume; }
        set { showDownResume = value; }
    }
    

    public static string GetCurrentLobby(){
        lobbyID = ZeepkistNetwork.CurrentLobby.ID;
        return lobbyID;
    }

    public static void LobbyTime(){
        ModLogger.LogInfo($"Setting lobby timer to {secondsInADay} seconds");
        CommandSenderManager.SetLobbyTime(secondsInADay);
    }

    public static void JoinMessage(){
        ModLogger.LogInfo($"Setting join message");
        CommandSenderManager.SetJoinMessage(joinMessageColor,joinMessage);
    }


    public static void RejoinLobby(){
        foreach(ZeepkistLobby lobby in ZeepkistNetwork.AllLobbies){
            if(lobby.ID==lobbyID){
                ModLogger.LogInfo("Congrats! The previous lobby was found!");
                ZeepkistNetwork.JoinLobby(lobbyID);
                return;
            }
        }
        ModLogger.LogInfo("Couldn't find the previous lobby :( Creating a new one!");
        ZeepkistNetwork.CreateLobby(Plugin.modConfig.lobbyName.Value,Plugin.modConfig.lobbyMaxPlayers.Value,true);
    }

    public static void OnShowdownEnded(){
        ModLogger.LogInfo("Showdown Ended!");
        showdownStarted = false;
        CommandSenderManager.NotifyEndOfShowdown();
        CommandSenderManager.SetJoinMessage(joinMessageColor,joinMessageEnded);
    }

    public static void SetLobbyForShowdownStart(){
        playlistSet = true;
        showdownStarted = true;
        PlaylistManager.LoadPlaylist(Plugin.modConfig.qualiPlaylistName.Value);
        if(!PlaylistManager.CompareLevels(Plugin.modConfig.qualiPlaylistName.Value)){
            CommandSenderManager.SkipNextLevel();
        }
        else{
            ChangeLobbyInfo(); 
            JoinMessage();
            LobbyTime();
            ResetLobbyTimerManager.StartDailyTimer();
            CountdownManager.StartCountdown();
        }
        
    }
    public static void SetLobbyForShowdownResume()
    {
        CoroutineStarter.StartExternalCoroutine(DelayedLoadPlaylist());
    }

    private static IEnumerator DelayedLoadPlaylist()
    {
        yield return new WaitForSeconds(2f); // 2-second delay
        playlistSet = true;
        showDownResume = true;
        PlaylistManager.LoadPlaylist(Plugin.modConfig.qualiPlaylistName.Value);
        if(!PlaylistManager.CompareLevels(Plugin.modConfig.qualiPlaylistName.Value)){
            CommandSenderManager.SkipNextLevel();
        }
        else{
            ShowDownResume = false;
            ChangeLobbyInfo(); 
            JoinMessage();
            double remainingTime = ResetLobbyTimerManager.RemainingTime;
            int newLobbyTime = (int)(remainingTime / 1000) + (60*10); 
            CommandSenderManager.SetLobbyTime(newLobbyTime);
            CountdownManager.ResumeCountdown();
            ResetLobbyTimerManager.ResumeDailyTimer();
        }
    }

    private static void ChangeLobbyName(){
        ZeepkistNetwork.CurrentLobby.Name = Plugin.modConfig.lobbyName.Value;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyNamePacket
            {
                Name = ZeepkistNetwork.CurrentLobby.Name,
            });
        }
        catch (Exception ex)
        {
            ModLogger.LogError("Unabled exception in ChangeLobbyName: " + ex);
        }
    } 
    private static void ChangeLobbyMaxPlayers(){
        ZeepkistNetwork.CurrentLobby.MaxPlayerCount = Plugin.modConfig.lobbyMaxPlayers.Value;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyMaxPlayersPacket
            {
                MaxPlayers = ZeepkistNetwork.CurrentLobby.MaxPlayerCount,
            });
        }
        catch (Exception ex)
        {
            ModLogger.LogError("Unabled exception in ChangeLobbyMaxPlayers: " + ex);
        }
    }

    private static void ChangeLobbyVisibility(){
        ZeepkistNetwork.CurrentLobby.IsPublic = true;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyVisibilityPacket
            {
                Visiblity = ZeepkistNetwork.CurrentLobby.IsPublic,
            });
        }
        catch (Exception ex)
        {
            ModLogger.LogError("Unabled exception in ChangeLobbyVisibility: " + ex);
        }
    }  
    
    public static void ChangeLobbyInfo()
    {
        ChangeLobbyName();
        ChangeLobbyMaxPlayers();
        ChangeLobbyVisibility();
    }
}