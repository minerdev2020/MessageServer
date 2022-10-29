using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class JoinRoomHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var userId = data?["userId"]?.GetValue<int>() ?? 0;
        var roomId = data?["roomId"]?.GetValue<int>() ?? 0;

        if (userId == 0 || roomId == 0)
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
            long userCount;
            long roomCount;
            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                var q1 = $"SELECT COUNT(*) FROM user WHERE id = {userId}";
                var cmd1 = new MySqlCommand(q1, conn);
                userCount = cmd1.ExecuteScalar() as long? ?? 0;

                var q2 = $"SELECT COUNT(*) FROM room WHERE id = {roomId}";
                var cmd2 = new MySqlCommand(q2, conn);
                roomCount = cmd2.ExecuteScalar() as long? ?? 0;
            }

            if (userCount == 1 && roomCount == 1)
            {
                Console.WriteLine("Found!");

                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

                    var insertQuery = $"INSERT INTO user_room(user_id, room_id) VALUES ({userId}, {roomId})";
                    var cmd = new MySqlCommand(insertQuery, conn);
                    cmd.ExecuteNonQuery();
                    result = true;
                }
            }

            else
            {
                Console.WriteLine("Not Found!");
                result = false;
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