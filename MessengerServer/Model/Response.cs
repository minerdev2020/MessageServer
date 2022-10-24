using System.Text.Json.Serialization;

namespace MessengerServer.Model;

public class Response
{
    [JsonInclude] public long Id;
    [JsonInclude] public DateTime EventTime = DateTime.UtcNow;
    [JsonInclude] public string Data = String.Empty;
    [JsonInclude] public bool Result;

    public override string ToString()
    {
        return $"Id : {Id}, EventTime : {EventTime}, Data : {Data}, Result : {Result}";
    }
}