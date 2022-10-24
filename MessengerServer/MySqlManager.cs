using MySql.Data.MySqlClient;

namespace MessengerServer;

public static class MySqlManager
{
    private const string ConnectionString = "Server=localhost;Database=message_server;Uid=root;Pwd=minerdev12!A";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }
}