using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MessengerServer.Model;

namespace MessengerServer;

public class Server
{
    public static readonly Dictionary<string, SocketWrapper> SocketDictionary = new();

    public static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task RunAsyncSocketServer()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 7000);
        socket.Bind(ipEndPoint);

        socket.Listen(Constant.MaxConnection);

        while (true)
        {
            SocketWrapper socketWrapper = new SocketWrapper(await socket.AcceptAsync());

            if (socketWrapper.IpAddress != null)
            {
                SocketDictionary[socketWrapper.IpAddress] = socketWrapper;
                MainLoop(socketWrapper);
            }
        }
    }

    private async Task MainLoop(SocketWrapper socketWrapper)
    {
        Console.WriteLine(socketWrapper.IpAddress + " Connected!");

        while (true)
        {
            try
            {
                Console.WriteLine(socketWrapper.IpAddress + " " + Thread.CurrentThread.ManagedThreadId);

                Request? request = await socketWrapper.ReceiveRequestAsync();

                if (request != null)
                {
                    bool isQuit = await RequestHandler.Invoke(socketWrapper, request);

                    if (isQuit)
                    {
                        if (socketWrapper.IpAddress != null)
                        {
                            SocketDictionary.Remove(socketWrapper.IpAddress);
                            Console.WriteLine(socketWrapper.IpAddress + " Disconnected!");
                        }

                        break;
                    }
                }
            }

            catch (SocketException)
            {
                Console.WriteLine(socketWrapper.IpAddress + " Disconnected!");
                break;
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(socketWrapper.IpAddress + " Disconnected!");
                break;
            }
        }

        socketWrapper.Close();
    }
}