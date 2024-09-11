using System;
using System.Collections.Generic;
using System.Linq;
using ShowdownQuali;
using ZeepkistClient;
using ZeepSDK.Multiplayer;
using ZeepSDK.Racing;

public class GameEventsManager{

    private static bool gotDisconnected = false;
    private static bool createNewLobby = false;
    public static bool CreateNewLobby
    {
        get { return createNewLobby; }
        set { createNewLobby = value; }
    }

    public static void SubscribeToEvents()
    {
        MultiplayerApi.JoinedRoom += OnJoinedRoom;
        MultiplayerApi.DisconnectedFromGame += OnDisconnected;
        MultiplayerApi.CreatedRoom += OnCreatedRoom;
        ZeepkistNetwork.LobbyListUpdated += OnLobbyListUpdated;
        ZeepkistNetwork.MasterChanged += OnMasterChanged;
        RacingApi.LevelLoaded += OnLevelLoaded;   
    }

    private static void OnJoinedRoom(){
        string lobbyID = LobbiesManager.GetCurrentLobby();
        ModLogger.LogInfo($"Joining Room with ID: {lobbyID}");
        if(LobbiesManager.ShowdownStarted&&!ZeepkistNetwork.LocalPlayerHasHostPowers()){
            ModLogger.LogInfo($"Requesting host");
            List<ZeepkistNetworkPlayer> playerList = ZeepkistNetwork.PlayerList;
            var hostPlayer = playerList.FirstOrDefault(p => p.isHost);
            CommandSenderManager.RequestHost(hostPlayer.Username);
            WaitingHostTimerManager.StartWaitingHostTimer();
        }
    }

    private static void OnDisconnected(){
        ModLogger.LogInfo($"Disconnected from lobby");
        gotDisconnected = true;
        if (LobbiesManager.ShowdownStarted){
            CountdownManager.PauseCountdown();
            ResetLobbyTimerManager.PauseDailyTimer();
        }
    }
    
    private static void OnCreatedRoom(){
        string lobbyID = LobbiesManager.GetCurrentLobby();
        ModLogger.LogInfo($"Creating Room with ID: {lobbyID}");
        gotDisconnected = false;
        createNewLobby = false;
        if (LobbiesManager.ShowdownStarted){
            LobbiesManager.SetLobbyForShowdownResume();
        }
    }

    private static void OnLobbyListUpdated()
    {
        ModLogger.LogInfo("Lobby list got updated");
        if(gotDisconnected&&LobbiesManager.ShowdownStarted){
            if(createNewLobby){
                ZeepkistNetwork.CreateLobby(Plugin.modConfig.lobbyName.Value,Plugin.modConfig.lobbyMaxPlayers.Value,true);
            }
            else{
                LobbiesManager.RejoinLobby();
            }
            
        }
    }

    private static void OnMasterChanged(ZeepkistNetworkPlayer player)
    {
        ModLogger.LogInfo($"The new master of the lobby is: {player.Username}");
        if(player.IsLocal&&gotDisconnected&&LobbiesManager.ShowdownStarted)
        {
            gotDisconnected = false;
            createNewLobby = false;
            WaitingHostTimerManager.StopWaitingHostTimer();
            LobbiesManager.SetLobbyForShowdownResume();
        }
    }

    private static void OnLevelLoaded()
    {
        ModLogger.LogInfo("Level loaded");
        if (ZeepkistNetwork.LocalPlayerHasHostPowers()&&LobbiesManager.ShowdownStarted&&LobbiesManager.PlaylistSet){
            LobbiesManager.PlaylistSet = false;
            if(!LobbiesManager.ShowDownResume){
                LobbiesManager.ChangeLobbyInfo(); 
                LobbiesManager.JoinMessage();
                LobbiesManager.LobbyTime();
                ResetLobbyTimerManager.StartDailyTimer();
                CountdownManager.StartCountdown();
            }
            else{
                LobbiesManager.ShowDownResume = false;
                LobbiesManager.ChangeLobbyInfo(); 
                LobbiesManager.JoinMessage();
                double remainingTime = ResetLobbyTimerManager.RemainingTime;
                int newLobbyTime = (int)(remainingTime / 1000) + (60*10); 
                CommandSenderManager.SetLobbyTime(newLobbyTime);
                CountdownManager.ResumeCountdown();
                ResetLobbyTimerManager.ResumeDailyTimer();
            }
        }
    }
    
}