using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public class User
{
    [JsonInclude] public int Id;
    [JsonInclude] public string Email = String.Empty;
    [JsonInclude] public string Password = String.Empty;
    [JsonInclude] public string Name = String.Empty;
    [JsonInclude] public bool Online;
}