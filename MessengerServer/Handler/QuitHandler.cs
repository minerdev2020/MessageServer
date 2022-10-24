using System.Data;
using System.Text.Json;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class QuitHandler : IBaseHandler
{
    public async void Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Data);
        var id = data?["Id"];

        if (id == null)
        {
            Response response = new Response
            {
                Id = request.Id,
                EventTime = DateTime.UtcNow,
                Data = "",
                Result = false
            };

            socketWrapper.SendAsync(response);
        }

        else
        {
            bool result;
            var dataSet = new DataSet();
            await using (var conn = MySqlManager.GetConnection())
            {
                var q = $"SELECT * FROM user WHERE id = {id}";
                var adapter = new MySqlDataAdapter(q, conn);
                await adapter.FillAsync(dataSet);
            }

            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();
                
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    Console.WriteLine("Found!");
                    var updateQuery = $"UPDATE user SET online = 0 WHERE id = {id}";
                    var cmd = new MySqlCommand(updateQuery, conn);
                    cmd.ExecuteNonQuery();
                    result = true;
                }

                else
                {
                    Console.WriteLine("Not Found!");
                    result = false;
                }
            }

            Response response = new Response
            {
                Id = request.Id,
                EventTime = DateTime.UtcNow,
                Data = "",
                Result = result
            };

            socketWrapper.SendAsync(response);
        }
    }
}