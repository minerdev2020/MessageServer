using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class RegisterHandler : IBaseHandler
{
    public async void Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Data);
        var email = data?["Email"];
        var password = data?["Password"];
        var name = data?["Name"];

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
                var q = $"SELECT * FROM user WHERE email = '{email}'";
                var adapter = new MySqlDataAdapter(q, conn);
                await adapter.FillAsync(dataSet);
            }

            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();
                
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    Console.WriteLine("Found!");
                    result = false;
                }

                else
                {
                    Console.WriteLine("Not Found!");
                    var insertQuery = $"INSERT INTO user(email, password, name) VALUES ('{email}', '{encryptedPassword}', '{name}')";
                    var cmd = new MySqlCommand(insertQuery, conn);
                    cmd.ExecuteNonQuery();
                    result = true;
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