using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class LogoutHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var id = data?["id"]?.GetValue<int>() ?? 0;

        if (id == 0)
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
            long count;
            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                var q = $"SELECT COUNT(*) FROM user WHERE id = {id}";
                var cmd = new MySqlCommand(q, conn);
                count = cmd.ExecuteScalar() as long? ?? 0;
            }

            if (count > 0)
            {
                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

                    Console.WriteLine("Found!");
                    var updateQuery = $"UPDATE user SET online = 0 WHERE id = {id}";
                    var cmd = new MySqlCommand(updateQuery, conn);
                    cmd.ExecuteNonQuery();
                    result = true;
                }

                socketWrapper.IsLogin = false;
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