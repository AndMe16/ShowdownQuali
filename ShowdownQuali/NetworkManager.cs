using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepkistClient;

public class NetworkManager
{
    public static IEnumerator TryReconnect()
    {
        while (!ZeepkistNetwork.IsConnected)
        {
            yield return new WaitForSeconds(20);  // Check every 20 seconds
            SceneManager.LoadScene("Online Lobby");
            ModLogger.LogInfo("Loading Online Lobby scene...");
        }
    }
}
