namespace MessengerServer;

class Program
{
    static void Main(string[] args)
    {
        Server server = new Server();
        server.RunAsyncSocketServer().Wait();
    }
}