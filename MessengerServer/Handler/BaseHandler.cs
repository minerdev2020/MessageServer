using MessengerServer.Model;

namespace MessengerServer.Handler;

public interface IBaseHandler
{
    public void Invoke(SocketWrapper socketWrapper, Request request);
}