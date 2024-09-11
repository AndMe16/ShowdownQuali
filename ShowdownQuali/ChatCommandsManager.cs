using ShowdownQuali;
using ZeepkistClient;
using ZeepSDK.ChatCommands;


public class ChatCommandManager
{
    
    public static void RegisterCommands()
    {
        RegisterStartCommand();
        RegisterStopCommand();
        RegisterInfoCommand();
    }

    private static void RegisterStartCommand(){
        LocalChatCommandCallbackDelegate startCommandCallback = new(OnStartCommand);
        ChatCommandApi.RegisterLocalChatCommand("/", "qualistart", "Start the qualifiers", startCommandCallback);
    }

    private static void OnStartCommand(string arguments)
    {
        if(ZeepkistNetwork.LocalPlayerHasHostPowers()&&!LobbiesManager.ShowdownStarted){
            ModLogger.LogInfo("Received a QualiStart command");
            LobbiesManager.SetLobbyForShowdownStart();
        }
    }


    private static void RegisterStopCommand(){
        LocalChatCommandCallbackDelegate stopCommandCallback = new(OnStopCommand);
        ChatCommandApi.RegisterLocalChatCommand("/", "qualistop", "Stop the qualifiers", stopCommandCallback);
    }

    private static void OnStopCommand(string arguments)
    {
        if (ZeepkistNetwork.LocalPlayerHasHostPowers())
        {
            ModLogger.LogInfo("Received a QualiStop command");
            LobbiesManager.ShowdownStarted = false;
            ResetLobbyTimerManager.StopDailyTimer();
            CountdownManager.StopCountdownTimer();
            CommandSenderManager.RemoveJoinServerMessages();
        }
        
    }

    private static void RegisterInfoCommand(){
        MixedChatCommandCallbackDelegate infoCommandCallback = new(OnInfoCommand);
        ChatCommandApi.RegisterMixedChatCommand("!","qualiinfo","Show all available info commands",infoCommandCallback);
    }

    private static void OnInfoCommand(bool isLocal, ulong playerId, string arguments)
    {
        ModLogger.LogInfo($"Received a QualInfo command from {playerId}");
    }
}