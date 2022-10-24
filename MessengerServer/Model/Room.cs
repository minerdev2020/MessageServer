using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public class Room
{
    [JsonInclude] public int Id;
    [JsonInclude] public string Title = String.Empty;
    [JsonInclude] public List<int> MemberList = new();
}