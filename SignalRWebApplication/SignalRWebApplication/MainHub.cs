using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace SignalRWebApplication
{
    public class MainHub : Hub
    {
        public void SendNewPerson(string personJson)
        {
            Clients.All.RecieveNewPerson(personJson, Clients.Caller.id);
        }

        public void NotifyAllAboutNewConnection()
        {
            Clients.All.RecevieConnectionNews(string.Format("A new client is connected. Id: {0}.", Clients.Caller.id), true);
        }

        public void NotifyAllAboutClosedConnection()
        {
            Clients.All.RecevieConnectionNews(string.Format("A client has disconnected."), false);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            NotifyAllAboutClosedConnection();
            return base.OnDisconnected(stopCalled);
        }

    }
}