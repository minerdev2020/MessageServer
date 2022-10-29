using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class CreateRoomHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var memberListString = data?["memberList"]?.GetValue<string>() ?? string.Empty;

        if (memberListString == "")
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
            var memberList = JsonSerializer.Deserialize<List<int>>(memberListString) ?? new List<int>();

            string responseData = "";
            bool result;
            long count;
            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                var memberQuery = string.Join(",", memberList.Select(i => $"{i}"));
                var q = $"SELECT COUNT(*) FROM user WHERE id IN ({memberQuery})";
                var cmd = new MySqlCommand(q, conn);
                count = cmd.ExecuteScalar() as long? ?? 0;
            }

            if (count != memberList.Count)
            {
                Console.WriteLine("Not Found!");
                result = false;
            }

            else
            {
                Console.WriteLine("Found!");

                long roomId;
                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

                    var insertQuery = "INSERT INTO room(title) VALUES ('default title')";
                    var cmd = new MySqlCommand(insertQuery, conn);
                    cmd.ExecuteNonQuery();
                    roomId = cmd.LastInsertedId;
                }

                if (roomId != 0)
                {
                    await using (var conn = MySqlManager.GetConnection())
                    {
                        conn.Open();

                        foreach (int member in memberList)
                        {
                            var insertQuery = $"INSERT INTO user_room(user_id, room_id) VALUES ({member}, {roomId})";
                            var cmd = new MySqlCommand(insertQuery, conn);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    responseData = new JsonObject
                    {
                        ["id"] = roomId
                    }.ToJsonString();

                    result = true;
                }

                else
                {
                    result = false;
                }
            }

            Response response = new Response
            {
                Id = request.Id,
                EventTime = DateTime.UtcNow,
                Data = responseData,
                Result = result
            };

            socketWrapper.SendResponseAsync(response);
        }

        return false;
    }
}