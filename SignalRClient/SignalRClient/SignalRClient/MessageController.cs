using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient
{
    public class MessageController
    {
        #region Members

        #region SirgnalR connection
        IHubProxy mainHubProxy;
        HubConnection conn;
        private string connectionUrl = "https://sendpersons.azurewebsites.net/";
        #endregion

        #region Incoming data delegates
        public Action<string, int> personDataRecieved;
        public Action<string, bool> connectionNewsReceived;
        #endregion

        #region Client id
        public readonly int clientID;
        #endregion
    
        #endregion

        #region Class instance
        private static MessageController instance;
        public static MessageController Instance()
        {
            if (instance == null)
            {
                instance = new MessageController();
            }
            return instance;
        }
        #endregion

        private int GenerateClientID()
        {
            Random r = new Random();
            return r.Next(10000);
        }

        private void RecieveNewPerson(string personData, int senderClientID)
        {
            personDataRecieved(personData, senderClientID);
        }

        private void RecevieConnectionNews(string message, bool isNewConnectoion)
        {
            connectionNewsReceived(message, isNewConnectoion);
        }

        public void SendNewPerson(string personData)
        {
            mainHubProxy.Invoke("SendNewPerson", personData);
        }

        private MessageController()
        {
            clientID = GenerateClientID();

            conn = new HubConnection(connectionUrl);

            mainHubProxy = conn.CreateHubProxy("MainHub");
            mainHubProxy["id"] = clientID;

            mainHubProxy.On<string, int>("RecieveNewPerson", (personData, senderClientID) =>
            {
                this.RecieveNewPerson(personData, senderClientID);
            });
            mainHubProxy.On<string, bool>("RecevieConnectionNews", (message, isNewConnection) =>
            {
                this.RecevieConnectionNews(message, isNewConnection);
            });

            conn.StateChanged += (connectionState) =>
            {
                if(connectionState.NewState == ConnectionState.Connected)
                {
                    mainHubProxy.Invoke("NotifyAllAboutNewConnection");
                }
            };
            conn.ConnectionSlow += () =>
            {
                RecevieConnectionNews("Slow connection determined.", false);
            };
            conn.Closed += () =>
            {
                RecevieConnectionNews("SignalR connection has been closed.", false);
            };
            conn.Start();
        }
    }
}
