using System;
using System.Threading.Tasks;
using ZeepkistClient;
using ZeepSDK.Chat;
using ZeepSDK.ChatCommands;

namespace ShowdownQuali.Commands;

public class CommandInfo : IMixedChatCommand
{
    private static DateTime _lastExecutionTime = DateTime.MinValue; // Track the last time the method was executed
    private static readonly TimeSpan CooldownPeriod = TimeSpan.FromSeconds(30); // Set cooldown duration
    private static bool _isExecuting;
    public string Prefix => "!";
    public string Command => "info";
    public string Description => "Shows detailed information about the Showdown Qualifier";

    public async void Handle(ulong playerId, string arguments)
    {
        const double averageReadingSpeed = 250.0; // Words per minute
        const double readingTimeFactor = 1.6; // Adjusted time factor

        if (_isExecuting)
        {
            return;
        }


        // Try to get the player information
        string playerNamePrefix = ZeepkistNetwork.TryGetPlayer(playerId, out ZeepkistNetworkPlayer player)
            ? $"@{player.Username}"
            : ""; // If player is not found, no username is included

        if (DateTime.Now - _lastExecutionTime < CooldownPeriod)
        {
            // If cooldown has not passed, return or notify the user.
            ChatApi.SendMessage($"{playerNamePrefix} - You need to wait before using this command again.");
            return;
        }

        _isExecuting = true;

        string[] messages =
        {
            $"{playerNamePrefix}<br>1. To qualify, you first need to install the GTR mod by Thundernerd.",
            "<br>2. Next, go to the mod options and link your Discord account in the GTR section.",
            "<br>3. To finalize your qualification, join our Discord and use /register in the #register channel.",
            "<br>4. After that, create an online lobby and try to set the best times on the map.",
            "<br>5. Alternatively, you can stay in this lobby; both options work just fine.",
            "<br>6. Good luck, and have fun! :smile:",
            ".",
            ".",
            ".",
            ".",
            "."
        };

        foreach (string message in messages)
        {
            ChatApi.SendMessage(message);

            // Calculate words in the message
            int wordCount = message.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            // Calculate the delay based on reading speed and the extra time factor
            double secondsPerWord = 60.0 / averageReadingSpeed;
            double delayInSeconds = wordCount * secondsPerWord * readingTimeFactor;

            // Convert delay to milliseconds and apply it
            await Task.Delay((int)(delayInSeconds * 1000)); // Delay in milliseconds
        }

        // Update the last execution time after the command is successfully processed
        _lastExecutionTime = DateTime.Now;
        _isExecuting = false;
    }

    public void Handle(string arguments)
    {
        Handle(ZeepkistNetwork.LocalPlayer.SteamID, arguments);
    }

    public static event Action CommandInvoked;
}