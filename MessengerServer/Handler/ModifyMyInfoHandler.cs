using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class ModifyMyInfoHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var id = data?["id"]?.GetValue<int>() ?? 0;
        var newName = data?["newName"]?.GetValue<string>() ?? "";

        if (id == 0 || newName == "")
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

                var q = $"UPDATE user SET name = '{newName}' WHERE id = {id}";
                var cmd = new MySqlCommand(q, conn);
                count = cmd.ExecuteNonQuery();
            }

            if (count == 1)
            {
                Console.WriteLine("Found!");
                result = true;
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