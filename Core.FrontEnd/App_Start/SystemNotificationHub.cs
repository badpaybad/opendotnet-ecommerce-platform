using System;
using Microsoft.AspNet.SignalR;
    
namespace Core.FrontEnd
{
    public class SystemNotificationHub: Hub
    {
        public SystemNotificationHub()
        {
           // var now = DateTime.Now;
        }

        public void ClientConnected()
        {
            var id = Context.ConnectionId;
        }

        public void BroadCastMessage(NotificationMessage msg)
        {
            msg.OwnerId = Context.ConnectionId;

            Clients.All.boardCastMessage(msg);
        }
    }

    public class NotificationMessage
    {
        public string DataType;
        public string DataJson;
        public string OwnerId;
    }
}