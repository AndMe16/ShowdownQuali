using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZeepkistClient;

namespace ShowdownQuali;

public class NetworkManager
{
    public static IEnumerator TryReconnect()
    {
        while (!ZeepkistNetwork.IsConnected)
        {
            // Load the IntroScene3 immediately
            SceneManager.LoadScene("3D_MainMenu");

            // Wait for 10 seconds before loading the Online Lobby scene
            yield return new WaitForSeconds(5);
            // Load the Online Lobby scene after the 10-second delay
            SceneManager.LoadScene("Online Lobby");

            // Log and wait for 20 seconds before the next check
            ModLogger.LogInfo("Loading Online Lobby scene...");
            yield return new WaitForSeconds(60); // Check every 20 seconds
        }
    }
}