using ZeepSDK.Chat;

namespace ShowdownQuali;

public class CommandSenderManager
{
    private static string timeLeftStr;

    private static string baseServerMessage;

    public static void SetLobbyTime(int time)
    {
        ChatApi.SendMessage($"/settime {time}");
    }

    public static void SetJoinMessage(string color, string joinMessage)
    {
        ChatApi.SendMessage($"/joinmessage {color} {joinMessage}");
    }

    public static void SetServerMessage(string formattedTime)
    {
        timeLeftStr = formattedTime;
        baseServerMessage = "/servermessage white 0 <size=+25><align=\"left\"><voffset=-1em><b><u><alpha=#00>S4OWDOWN</u></voffset>" +
                            "<br><line-height=1em><#FFFFFF><u>S<#F5DC4E>4<#FFFFFF>OWDOWN</u>" +
                            "<br><pos=0.25em><u>SEASON<#F5DC4E>4</u><voffset=0.5em>  </b><nobr><size=+15><#F77777>48h<#FFFFFF>-<#778AF7>Qualifier  " +
                            $"<#FFFFFF>Time Left: <#F5DC4E>{timeLeftStr}</voffset></nobr>" +
                            "<br><line-height=70%><size=+10><#FFFFFF>Register in our discord to participate!" +
                            "<br>Link:<#F5DC4E> dsc.gg/zeepkist-showdown" +
                            "<br><#FFFFFF>For more info use <#F5DC4E>!info <#FFFFFF>in chat";
        ChatApi.SendMessage(baseServerMessage);
    }

    public static void NotifyEndOfShowdown()
    {
        baseServerMessage = "/servermessage white 0 <size=+25><align=\"left\"><voffset=-1em><b><u><alpha=#00>S4OWDOWN</u></voffset>" +
                            "<br><line-height=1em><#FFFFFF><u>S<#F5DC4E>4<#FFFFFF>OWDOWN</u>" +
                            "<br><pos=0.25em><u>SEASON<#F5DC4E>4</u><voffset=0.5em>  </b><nobr><size=+15><#F77777>48h<#FFFFFF>-<#778AF7>Qualifier  " +
                            "<#F5DC4E>Time is up!</voffset></nobr>" +
                            "<br><line-height=70%><size=+10>The qualifier is over! Check our Discord for more info!" +
                            "<br>Link:<#F5DC4E> dsc.gg/zeepkist-showdown" +
                            "<br><#FFFFFF>For more info use <#F5DC4E>!qualiinfo <#FFFFFF>in chat";
        ChatApi.SendMessage(baseServerMessage);
    }

    public static void RequestHost(string username)
    {
        ChatApi.SendMessage($" {username}, I got disconnected from the lobby, can you please give me back host?");
        ChatApi.SendMessage("If in 5 minutes I don't have host, I will create a new official showdown lobby!");
    }

    public static void RemoveJoinServerMessages()
    {
        ChatApi.SendMessage("/servermessage remove");
        ChatApi.SendMessage("/joinmessage off");
    }

    public static void SkipNextLevel()
    {
        ChatApi.SendMessage("/fs");
    }

    public static void NotifyCreatingNewLobby()
    {
        ChatApi.SendMessage("Creating a new official Showdown Lobby. I'll wait for you all there!");
    }

    public static void NotifyEndOfShowdownChat()
    {
        ChatApi.SendMessage("The Qualifier is over! Thank you for participating :party:");
    }
}