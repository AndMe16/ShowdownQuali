using System.Collections;
using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{
    private static CoroutineStarter _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public static void StartExternalCoroutine(IEnumerator coroutine)
    {
        if (_instance == null)
        {
            ModLogger.LogError("CoroutineStarter is not initialized!");
            return;
        }
        _instance.StartCoroutine(coroutine);
    }

    public static void StopExternalCoroutine(IEnumerator coroutine)
    {
        if (_instance == null)
        {
            ModLogger.LogError("CoroutineStarter is not initialized!");
            return;
        }
        _instance.StopCoroutine(coroutine);
    }
}
