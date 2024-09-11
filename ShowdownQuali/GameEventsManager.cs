using System;
using System.Collections.Generic;
using System.Linq;
using ZeepkistClient;
using ZeepSDK.Multiplayer;

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
            LobbiesManager.JoinMessage();
            double remainingTime = ResetLobbyTimerManager.RemainingTime;
            int newLobbyTime = (int)(remainingTime / 1000) + (60*10); 
            CommandSenderManager.SetLobbyTime(newLobbyTime);
            CountdownManager.ResumeCountdown();
            ResetLobbyTimerManager.ResumeDailyTimer();
            // MISSING ADDING QUALI LEVEL !!!!!!
        }
    }

    private static void OnLobbyListUpdated()
    {
        ModLogger.LogInfo("Lobby list got updated");
        if(gotDisconnected&&LobbiesManager.ShowdownStarted){
            if(createNewLobby){
                ZeepkistNetwork.CreateLobby(LobbiesManager.lobbyName,LobbiesManager.lobbyMaxPlayers,true);
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
            LobbiesManager.JoinMessage();
            double remainingTime = ResetLobbyTimerManager.RemainingTime;
            int newLobbyTime = (int)(remainingTime / 1000) + (60*10); 
            CommandSenderManager.SetLobbyTime(newLobbyTime);
            CountdownManager.ResumeCountdown();
            ResetLobbyTimerManager.ResumeDailyTimer();
            // MISSING ADDING QUALI LEVEL !!!!!!
        }
    }
    
}