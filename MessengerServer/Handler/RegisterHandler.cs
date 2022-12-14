using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using MessengerServer.Model;
using MySql.Data.MySqlClient;

namespace MessengerServer.Handler;

public class RegisterHandler : IBaseHandler
{
    public async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        var data = JsonNode.Parse(request.Data);
        var email = data?["email"]?.GetValue<string>() ?? "";
        var password = data?["password"]?.GetValue<string>() ?? "";
        var name = data?["name"]?.GetValue<string>() ?? "";

        if (email == "" || password == "" || name == "")
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
            long count;
            await using (var conn = MySqlManager.GetConnection())
            {
                conn.Open();

                var q = $"SELECT COUNT(*) FROM user WHERE email = '{email}'";
                var cmd = new MySqlCommand(q, conn);
                count = cmd.ExecuteScalar() as long? ?? 0;
            }

            if (count > 0)
            {
                Console.WriteLine("Found!");
                result = false;
            }

            else
            {
                await using (var conn = MySqlManager.GetConnection())
                {
                    conn.Open();

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

            socketWrapper.SendResponseAsync(response);
        }

        return false;
    }
}