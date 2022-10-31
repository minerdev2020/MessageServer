using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class GetFriendListHandler : IBaseHandler
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
            string responseData;
            bool result;
            await using (var conn = MySqlManager.GetConnection())
            {
                var q = $"SELECT * FROM user JOIN user_friend ON user.id = user_friend.friend_id WHERE user_id = {id}";
                var adapter = new MySqlDataAdapter(q, conn);

                var dataSet = new DataSet();
                await adapter.FillAsync(dataSet);

                var friendList = new List<User>();
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    friendList.Add(new User
                    {
                        Id = row["id"] as int? ?? 0,
                        Email = row["email"] as string ?? "",
                        Name = row["name"] as string ?? "",
                        Online = row["online"] as byte? == 1
                    });
                }

                responseData = new JsonObject
                {
                    ["friendList"] = JsonSerializer.Serialize(friendList, Server.JsonSerializerOptions)
                }.ToJsonString();

                result = true;
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