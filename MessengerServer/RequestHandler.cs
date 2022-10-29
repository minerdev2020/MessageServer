using MessengerServer.Handler;
using MessengerServer.Model;

namespace MessengerServer;

public static class RequestHandler
{
    private static readonly Dictionary<Request.RequestType, IBaseHandler> Dictionary = new();

    static RequestHandler()
    {
        Dictionary[Request.RequestType.Quit] = new QuitHandler();
        Dictionary[Request.RequestType.Login] = new LoginHandler();
        Dictionary[Request.RequestType.Logout] = new LogoutHandler();
        Dictionary[Request.RequestType.Register] = new RegisterHandler();
        Dictionary[Request.RequestType.ModifyMyInfo] = new ModifyMyInfoHandler();
        Dictionary[Request.RequestType.SendMessage] = new SendMessageHandler();
        Dictionary[Request.RequestType.GetRoomList] = new GetRoomListHandler();
        Dictionary[Request.RequestType.CreateRoom] = new CreateRoomHandler();
        Dictionary[Request.RequestType.JoinRoom] = new JoinRoomHandler();
        Dictionary[Request.RequestType.LeaveRoom] = new LeaveRoomHandler();
        Dictionary[Request.RequestType.ModifyRoom] = new ModifyRoomHandler();
        Dictionary[Request.RequestType.AddFriend] = new AddFriendHandler();
        Dictionary[Request.RequestType.GetFriendList] = new GetFriendListHandler();
    }

    public static async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        if (Dictionary.ContainsKey(request.Type))
        {
            return await Dictionary[request.Type].Invoke(socketWrapper, request);
        }

        return false;
    }
}