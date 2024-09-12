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
        _ = CoroutineStarter.Instance;
        modConfig.endUnixTimestamp.Value = modConfig.GetTimestampInAdvance(2880);
    }

    


    

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        harmony = null;
    }
}
