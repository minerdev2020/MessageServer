using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class AddFriendHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var myId = data?["id"]?.GetValue<int>() ?? 0;
        var friendId = data?["targetId"]?.GetValue<int>() ?? 0;

        if (myId == 0 || friendId == 0)
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
            string responseData = "";
            bool result;
            long count;
            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                var q = $"SELECT COUNT(*) FROM user WHERE id IN ({myId}, {friendId})";
                var cmd = new MySqlCommand(q, conn);
                count = cmd.ExecuteScalar() as long? ?? 0;
            }

            if (count == 2)
            {
                Console.WriteLine("Found!");

                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

                    var insertQuery1 = $"INSERT INTO user_friend(user_id, friend_id) VALUES ({myId}, {friendId})";
                    var cmd1 = new MySqlCommand(insertQuery1, conn);
                    cmd1.ExecuteNonQuery();

                    var insertQuery2 = $"INSERT INTO user_friend(user_id, friend_id) VALUES ({friendId}, {myId})";
                    var cmd2 = new MySqlCommand(insertQuery2, conn);
                    cmd2.ExecuteNonQuery();
                }

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
                Data = responseData,
                Result = result
            };

            socketWrapper.SendResponseAsync(response);
        }

        return false;
    }
}