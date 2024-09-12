using System;
using System.Timers;
using ShowdownQuali;

public class CountdownManager{
    private static Timer countdownTimer;
    private static int countdownLeft;
    public static int CountdownLeft
    {
        get { return countdownLeft; }
    }

    public static void StartCountdown()
    {
        ModLogger.LogInfo("Starting countdown");
        countdownTimer = new Timer(1000); 
        countdownTimer.Elapsed += OnTimedEvent;
        countdownTimer.AutoReset = true;
        countdownTimer.Enabled = true;
    }

    public static void StopCountdownTimer()
    {
        if (countdownTimer != null)
        {
            countdownLeft = 0;
            countdownTimer.Stop();
            countdownTimer.Dispose();
        }
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        if (countdownLeft > 0)
        {
            UpdateCountdown();
        }
        else
        {
            StopCountdownTimer();
            LobbiesManager.OnShowdownEnded();
        }
    }

    private static void UpdateCountdown()
    {
        string formattedTime = GetFormattedTime();
        CommandSenderManager.SetServerMessage(formattedTime);
        countdownLeft--;
    }

    private static string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(countdownLeft);
        int totalHours = (int)time.TotalHours; 
        return string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, time.Minutes, time.Seconds);
    }


    public static void UpdatingCountDownLeft()
    {
        int currentUnixTimestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int remainingTimeInSeconds = Plugin.modConfig.endUnixTimestamp.Value - currentUnixTimestamp;

        if (remainingTimeInSeconds < 0)
        {
            ModLogger.LogWarning("The endUnixTimestamp has already passed.");
            countdownLeft = 0; // If the event has already ended, return 60
        }
        ModLogger.LogInfo($"Remaining time updated: {remainingTimeInSeconds} seconds");
        countdownLeft = (int)remainingTimeInSeconds;
    }
    
}