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
        ModifyMyInfo,
        SendMessage,
        GetRoomList,
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        ModifyRoom,
        AddFriend,
        GetFriendList
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