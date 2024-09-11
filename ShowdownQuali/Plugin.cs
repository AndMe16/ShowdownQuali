using BepInEx;
using HarmonyLib;
using UnityEngine;


namespace ShowdownQuali;

[BepInPlugin("andme123.showdownquali", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ZeepSDK")]
public class Plugin : BaseUnityPlugin
{
    private Harmony harmony;
    public static ModConfig modConfig;

    private void Awake()
    {
        ModLogger.Initialize(Logger);
        harmony = new Harmony("andme123.showdownquali");
        harmony.PatchAll();

        // Plugin startup logic
        Logger.LogInfo($"Plugin {"andme123.showdownquali"} is loaded!");

        modConfig = new ModConfig(Config);
        GameEventsManager.SubscribeToEvents();
        ChatCommandManager.RegisterCommands();
        InitializeCoroutineStarter();
    }

    private void InitializeCoroutineStarter()
    {
        GameObject coroutineStarterObject = new GameObject("CoroutineStarter");
        coroutineStarterObject.AddComponent<CoroutineStarter>();
        DontDestroyOnLoad(coroutineStarterObject);  // Optional: keep it across scenes
    }


    

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
