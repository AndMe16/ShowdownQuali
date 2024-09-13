using System;
using System.Collections;
using UnityEngine;
using ZeepkistClient;
using ZeepkistNetworking;

namespace ShowdownQuali;

public class LobbiesManager
{
    // Lobby Joinmessage
    public static readonly string joinMessageColor = "orange";
    private static readonly string joinMessage = "This is the qualifier round! To compete in the event, you must register on our Discord. Join here: dsc.gg/zeepkist-showdown";
    public static readonly string joinMessageEnded = "The qualifier is over! Check our Discord for more info! dsc.gg/zeepkist-showdown";

    // Lobby Info

    // Showdown status

    private static readonly CoroutineStarter delayedLoadPlaylistCorutine = new CoroutineStarter();

    public static string LobbyID { get; set; }

    public static bool ShowdownStarted { get; set; }

    public static bool PlaylistSet { get; set; }

    public static bool EndingShowdown { get; set; }


    public static string GetCurrentLobby()
    {
        LobbyID = ZeepkistNetwork.CurrentLobby.ID;
        return LobbyID;
    }

    public static void LobbyTime(int seconds)
    {
        ModLogger.LogInfo($"Setting lobby timer to {seconds} seconds");
        CommandSenderManager.SetLobbyTime(seconds);
    }

    public static void JoinMessage()
    {
        ModLogger.LogInfo("Setting join message");
        CommandSenderManager.SetJoinMessage(joinMessageColor, joinMessage);
    }


    public static void RejoinLobby()
    {
        foreach (ZeepkistLobby lobby in ZeepkistNetwork.AllLobbies)
        {
            if (lobby.ID == LobbyID)
            {
                ModLogger.LogInfo("Congrats! The previous lobby was found!");
                ZeepkistNetwork.JoinLobby(LobbyID);
                return;
            }
        }

        ModLogger.LogInfo("Couldn't find the previous lobby :( Creating a new one!");
        ZeepkistNetwork.CreateLobby(Plugin.modConfig.lobbyName.Value, Plugin.modConfig.lobbyMaxPlayers.Value, Plugin.modConfig.lobbyVisibility.Value);
    }

    public static void OnShowdownEnded()
    {
        ModLogger.LogInfo("Showdown Ended!");
        ShowdownStarted = false;
        PlaylistSet = false;
        EndingShowdown = true;
        CountdownManager.StopCountdownTimer();
        CommandSenderManager.NotifyEndOfShowdownChat();
        PlaylistManager.SetHoFIndex();
        CommandSenderManager.SkipNextLevel();
    }

    public static void SetLobbyForShowdownStart()
    {
        PlaylistSet = true;
        ShowdownStarted = true;
        PlaylistManager.LoadPlaylist(Plugin.modConfig.qualiPlaylistName.Value);
        if (!PlaylistManager.CompareLevels(Plugin.modConfig.qualiPlaylistName.Value))
        {
            CommandSenderManager.SkipNextLevel();
        }
        else
        {
            ShowdownStart();
        }
    }

    public static void SetLobbyForShowdownResume()
    {
        delayedLoadPlaylistCorutine.StartExternalCoroutine(DelayedLoadPlaylist());
    }

    private static IEnumerator DelayedLoadPlaylist()
    {
        yield return new WaitForSeconds(5f); // 2-second delay
        PlaylistSet = true;
        PlaylistManager.LoadPlaylist(Plugin.modConfig.qualiPlaylistName.Value);
        if (!PlaylistManager.CompareLevels(Plugin.modConfig.qualiPlaylistName.Value))
        {
            CommandSenderManager.SkipNextLevel();
        }
        else
        {
            ShowdownStart();
        }
    }

    private static void ChangeLobbyName()
    {
        ZeepkistNetwork.CurrentLobby.Name = Plugin.modConfig.lobbyName.Value;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyNamePacket
            {
                Name = ZeepkistNetwork.CurrentLobby.Name
            });
        }
        catch (Exception ex)
        {
            ModLogger.LogError("Unabled exception in ChangeLobbyName: " + ex);
        }
    }

    private static void ChangeLobbyMaxPlayers()
    {
        ZeepkistNetwork.CurrentLobby.MaxPlayerCount = Plugin.modConfig.lobbyMaxPlayers.Value;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyMaxPlayersPacket
            {
                MaxPlayers = ZeepkistNetwork.CurrentLobby.MaxPlayerCount
            });
        }
        catch (Exception ex)
        {
            ModLogger.LogError("Unabled exception in ChangeLobbyMaxPlayers: " + ex);
        }
    }

    private static void ChangeLobbyVisibility()
    {
        ZeepkistNetwork.CurrentLobby.IsPublic = Plugin.modConfig.lobbyVisibility.Value;
        try
        {
            ZeepkistNetwork.NetworkClient?.SendPacket(new ChangeLobbyVisibilityPacket
            {
                Visiblity = ZeepkistNetwork.CurrentLobby.IsPublic
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

    public static void ShowdownStart()
    {
        ChangeLobbyInfo();
        JoinMessage();
        CountdownManager.UpdatingCountDownLeft();
        LobbyTime(CountdownManager.CountdownLeft + 60);
        CountdownManager.StartCountdown();
    }
}