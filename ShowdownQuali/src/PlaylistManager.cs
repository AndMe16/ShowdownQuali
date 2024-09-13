using ZeepkistClient;
using ZeepSDK.Level;
using ZeepSDK.Multiplayer;
using ZeepSDK.Playlist;

namespace ShowdownQuali;

public class PlaylistManager
{
    public static void LoadPlaylist(string playlistName)
    {
        PlaylistSaveJSON qualiPlaylist = PlaylistApi.GetPlaylist(playlistName);
        ZeepkistNetwork.CurrentLobby.Playlist = qualiPlaylist.levels;
        ZeepkistNetwork.CurrentLobby.RoundTime = qualiPlaylist.roundLength;
        ZeepkistNetwork.CurrentLobby.PlaylistRandom = qualiPlaylist.shufflePlaylist;
        ZeepkistNetwork.CurrentLobby.CurrentPlaylistIndex = 0;
        ZeepkistNetwork.CurrentLobby.NextPlaylistIndex = 0;
        MultiplayerApi.UpdateServerPlaylist();
    }

    public static bool CompareLevels(string playlistName)
    {
        PlaylistSaveJSON qualiPlaylist = PlaylistApi.GetPlaylist(playlistName);
        if (qualiPlaylist.levels[0].UID == LevelApi.CurrentZeepLevel.UniqueId)
        {
            return true;
        }

        return false;
    }

    public static void SetHoFIndex()
    {
        MultiplayerApi.SetNextLevelIndex(1);
    }
}