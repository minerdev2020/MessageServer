using MessengerServer.Model;

namespace MessengerServer.Handler;

public interface IBaseHandler
{
    public Task<bool> Invoke(SocketWrapper socketWrapper, Request request);
}