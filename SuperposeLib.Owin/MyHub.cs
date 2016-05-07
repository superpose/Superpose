using Microsoft.AspNet.SignalR;

namespace SuperposeLib.Owin
{
    public class MyHub : Hub
    {
        public void Send( string message)
        {
            Clients.All.addMessage( message);
        }
    }
}