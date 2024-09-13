using System.Timers;
using ZeepkistClient;

namespace ShowdownQuali;

public class WaitingHostTimerManager
{
    private static Timer waitingHostTimer;

    public static void StartWaitingHostTimer()
    {
        ModLogger.LogInfo("Starting waitingHostTimer");
        waitingHostTimer = new Timer(5 * 60 * 1000);
        waitingHostTimer.Elapsed += OnTimedEvent;
        waitingHostTimer.AutoReset = false;
        waitingHostTimer.Enabled = true;
    }

    public static void StopWaitingHostTimer()
    {
        if (waitingHostTimer == null)
        {
            return;
        }

        waitingHostTimer.Stop();
        waitingHostTimer.Dispose();
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        ModLogger.LogInfo("Creating new lobby after 5 minutes waiting for host");
        CommandSenderManager.NotifyCreatingNewLobby();
        GameEventsManager.CreateNewLobby = true;
        StopWaitingHostTimer();
        ZeepkistNetwork.Disconnect(); // Not sure if this works :)
    }
}