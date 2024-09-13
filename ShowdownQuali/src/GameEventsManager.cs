using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeepkistClient;
using ZeepSDK.Multiplayer;
using ZeepSDK.Racing;

namespace ShowdownQuali;

public class GameEventsManager
{
    private static bool gotDisconnected;

    public static bool inBetweenRounds;

    private static readonly CoroutineStarter reconnectCorutine = new CoroutineStarter();

    public static bool CreateNewLobby { get; set; }

    public static void SubscribeToEvents()
    {
        MultiplayerApi.JoinedRoom += OnJoinedRoom;
        MultiplayerApi.DisconnectedFromGame += OnDisconnected;
        MultiplayerApi.CreatedRoom += OnCreatedRoom;
        ZeepkistNetwork.LobbyListUpdated += OnLobbyListUpdated;
        ZeepkistNetwork.MasterChanged += OnMasterChanged;
        RacingApi.LevelLoaded += OnLevelLoaded;
        RacingApi.RoundEnded += OnRoundEnded;
        RacingApi.RoundStarted += OnRoundStarted;
    }

    private static async void OnJoinedRoom()
    {
        string lobbyID = LobbiesManager.GetCurrentLobby();
        ModLogger.LogInfo($"Joining Room with ID: {lobbyID}");

        // Wait for 5 seconds before continuing
        await Task.Delay(5000);

        if (LobbiesManager.ShowdownStarted && !ZeepkistNetwork.LocalPlayerHasHostPowers())
        {
            ModLogger.LogInfo("Requesting host");
            List<ZeepkistNetworkPlayer> playerList = ZeepkistNetwork.PlayerList;
            ZeepkistNetworkPlayer hostPlayer = playerList.FirstOrDefault(p => p.isHost);
            CommandSenderManager.RequestHost(hostPlayer.Username);
            WaitingHostTimerManager.StartWaitingHostTimer();
        }
    }

    private static void OnDisconnected()
    {
        ModLogger.LogInfo("Disconnected from lobby");
        gotDisconnected = true;
        if (LobbiesManager.ShowdownStarted)
        {
            CountdownManager.StopCountdownTimer();
            LobbiesManager.PlaylistSet = false;
            reconnectCorutine.StartExternalCoroutine(NetworkManager.TryReconnect());
        }
    }

    private static void OnCreatedRoom()
    {
        string lobbyID = LobbiesManager.GetCurrentLobby();
        ModLogger.LogInfo($"Creating Room with ID: {lobbyID}");
        gotDisconnected = false;
        CreateNewLobby = false;
        if (LobbiesManager.ShowdownStarted)
        {
            LobbiesManager.SetLobbyForShowdownResume();
        }
    }

    private static void OnLobbyListUpdated()
    {
        ModLogger.LogInfo("Lobby list got updated");
        if (gotDisconnected && LobbiesManager.ShowdownStarted)
        {
            reconnectCorutine.StopExternalCoroutine();
            if (CreateNewLobby)
            {
                ZeepkistNetwork.CreateLobby(Plugin.modConfig.lobbyName.Value, Plugin.modConfig.lobbyMaxPlayers.Value, Plugin.modConfig.lobbyVisibility.Value);
            }
            else
            {
                LobbiesManager.RejoinLobby();
            }
        }
    }

    private static void OnMasterChanged(ZeepkistNetworkPlayer player)
    {
        ModLogger.LogInfo($"The new master of the lobby is: {player.Username}");
        if (player.IsLocal && gotDisconnected && LobbiesManager.ShowdownStarted)
        {
            gotDisconnected = false;
            CreateNewLobby = false;
            WaitingHostTimerManager.StopWaitingHostTimer();
            LobbiesManager.SetLobbyForShowdownResume();
        }
    }

    private static void OnLevelLoaded()
    {
        ModLogger.LogInfo("Level loaded");
        if (ZeepkistNetwork.LocalPlayerHasHostPowers() && LobbiesManager.EndingShowdown)
        {
            LobbiesManager.EndingShowdown = false;
            LobbiesManager.LobbyTime(60 * 6);
            CommandSenderManager.NotifyEndOfShowdown();
            CommandSenderManager.SetJoinMessage(LobbiesManager.joinMessageColor, LobbiesManager.joinMessageEnded);
        }
        else if (ZeepkistNetwork.LocalPlayerHasHostPowers() && LobbiesManager.ShowdownStarted && LobbiesManager.PlaylistSet)
        {
            LobbiesManager.PlaylistSet = false;
            LobbiesManager.ShowdownStart();
        }
    }

    private static void OnRoundEnded()
    {
        inBetweenRounds = true;
    }

    private static void OnRoundStarted()
    {
        inBetweenRounds = false;
    }
}