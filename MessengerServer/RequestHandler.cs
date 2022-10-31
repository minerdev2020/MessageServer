using System.Text.Json.Nodes;
using MessengerServer.Handler;
using MessengerServer.Model;

namespace MessengerServer;

public static class RequestHandler
{
    private static readonly Dictionary<Request.RequestType, IBaseHandler> HandlerDictionary = new();

    static RequestHandler()
    {
        HandlerDictionary[Request.RequestType.Quit] = new QuitHandler();
        HandlerDictionary[Request.RequestType.Login] = new LoginHandler();
        HandlerDictionary[Request.RequestType.Logout] = new LogoutHandler();
        HandlerDictionary[Request.RequestType.Register] = new RegisterHandler();
        HandlerDictionary[Request.RequestType.ModifyMyInfo] = new ModifyMyInfoHandler();
        HandlerDictionary[Request.RequestType.SendMessage] = new SendMessageHandler();
        HandlerDictionary[Request.RequestType.GetRoomList] = new GetRoomListHandler();
        HandlerDictionary[Request.RequestType.CreateRoom] = new CreateRoomHandler();
        HandlerDictionary[Request.RequestType.JoinRoom] = new JoinRoomHandler();
        HandlerDictionary[Request.RequestType.LeaveRoom] = new LeaveRoomHandler();
        HandlerDictionary[Request.RequestType.ModifyRoom] = new ModifyRoomHandler();
        HandlerDictionary[Request.RequestType.AddFriend] = new AddFriendHandler();
        HandlerDictionary[Request.RequestType.GetFriendList] = new GetFriendListHandler();
    }

    public static async Task<bool> Invoke(SocketWrapper socketWrapper, Request request)
    {
        if (HandlerDictionary.ContainsKey(request.Type))
        {
            if (CheckNeedLogin(request.Type) && !socketWrapper.IsLogin)
            {
                Response response = new Response
                {
                    Id = request.Id,
                    EventTime = DateTime.UtcNow,
                    Data = new JsonObject { ["error"] = "Bad Request" }.ToJsonString(),
                    Result = false
                };
                Console.WriteLine(response.ToString());
                socketWrapper.SendResponseAsync(response);

                return false;
            }

            return await HandlerDictionary[request.Type].Invoke(socketWrapper, request);
        }

        return false;
    }

    private static bool CheckNeedLogin(Request.RequestType type)
    {
        return type is not (Request.RequestType.Login or Request.RequestType.Register or Request.RequestType.Quit);
    }
}