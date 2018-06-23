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
    public class FaseI
    {
        public Client Client { get; internal set; }
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

        public FaseI(Client client)
        {
            this.Client = client;
        }

        #region  eleição

        public bool eleicaorodando = false;
        public void SendOkElection(int usrid)
        {
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatName", this.name);
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.ElectionOK);
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
                sendData.ReadData.Add("ChatMessage", "*" + this.name + ":" + "respondeu a " + usrname + " com (OK)");
                byte[] byteData = sendData.GetDataStream();
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.Client.SendData), null);
                this.Client.GetNewLineLog = sendData.ReadData["ChatMessage"] as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SendIsAlive(int usrid)
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
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.Client.SendData), null);
                this.Client.GetNewLineLog = sendData.ReadData["ChatMessage"] as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void StartElection()
        {
            if (eleicaorodando == true) return;
            eleicaorodando = true;
            var t = new Thread(() =>
            {
                try
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", this.name);
                    sendData.ReadData.Add("ChatMessage", this.Client.GettxtMessage());
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.ELECTION);
                    sendData.ReadData.Add("ChatId", myId);

                    // Get packet as byte array
                    byte[] byteData = sendData.GetDataStream();
                    // Send packet to the server
                    IPEndPoint client = null;
                    sendData.ReadData["ChatMessage"] = this.name + ":" + sendData.ReadData["ChatMessage"];
                    for (int id = 1; id < clientList.Count(); id++)
                    {
                        if (clientList[id].Id > myId && clientList[id].Id != myId)
                        {
                            this.Client.GetNewLineLog = "Requisitando eleição para " + clientList[id].name;
                            client = IpData.CreateIPEndPoint(clientList[id].IP);
                            // Initialise the EndPoint for the client
                            epClient = (EndPoint)client;
                            byteData = sendData.GetDataStream();
                            clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                new AsyncCallback(this.Client.SendData), null);
                        }
                    }
                    this.Client.GetNewLineLog = sendData.ReadData["ChatMessage"] as string;
                    var r = new Random();
                    for (int i = 0; i < 5000 + r.Next(1, 100); i++)
                    {
                        Thread.Sleep(1);
                        if (ElectionOKCount > 0)
                        {
                            i = 5000;
                        }
                    }
                    if (ElectionOKCount > 0)
                    {
                        //alguém superior quer ser o chefe
                        clientLeader = false;
                        this.Client.updatetxtlider("Escravo");
                        this.Client.GetNewLineLog = "(não sou o lider)";
                    }
                    else
                    {
                        //eu sou o lider
                        clientLeader = true;
                        this.Client.updatetxtlider("Lider");
                        sendData.ReadData["ChatDataIdentifier"] = DataIdentifier.Coordinator;
                        this.Client.GetNewLineLog = "(Informando que sou o lider)";
                        for (int id = 1; id < clientList.Count(); id++)
                        {
                            if (clientList[id].Id != myId)
                            {
                                client = IpData.CreateIPEndPoint(clientList[id].IP);
                                // Initialise the EndPoint for the client
                                epClient = (EndPoint)client;
                                byteData = sendData.GetDataStream();
                                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                    new AsyncCallback(this.Client.SendData), null);
                            }
                        }
                    }
                    ElectionOKCount = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                eleicaorodando = false;
            });
            t.IsBackground = true;
            t.Start();
        }

        #endregion

        Random r = new Random();
        public void CheckLeaderThread()
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(r.Next(10000, 50000));
                    if (epServer != null)
                    {
                        leaderAlive = false;

                        Packet sendData = new Packet();
                        sendData.ReadData.Add("ChatName", this.name);
                        sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.IsAlive);
                        sendData.ReadData.Add("ChatId", myId);
                        for (int id = 1; id < clientList.Count(); id++)
                        {
                            if (clientList[id].Id == leaderId)
                            {
                                var client = IpData.CreateIPEndPoint(clientList[id].IP);
                                // Initialise the EndPoint for the client
                                epClient = (EndPoint)client;
                                var byteData = sendData.GetDataStream();
                                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                    new AsyncCallback(this.Client.SendData), null);
                            }
                        }
                        Thread.Sleep(1000);
                        if (leaderAlive == false)
                        {
                            if(leaderId == myId)
                            {
                                leaderAlive = true;
                            }
                            else{
                                this.Client.GetNewLineLog = "(lider morreu, requerindo uma eleição)";
                                StartElection();
                            }
                        }
                    }
                }

            });
            t.IsBackground = true;
            t.Start();
        }

    }
}
