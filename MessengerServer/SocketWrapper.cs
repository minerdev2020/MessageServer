using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessengerServer.Model;

namespace MessengerServer;

public class SocketWrapper
{
    private const int MaxSize = 1024;

    private readonly byte[] _receiveBuffer = new byte[MaxSize];
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
        int length = await _socket.ReceiveAsync(_receiveBuffer, SocketFlags.None);
        if (length > 0)
        {
            string json = Encoding.UTF8.GetString(_receiveBuffer, 0, length);
            Console.WriteLine("Receive : " + json);

            Request? request = JsonSerializer.Deserialize<Request>(json, Server.JsonSerializerOptions);
            return request;
        }

        return null;
    }

    public async void SendResponseAsync(Response response)
    {
        string json = JsonSerializer.Serialize(response, Server.JsonSerializerOptions);
        Console.WriteLine("Send : " + json);
        await _socket.SendAsync(Encoding.UTF8.GetBytes(json), SocketFlags.None);
    }

    public async void SendMessageAsync(int userId, string message)
    {
        Console.WriteLine("Send Message : " + message + " From " + userId);
        await _socket.SendAsync(Encoding.UTF8.GetBytes(userId + ":" + message), SocketFlags.None);
    }
}