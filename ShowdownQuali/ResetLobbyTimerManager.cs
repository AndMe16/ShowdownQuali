using System.Timers;
using System.Diagnostics;
using ShowdownQuali;

public class ResetLobbyTimerManager
{
    private static Timer dailyTimer;
    private static int dailyMilliSeconds = Plugin.modConfig.remainingTimeInSeconds * 1000/2;
    public static int DailyMilliSeconds
    {
        set { dailyMilliSeconds=value; }
        get {return dailyMilliSeconds;}
    }
    private static double remainingTime;
    public static double RemainingTime
    {
        get { return remainingTime; }
    }
    

    private static Stopwatch stopwatch = new Stopwatch();

    public static void StartDailyTimer()
    {
        ModLogger.LogInfo($"Starting dailyTimer. dailyMilliSeconds: {dailyMilliSeconds}");
        dailyTimer = new Timer(dailyMilliSeconds - (dailyMilliSeconds*0.1)); // -10 minutes to compensate timer errors 
        dailyTimer.Elapsed += OnTimedEvent;
        dailyTimer.AutoReset = true;
        dailyTimer.Enabled = true;
        stopwatch.Start(); // Start tracking elapsed time
    }

    public static void StopDailyTimer()
    {
        if (dailyTimer != null)
        {
            dailyTimer.Stop();
            dailyTimer.Dispose();
        }
        stopwatch.Stop();
        stopwatch.Reset();
    }

    public static void PauseDailyTimer()
    {
        if (dailyTimer != null)
        {
            dailyTimer.Stop();
            stopwatch.Stop();
            remainingTime = dailyTimer.Interval - stopwatch.ElapsedMilliseconds; // Calculate remaining time
            ModLogger.LogInfo($"Timer paused with {remainingTime} ms remaining.");
        }
    }

    public static void ResumeDailyTimer()
    {
        if (dailyTimer != null && remainingTime > 0)
        {
            dailyTimer.Interval = remainingTime; // Resume with remaining time
            dailyTimer.Start();
            stopwatch.Restart(); // Restart the stopwatch to track elapsed time again
            ModLogger.LogInfo("Timer resumed.");
        }
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        LobbiesManager.LobbyTime();
        ModLogger.LogInfo("Resetting lobby timer");
        stopwatch.Reset(); // Reset the stopwatch after the timer event
        remainingTime = dailyMilliSeconds - (dailyMilliSeconds*0.1); // Reset the remaining time
    }
}
