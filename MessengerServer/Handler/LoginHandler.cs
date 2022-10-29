using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class LoginHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var email = data?["email"]?.GetValue<string>() ?? "";
        var password = data?["password"]?.GetValue<string>() ?? "";

        if (email == "" || password == "")
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
            var encryptedPassword = "";
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                foreach (byte b in hashValue)
                {
                    encryptedPassword += b.ToString();
                }
            }

            string responseData = "";
            bool result;
            var dataSet = new DataSet();
            await using (var conn = MySqlManager.GetConnection())
            {
                var q = $"SELECT * FROM user WHERE email = '{email}' and password = '{encryptedPassword}'";
                var adapter = new MySqlDataAdapter(q, conn);
                await adapter.FillAsync(dataSet);
            }

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

                    Console.WriteLine("Found!");
                    var id = dataSet.Tables[0].Rows[0]["id"] as int? ?? 0;
                    var updateQuery = $"UPDATE user SET online = 1, ip = '{socketWrapper.IpAddress}' WHERE id = {id}";
                    var cmd = new MySqlCommand(updateQuery, conn);
                    cmd.ExecuteNonQuery();
                    result = true;

                    User user = new User
                    {
                        Id = dataSet.Tables[0].Rows[0]["id"] as int? ?? 0,
                        Email = dataSet.Tables[0].Rows[0]["email"] as string ?? "",
                        Name = dataSet.Tables[0].Rows[0]["name"] as string ?? "",
                        Online = true
                    };

                    responseData = JsonSerializer.Serialize(user, Program.JsonSerializerOptions);
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
                Data = responseData,
                Result = result
            };

            socketWrapper.SendResponseAsync(response);
        }

        return false;
    }
}