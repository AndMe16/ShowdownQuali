using ZeepSDK.Chat;

public class CommandSenderManager{

    private static string timeLeftStr;

    private static string baseServerMessage;

    public static void SetLobbyTime(int time){
        ChatApi.SendMessage($"/settime {time}");
    }

    public static void SetJoinMessage(string color, string joinMessage){
        ChatApi.SendMessage($"/joinmessage {color} {joinMessage}");
    }

    public static void SetServerMessage(string formattedTime){
        timeLeftStr = formattedTime;
        baseServerMessage = "/servermessage white 0 <size=+30><align=\"left\"><voffset=-1em><b><u><alpha=#00>S4OWDOWN</u></voffset>"
                                                        +"<br><line-height=1em><#FFFFFF><u>S<#F5DC4E>4<#FFFFFF>OWDOWN</u>"
                                                        +"<br><pos=0.25em><u>SEASON<#F5DC4E>4</u><pos=5em><voffset=0.5em> </b><size=+25><#F77777>48h<#FFFFFF>-<#778AF7>Qualifier "
                                                        +$"<pos=15em><#FFFFFF>Time Left: <#F5DC4E>{timeLeftStr}</voffset>"
                                                        +"<br><line-height=50%><size=+10><#FFFFFF>Register in our discord to participate!"
                                                        +"<br>Link:<#F5DC4E> dsc.gg/zeepkist-showdown"
                                                        +"<br><#FFFFFF>For more info use <#F5DC4E>!qualiinfo <#FFFFFF>in chat";
        ChatApi.SendMessage(baseServerMessage);
    }

    public static void NotifyEndOfShowdown(){
        baseServerMessage = "/servermessage white 0 <size=+30><align=\"left\"><voffset=-1em><b><u><alpha=#00>S4OWDOWN</u></voffset>"
                                                        +"<br><line-height=1em><#FFFFFF><u>S<#F5DC4E>4<#FFFFFF>OWDOWN</u>"
                                                        +"<br><pos=0.25em><u>SEASON<#F5DC4E>4</u><pos=5em><voffset=0.5em> </b><size=+25><#F77777>48h<#FFFFFF>-<#778AF7>Qualifier "
                                                        +$"<pos=15em><#F5DC4E>Time is up!</voffset>"
                                                        +"<br><line-height=50%><size=+10>The qualifier is over! Check our Discord for more info!"
                                                        +"<br>Link:<#F5DC4E> dsc.gg/zeepkist-showdown"
                                                        +"<br><#FFFFFF>For more info use <#F5DC4E>!qualiinfo <#FFFFFF>in chat";
        ChatApi.SendMessage(baseServerMessage);
    }

    public static void RequestHost(string username){
        ChatApi.SendMessage($" {username}, I got disconnected from the lobby, can you please give me back host?");
        ChatApi.SendMessage($"If in 5 minutes I don't have host, I will create a new official showdown lobby!");
    }

    public static void RemoveJoinServerMessages(){
        ChatApi.SendMessage("/servermessage remove");
        ChatApi.SendMessage("/joinmessage off");
    }


}