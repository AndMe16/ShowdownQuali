using System;
using System.Timers;
using ShowdownQuali;

public class CountdownManager{
    private static Timer countdownTimer;
    private static int countdownLeft = Plugin.modConfig.qualifierDuration.Value;

    public static int CountdownLeft
    {
        set { countdownLeft = value; }
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
            countdownLeft = Plugin.modConfig.qualifierDuration.Value;
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
            ResetLobbyTimerManager.StopDailyTimer();
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

    public static void PauseCountdown(){
        countdownTimer?.Stop();
    }

    public static void ResumeCountdown(){
        countdownTimer?.Start();
    }
   
    
}