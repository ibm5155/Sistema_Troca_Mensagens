using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Net;

using ChatApplication;
using System.Threading;
using System.Globalization;

namespace ChatClient
{

    public partial class Client : Form
    {

        private struct ClientStruct
        {
            public EndPoint endPoint;
            public string name;
            public string IP;
            public string Porta;
            public int Id;
        }

        private List<ClientStruct> clientList = new List<ClientStruct>();

        #region Private Members

        // Request Close Window
        // Client socket
        private Socket clientSocket;

        // Client name
        private string name;

        private bool clientLeader = false;
        private int myId = -1;
        private int ElectionOKCount = 0;
        private bool leaderAlive = false;
        private int leaderId = -1;

        // Server End Point
        private EndPoint epServer;
        EndPoint epClient;

        // Data stream
        private byte[] dataStream = new byte[1024];

        // Display message delegate
        private delegate void DisplayMessageDelegate(string message);
        private DisplayMessageDelegate displayMessageDelegate = null;

        #endregion

        #region Constructor

        public Client()
        {
            InitializeComponent();
            var all =  new ClientStruct();
            all.name = "All";
            clientList.Add(all);
            userslistbox.Items.Add(all.name);
            CheckLeaderThread();
        }

        #endregion

        #region Events

        private void Client_Load(object sender, EventArgs e)
        {
            // Initialise delegate
            this.displayMessageDelegate = new DisplayMessageDelegate(this.DisplayMessage);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (clientLeader == false)
                {
                    this.Invoke(this.displayMessageDelegate, new object[] { "Você não é o lider então não poderá enviar reequisições" });
                    return;
                }
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatName", this.name);
                sendData.ReadData.Add("ChatMessage" ,txtMessage.Text.Trim());
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.Message);

                // Get packet as byte array

                if(ToStr.Text == "All" || ToStr.Text == "")
                {
                    byte[] byteData = sendData.GetDataStream();
                    // Send packet to the server
                    IPEndPoint client = null;
                    sendData.ReadData["ChatMessage"] = this.name + ":" + sendData.ReadData["ChatMessage"];
                    for (int id=1; id < clientList.Count(); id++)
                    {
                            client = CreateIPEndPoint(clientList[id].IP);
                            // Initialise the EndPoint for the client
                            epClient = (EndPoint)client;
                            byteData = sendData.GetDataStream();
                            clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                    }
                    this.Invoke(this.displayMessageDelegate, new object[] { sendData.ReadData["ChatMessage"] });
                }
                else
                {
                    int id = 0;
                    IPEndPoint client = null;
                    for (;id< clientList.Count(); id++)
                    {
                        if(clientList[id].name == ToStr.Text)
                        {
                            client = CreateIPEndPoint(clientList[id].IP);
                        }
                    }
                    if(client != null)
                    {
                        // Initialise the EndPoint for the client
                        epClient = (EndPoint)client;
                        sendData.ReadData["ChatMessage"] = "*" + this.name + ":" + sendData.ReadData["ChatMessage"];
                        byte[] byteData = sendData.GetDataStream();
                        clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                        this.Invoke(this.displayMessageDelegate, new object[] { sendData.ReadData["ChatMessage"] });
                    }
                }


                txtMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.clientSocket != null)
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.LogOut);
                    sendData.ReadData.Add("ChatName", this.name);

                    // Get packet as byte array
                    byte[] byteData = sendData.GetDataStream();

                    // Send packet to the server
                    this.clientSocket.SendTo(byteData, 0, byteData.Length, SocketFlags.None, epServer);

                    // Close the socket
                    this.clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Closing Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.name = txtName.Text.Trim();
                
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatName", this.name);
                sendData.ReadData.Add("ChatDataIdentifier" , DataIdentifier.LogIn);
                sendData.ReadData.Add("ChatPassword", this.txtPassw.Text);
                myId = int.Parse(txtId.Text);
                sendData.ReadData.Add("ChatId", myId);

                // Initialise socket
                this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise server IP
                IPAddress serverIP = IPAddress.Parse(txtServerIP.Text.Trim());

                // Initialise the IPEndPoint for the server and use port 30000
                IPEndPoint server = new IPEndPoint(serverIP, 30000);

                // Initialise the EndPoint for the server
                epServer = (EndPoint)server;

                // Get packet as byte array
                byte[] data = sendData.GetDataStream();

                // Send data to server
                clientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, epServer, new AsyncCallback(this.SendData), null);

                this.dataStream = new byte[1024];
                // Begin listening for broadcasts
                clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Send And Receive

        private void SendData(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveData(IAsyncResult ar)
        {
            var t = new Thread(() =>
            {
                try
                {
                    // Receive all data
                    this.clientSocket.EndReceive(ar);

                    // Initialise a packet object to store the received data

                    Packet receivedData = new Packet(this.dataStream);

                    DataIdentifier ChatDataIdentifier = receivedData.GetDataIdentifier;
                    if (ChatDataIdentifier == DataIdentifier.OK)
                    {
                        this.Invoke(this.displayMessageDelegate, new object[] { "You Logged in" });
                    }
                    else if (ChatDataIdentifier == DataIdentifier.DuplicateId)
                    {
                        MessageBox.Show("Theres already a user logged with this id.", "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Close the socket
                        this.clientSocket.Close();
                    }
                    else if (ChatDataIdentifier == DataIdentifier.WrongPassw)
                    {
                        MessageBox.Show("Wrong Password", "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Close the socket
                        this.clientSocket.Close();
                    }
                    else if (ChatDataIdentifier == DataIdentifier.ELECTION)
                    {
                        int chatId = receivedData.GetInt("ChatId");
                        if (myId > chatId)
                        {
                            SendOkElection(chatId);
                            StartElection();
                        }
                        else
                        {
                            ElectionOKCount++;
                        }
                        receivedData.ReadData["ChatName"] = name;
                    }
                    else if (ChatDataIdentifier == DataIdentifier.ElectionOK)
                    {
                        ElectionOKCount++;
                        this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatName"] + " tem nivel maior" });
                        receivedData.ReadData["ChatName"] = name;
                        clientLeader = false;

                        updatetxtlider("Escravo");
                    }
                    else if (ChatDataIdentifier == DataIdentifier.Coordinator)
                    {
                        int chatId = receivedData.GetInt("ChatId");
                        if (myId > chatId)
                        {
                            this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatName"] + " acha que é o coordenador mas eu tenho id maior, resolvendo isso" });
                            receivedData.ReadData["ChatName"] = name;
                            SendOkElection(chatId);
                        }
                        else
                        {
                            this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatName"] + " é o coordenador" });
                            clientLeader = false;
                            updatetxtlider("Escravo");
                            receivedData.ReadData["ChatName"] = name;
                        }
                        receivedData.ReadData["ChatName"] = name;
                    }
                    else if (ChatDataIdentifier == DataIdentifier.AmAlive)
                    {
                        leaderAlive = true;
                        this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatName"] + " Lider está vivo " });
                        receivedData.ReadData["ChatName"] = name;
                    }
                    else if (ChatDataIdentifier == DataIdentifier.IsAlive)
                    {
                        leaderAlive = true;
                        this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatName"] + " Lider está vivo " });
                        receivedData.ReadData["ChatName"] = name;
                        SendIsAlive(receivedData.GetInt("ChatId"));
                    }

                    else if (ChatDataIdentifier == DataIdentifier.UpdateList)
                    {
                        var c = new ClientStruct();
                        c.name = receivedData.ReadData["ChatName"] as string;
                        c.IP = receivedData.ReadData["ChatIp"] as string;
                        c.Id = (receivedData.GetInt("ChatId"));
                        clientList.Add(c);
                        refresh_list();
                    }


                    else if (receivedData.ReadData["ChatName"].Equals(name)) ;//duplicate
                    else
                    {
                        // Update display through a delegate
                        if (receivedData.ReadData["ChatMessage"] != null)
                            this.Invoke(this.displayMessageDelegate, new object[] { receivedData.ReadData["ChatMessage"] });
                    }



                    // Reset data stream
                    this.dataStream = new byte[1024];

                    //check if its a new login
                    if (ChatDataIdentifier == DataIdentifier.LogIn)
                    {
                        ClientStruct c = new ClientStruct();
                        c.name = receivedData.ReadData["ChatName"] as string;
                        c.IP = receivedData.ReadData["ChatIp"] as string;
                        c.Id = receivedData.GetInt("ChatId");
                        clientList.Add(c);
                        refresh_list();
                    }
                    else if (ChatDataIdentifier == DataIdentifier.LogOut)
                    {
                        for (int i = 0; i < clientList.Count(); i++)
                        {
                            if (clientList[i].name.Equals(receivedData.ReadData["ChatName"]))
                            {
                                clientList.RemoveAt(i);
                                i = clientList.Count() + 10;
                            }
                        }
                        refresh_list();
                    }
                    // Continue listening for broadcasts
                    clientSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(this.ReceiveData), null);
                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                    MessageBox.Show("Receive Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
            t.IsBackground = true;
            t.Start();
        }

        #endregion

        #region Other Methods

        private void DisplayMessage(string messge)
        {
            rtxtConversation.Text += messge + Environment.NewLine;
        }

        #endregion

        private void userslistbox_Click(object sender, EventArgs e)
        {
            var x = sender as ListBox;
            ToStr.Text = (string)x.SelectedItem;
        }

        private void refresh_list()
        {
            var lu = new string[clientList.Count];
            for (int i=0; i < clientList.Count(); i++)
            {
                lu[i]  = clientList[i].name;
            }
            this.Invoke(new MethodInvoker(delegate ()
            {
                userslistbox.Items.Clear();
                userslistbox.Items.AddRange(lu);
            }));
        }

        // Handles IPv4 and IPv6 notation.
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }

        #region  eleição

        private bool eleicaorodando = false;
        private void SendOkElection(int usrid)
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
                        client = CreateIPEndPoint(clientList[id].IP);
                        id = clientList.Count();
                    }
                }
                // Initialise the EndPoint for the client
                epClient = (EndPoint)client;
                sendData.ReadData.Add("ChatMessage", "*" + this.name + ":" + "respondeu a " + usrname  +  " com (OK)");
                byte[] byteData = sendData.GetDataStream();
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                this.Invoke(this.displayMessageDelegate, new object[] { sendData.ReadData["ChatMessage"] });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendIsAlive(int usrid)
        {
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatName",this.name);
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
                        client = CreateIPEndPoint(clientList[id].IP);
                        id = clientList.Count();
                    }
                }
                // Initialise the EndPoint for the client
                epClient = (EndPoint)client;
                sendData.ReadData["ChatMessage"] = "*" + this.name + ":" + "respondeu a " + usrname + " que estou vivo";
                byte[] byteData = sendData.GetDataStream();
                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                this.Invoke(this.displayMessageDelegate, new object[] { sendData.ReadData["ChatMessage"] });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void StartElection()
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
                    sendData.ReadData.Add("ChatMessage", txtMessage.Text.Trim());
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
                            this.Invoke(this.displayMessageDelegate, new object[] { "Requisitando eleição para " + clientList[id].name });
                            client = CreateIPEndPoint(clientList[id].IP);
                            // Initialise the EndPoint for the client
                            epClient = (EndPoint)client;
                            byteData = sendData.GetDataStream();
                            clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                new AsyncCallback(this.SendData), null);
                        }
                    }
                    this.Invoke(this.displayMessageDelegate, new object[] { sendData.ReadData["ChatMessage"] });
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
                        updatetxtlider("Escravo");
                        this.Invoke(this.displayMessageDelegate, new object[] { "(não sou o lider)" });
                    }
                    else
                    {
                        //eu sou o lider
                        clientLeader = true;
                        updatetxtlider("Lider");
                        sendData.ReadData["ChatDataIdentifier"] = DataIdentifier.Coordinator;
                        this.Invoke(this.displayMessageDelegate, new object[] { "(Informando que sou o lider)" });
                        for (int id = 1; id < clientList.Count(); id++)
                        {
                            if (clientList[id].Id != myId)
                            {
                                client = CreateIPEndPoint(clientList[id].IP);
                                // Initialise the EndPoint for the client
                                epClient = (EndPoint)client;
                                byteData = sendData.GetDataStream();
                                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                    new AsyncCallback(this.SendData), null);
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

        private void ElectionRequested(object sender, EventArgs e)
        {
            StartElection();
        }

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
                                var client = CreateIPEndPoint(clientList[id].IP);
                                // Initialise the EndPoint for the client
                                epClient = (EndPoint)client;
                                var byteData = sendData.GetDataStream();
                                clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                    new AsyncCallback(this.SendData), null);
                            }
                        }
                        Thread.Sleep(1000);
                        if (leaderAlive == false)
                        {
                            this.Invoke(this.displayMessageDelegate, new object[] { "(lider morreu, requerindo uma eleição)" });
                            StartElection();
                        }
                    }
                }

            });
            t.IsBackground = true;
            t.Start();
        }

        public void updatetxtlider(string nome)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtLider.Text = nome;
            });
        }
    }
}
