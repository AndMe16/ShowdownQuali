using ShowdownQuali;
using ZeepkistClient;

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
    public static readonly string lobbyName = "Showdown Qualifiers! TEST DONT ENTER"; // Change It!
    public static readonly int lobbyMaxPlayers = 64; // Change It!
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
        ZeepkistNetwork.CreateLobby(lobbyName,lobbyMaxPlayers,true);
    }

    public static void OnShowdownEnded(){
        ModLogger.LogInfo("Showdown Ended!");
        showdownStarted = false;
        CommandSenderManager.NotifyEndOfShowdown();
        CommandSenderManager.SetJoinMessage(joinMessageColor,joinMessageEnded);
    }

    
}