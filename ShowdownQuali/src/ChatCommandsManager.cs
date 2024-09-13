using ShowdownQuali.Commands;
using ZeepkistClient;
using ZeepSDK.ChatCommands;

namespace ShowdownQuali;

public class ChatCommandManager
{
    public static void RegisterCommands()
    {
        RegisterStartCommand();
        RegisterStopCommand();
        RegisterInfoCommand();
    }

    private static void RegisterStartCommand()
    {
        LocalChatCommandCallbackDelegate startCommandCallback = OnStartCommand;
        ChatCommandApi.RegisterLocalChatCommand("/", "qualistart", "Start the qualifiers", startCommandCallback);
    }

    private static void OnStartCommand(string arguments)
    {
        if (ZeepkistNetwork.LocalPlayerHasHostPowers() && !LobbiesManager.ShowdownStarted && !GameEventsManager.inBetweenRounds)
        {
            ModLogger.LogInfo("Received a QualiStart command");
            LobbiesManager.SetLobbyForShowdownStart();
        }
    }


    private static void RegisterStopCommand()
    {
        LocalChatCommandCallbackDelegate stopCommandCallback = OnStopCommand;
        ChatCommandApi.RegisterLocalChatCommand("/", "qualistop", "Stop the qualifiers", stopCommandCallback);
    }

    private static void OnStopCommand(string arguments)
    {
        if (LobbiesManager.ShowdownStarted && !GameEventsManager.inBetweenRounds)
        {
            ModLogger.LogInfo("Received a QualiStop command");
            LobbiesManager.ShowdownStarted = false;
            LobbiesManager.PlaylistSet = false;
            CountdownManager.StopCountdownTimer();
            CommandSenderManager.RemoveJoinServerMessages();
        }
    }

    private static void RegisterInfoCommand()
    {
        ChatCommandApi.RegisterMixedChatCommand<CommandInfo>();
    }

    private static void OnInfoCommand(bool isLocal, ulong playerId, string arguments)
    {
        ModLogger.LogInfo($"Received a QualInfo command from {playerId}");
    }
}