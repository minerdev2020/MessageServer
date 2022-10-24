using MessengerServer.Handler;
using MessengerServer.Model;

namespace MessengerServer;

public static class RequestHandler
{
    public static bool Invoke(SocketWrapper socketWrapper, Request request)
    {
        switch (request.Type)
        {
            case Request.RequestType.Unknown:
                break;
            case Request.RequestType.Quit:
                new QuitHandler().Invoke(socketWrapper, request);
                return true;
            case Request.RequestType.Login:
                new LoginHandler().Invoke(socketWrapper, request);
                break;
            case Request.RequestType.Logout:
                new LogoutHandler().Invoke(socketWrapper, request);
                break;
            case Request.RequestType.Register:
                new RegisterHandler().Invoke(socketWrapper, request);
                break;
            case Request.RequestType.SendMessage:
                break;
            case Request.RequestType.CreateRoom:
                break;
            case Request.RequestType.JoinRoom:
                break;
            case Request.RequestType.LeaveRoom:
                break;
            case Request.RequestType.ModifyRoom:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }
}