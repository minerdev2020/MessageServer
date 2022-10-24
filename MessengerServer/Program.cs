using System.Net;
using System.Net.Sockets;
using MessengerServer;
using MessengerServer.Model;

class Program
{
    static void Main(string[] args)
    {
        RunAsyncSocketServer().Wait();
    }

    static async Task RunAsyncSocketServer()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 7000);
        socket.Bind(ipEndPoint);

        socket.Listen(100);

        while (true)
        {
            Socket clientSocket = await socket.AcceptAsync();
            MainLoop(new SocketWrapper(clientSocket));
        }
    }

    static async Task MainLoop(SocketWrapper socketWrapper)
    {
        Console.WriteLine(socketWrapper.IpAddress + " Connected!");

        while (true)
        {
            try
            {
                Request? request = await socketWrapper.ReceiveAsync();
                if (request != null)
                {
                    bool isQuit = RequestHandler.Invoke(socketWrapper, request);
                    if (isQuit)
                    {
                        Console.WriteLine(socketWrapper.IpAddress + " Disconnected!");
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