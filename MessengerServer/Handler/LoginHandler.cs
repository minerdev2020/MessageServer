using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class LoginHandler : IBaseHandler
{
    public async void Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Data);
        var email = data?["Email"];
        var password = data?["Password"];

        if (email == null || password == null)
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

            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    Console.WriteLine("Found!");
                    var id = dataSet.Tables[0].Rows[0]["id"] as int? ?? 0;
                    var updateQuery = $"UPDATE user SET online = 1 WHERE id = {id}";
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

                    responseData = JsonSerializer.Serialize(user);
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
                Data = responseData,
                Result = result
            };

            socketWrapper.SendAsync(response);
        }
    }
}