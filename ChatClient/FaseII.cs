using ChatApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public class FaseII : Client
    {

        public bool AllowCriticalArea = false;//block the critical area

        #region External Vars
        private List<ClientData> clientList
        {
            set => Global.clientList = value;
            get => Global.clientList;
        }

        private Socket clientSocket
        {
            set => Global.clientSocket = value;
            get => Global.clientSocket;
        }

        // Client name
        private string name
        {
            set => Global.name = value;
            get => Global.name;
        }

        private bool clientLeader
        {
            set => Global.clientLeader = value;
            get => Global.clientLeader;
        }
        private int myId
        {
            set => Global.myId = value;
            get => Global.myId;
        }
        private int ElectionOKCount
        {
            set => Global.ElectionOKCount = value;
            get => Global.ElectionOKCount;
        }
        private bool leaderAlive
        {
            set => Global.leaderAlive = value;
            get => Global.leaderAlive;
        }
        private int leaderId
        {
            set => Global.leaderId = value;
            get => Global.leaderId;
        }

        // Server End Point
        public EndPoint epServer
        {
            set => Global.epServer = value;
            get => Global.epServer;
        }
        private EndPoint epClient
        {
            set => Global.epClient = value;
            get => Global.epClient;
        }

        // Data stream
        private static byte[] dataStream
        {
            set => Global.dataStream = value;
            get => Global.dataStream;
        }

        // Display message delegate
        public delegate void DisplayMessageDelegate(string message);
        public static DisplayMessageDelegate displayMessageDelegate = null;
        #endregion

        public FaseII()
        {
        }

        public void StartFaseII()
        {
            var rand = new Random();
            Thread t = new Thread(() =>
            {
                //Step 1 outside critical region
                Thread.Sleep(rand.Next(2000, 4000));//sleep btw 2s up to 4
                //Step 2

            });
        }


        public void SendRequestCriticalArea(int usrid)
        {
                try
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", this.name);
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.AmAlive);
                    sendData.ReadData.Add("ChatId", myId);
                    string usrname = "";

                    // Get packet as byte array
                    int id = 1;
                    IPEndPoint client = null;
                    for (; id < clientList.Count(); id++)
                    {
                        if (clientList[id].Id == usrid)
                        {
                            usrname = clientList[id].name;
                            client = IpData.CreateIPEndPoint(clientList[id].IP);
                            id = clientList.Count();
                        }
                    }
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    sendData.ReadData["ChatMessage"] = "*" + this.name + ":" + "respondeu a " + usrname + " que estou vivo";
                    byte[] byteData = sendData.GetDataStream();
                    clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(base.SendData), null);
                    base.GetNewLineLog =  sendData.ReadData["ChatMessage"] as string;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

    }
}
