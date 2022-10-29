using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessengerServer.Model;

namespace MessengerServer;

public class SocketWrapper
{
    private const int MAX_SIZE = 1024;

    private readonly Socket _socket;
    public readonly string? IpAddress;

    public SocketWrapper(Socket socket)
    {
        _socket = socket;
        IpAddress = (_socket.RemoteEndPoint as IPEndPoint)?.ToString();
    }

    public void Close()
    {
        _socket.Close();
    }

    public async Task<Request?> ReceiveRequestAsync()
    {
        byte[] buffer = new byte[MAX_SIZE];
        int length = await _socket.ReceiveAsync(buffer, SocketFlags.None);
        if (length > 0)
        {
            string json = Encoding.UTF8.GetString(buffer, 0, length);
            Console.WriteLine("Receive : " + json);

            Request? request = JsonSerializer.Deserialize<Request>(json, Program.JsonSerializerOptions);
            return request;
        }

        return null;
    }

    public async void SendResponseAsync(Response response)
    {
        string json = JsonSerializer.Serialize(response, Program.JsonSerializerOptions);
        Console.WriteLine("Send : " + json);

        byte[] buffer = Encoding.UTF8.GetBytes(json);
        await _socket.SendAsync(buffer, SocketFlags.None);
    }

    public async void SendMessageAsync(int userId, string message)
    {
        Console.WriteLine("Send Message : " + message + " From " + userId);
        byte[] buffer = Encoding.UTF8.GetBytes(userId + ":" + message);
        await _socket.SendAsync(buffer, SocketFlags.None);
    }
}