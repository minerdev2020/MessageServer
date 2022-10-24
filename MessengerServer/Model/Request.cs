using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public class Request
{
    public enum RequestType
    {
        Unknown,
        Quit,
        Login,
        Logout,
        Register,
        SendMessage,
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        ModifyRoom
    }

    [JsonInclude] public long Id;
    [JsonInclude] public DateTime EventTime;
    [JsonInclude] public RequestType Type = RequestType.Unknown;
    [JsonInclude] public string Data = String.Empty;

    public override string ToString()
    {
        return $"Id : {Id}, EventTime : {EventTime}, Type : {Type}, Data : {Data}";
    }
}