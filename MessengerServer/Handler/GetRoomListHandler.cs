using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class GetRoomListHandler : IBaseHandler
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
            var roomList = new List<Room>();
            await using (var conn = MySqlManager.GetConnection())
            {
                var q = $"SELECT * FROM room JOIN user_room ON room.id = user_room.room_id WHERE user_id = {id}";
                var adapter = new MySqlDataAdapter(q, conn);

                var dataSet = new DataSet();
                await adapter.FillAsync(dataSet);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    roomList.Add(new Room
                    {
                        Id = row["id"] as int? ?? 0,
                        Title = row["title"] as string ?? "",
                    });
                }
            }

            foreach (Room room in roomList)
            {
                var roomId = room.Id;
                var memberList = new List<int>();
                await using (var conn = MySqlManager.GetConnection())
                {
                    var q = $"SELECT * FROM user_room WHERE room_id = {roomId}";
                    var adapter = new MySqlDataAdapter(q, conn);

                    var dataSet = new DataSet();
                    await adapter.FillAsync(dataSet);

                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        memberList.Add(row["user_id"] as int? ?? 0);
                    }
                }

                room.MemberList = memberList;
            }

            responseData = new JsonObject
            {
                ["roomList"] = JsonSerializer.Serialize(roomList, Server.JsonSerializerOptions)
            }.ToJsonString();

            result = true;

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