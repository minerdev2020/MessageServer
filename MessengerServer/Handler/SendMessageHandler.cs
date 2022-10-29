using System.Data;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class SendMessageHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var userId = data?["userId"]?.GetValue<int>() ?? 0;
        var roomId = data?["roomId"]?.GetValue<int>() ?? 0;
        var message = data?["message"]?.GetValue<string>() ?? "";

        if (userId == 0 || roomId == 0 || message == "")
        {
            Response response = new Response
            {
                Id = request.Id,
                EventTime = DateTime.UtcNow,
                Data = "",
                Result = false
            };

            socketWrapper.SendResponseAsync(response);
        }

        else
        {
            bool result;
            await using (var conn = MySqlManager.GetConnection())
            {
                var q = $"SELECT * FROM user JOIN user_room ON user.id = user_room.user_id WHERE room_id = {roomId}";
                var adapter = new MySqlDataAdapter(q, conn);

                var dataSet = new DataSet();
                await adapter.FillAsync(dataSet);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    var id = row["id"] as int? ?? 0;
                    var ip = row["ip"] as string;
                    var online = row["online"] as bool? ?? false;

                    if (id == 0 || ip == null)
                    {
                        continue;
                    }

                    if (id != userId && online)
                    {
                        try
                        {
                            Program.SocketDictionary[ip].SendMessageAsync(userId, message);
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }

                result = true;
            }

            Response response = new Response
            {
                Id = request.Id,
                EventTime = DateTime.UtcNow,
                Data = "",
                Result = result
            };

            socketWrapper.SendResponseAsync(response);
        }

        return false;
    }
}