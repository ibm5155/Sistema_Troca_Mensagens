using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerProcess
{
    public partial class Form1 : Form
    {
        private List<int> ServerQueue = new List<int>();
        private static byte[] dataStream = new byte[1024];
        private ClientData _connectedClient;
        private ClientData _connectedCoordinator;
        private int myId = -1;
        private int leaderId = 3;
        private DataBase _dataBase;
        // Server End Point
        public static EndPoint epServer;
        public static EndPoint epClient;
        // Server socket
        private Socket serverSocket;



        private readonly object _criticalRegion = new object();
        public bool AllowCriticalArea = false;//block the critical area

        public Form1()
        {
            InitializeComponent();
        }
        #region Other Methods

        public void DisplayMessage(string messge)
        {
            lock (this)
            {
                Invoke((MethodInvoker)delegate
                {
                    rtxtConversation.Text += messge + Environment.NewLine;
                });
            }
        }

        #endregion



        #region Send And Receive

        public void SendData(IAsyncResult ar)
        {
            serverSocket.EndSend(ar);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            var t = new Thread(() =>
            {
            // Receive all data
            this.serverSocket.EndReceive(ar);

            // Initialise a packet object to store the received data

            Packet receivedData = new Packet(dataStream);

            DataIdentifier ChatDataIdentifier = receivedData.GetDataIdentifier;


            if (ChatDataIdentifier == DataIdentifier.Coordinator)
            {
                leaderId = receivedData.GetInt("ChatId");
            }
            else if (ChatDataIdentifier == DataIdentifier.Message)
            {
                    UserRequestInsertData(receivedData.ReadData["ChatMessage"] as string);
            }
            else if (ChatDataIdentifier == DataIdentifier.SiteIndex)
            {
                UserRequestGetIndex(receivedData.ReadData["ChatName"] as string, receivedData.ReadData["ChatIp"] as string, receivedData.GetInt("Index"));
            }
            else if (ChatDataIdentifier == DataIdentifier.SiteRelease)
            {
                    AllowCriticalArea = true;
            }
            else if (ChatDataIdentifier == DataIdentifier.SiteRequest)
            {
                SiteGetRequest(receivedData.GetInt("ChatId"));
            }
            else if (ChatDataIdentifier == DataIdentifier.UpdateList)
            {
                SiteGetUpdate(receivedData.ReadData["ChatMessage"] as string);
            }
            else
            {
                // Update display through a delegate
                if (receivedData.ReadData["ChatMessage"] != null)
                    DisplayMessage(receivedData.ReadData["ChatMessage"] as string);
            }



                // Reset data stream
                dataStream = new byte[1024];


                // Continue listening for broadcasts
                serverSocket.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref epServer, new AsyncCallback(ReceiveData), null);
            });
            t.IsBackground = true;
            t.Start();
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            myId = int.Parse(Input_ProcessId.Text);
            _dataBase = new DataBase(myId);

            try
            {

                // Initialise the socket
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server and listen on port 30000
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 30000+ myId);

                // Associate the socket with this IP address and port
                serverSocket.Bind(server);

                // Initialise the IPEndPoint for the clients
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);

                // Initialise the EndPoint for the clients
                EndPoint epSender = (EndPoint)clients;

                DisplayMessage("Conectado no endereço 127.0.0.1:" + (3000 + myId));

                // Start listening for incoming data
                serverSocket.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);


            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Error: " + ex.Message, "UDP Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void UserRequestInsertData(string s)
        {
            lock (_criticalRegion)
            {
                while(AllowCriticalArea == false)
                {
                    Thread.Sleep(1);
                }
                //my turn has come 
                DisplayMessage("[Site] Recebido mensagem a ser colocada no DB [" + s + "]");
                SiteSendUpdate(s);
                while (AllowCriticalArea == true)
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void UserRequestGetIndex(string name, string ip, int index)
        {
            DisplayMessage(name + " está requisitando a leitura da string no indice " + index);
            SiteSendUserIndex(name, ip, _dataBase.GetLine(index));
        }

        private void SiteSendUserIndex(string name, string ip, string str)
        {
            DisplayMessage("[Site] enviando a " + name + " a string [" + str + "]");
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatDataIdentifier", (str== "" ? DataIdentifier.SiteFail : DataIdentifier.SiteOk));
                sendData.ReadData.Add("ChatMessage", str);
                sendData.ReadData.Add("ChatId", myId);
                IPEndPoint client = IpData.CreateIPEndPoint(ip);
                // Initialise the EndPoint for the client
                epClient = (EndPoint)client;
                byte[] byteData = sendData.GetDataStream();
                serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "SiteSendUserIndex", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SiteGetCoordinator(int id)
        {
            DisplayMessage("[Site] Site " + id + "é o coordenador");
            if (myId != id)
            {
                button_leader.Enabled = false;
                button1.Enabled = false;
            }
            leaderId = id;
        }

        private void SiteSendCoordinator()
        {
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.Coordinator);
                sendData.ReadData.Add("ChatId", myId);
                IPEndPoint client = null;
                for (int i = 1; i <= 3; i++)
                {
                    client = null;
                    client = IpData.CreateIPEndPoint("127.0.0.1:" + (3000 + i).ToString());
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "SendCoordinator", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SiteGetUpdate(string s)
        {
            _dataBase.Insert(s);
            AllowCriticalArea = false;
            DisplayMessage("[Site] Atualizando DB, mensagem [" + s + "]");
            if (leaderId == myId)
            {
                lock (ServerQueue)
                {
                    if(ServerQueue.Count > 0)
                    {
                        ServerQueue.RemoveAt(0);
                        if (ServerQueue.Count > 0)
                        {
                            DisplayMessage("[Leader] Liberando requisição de Site " + ServerQueue[0]);
                            SendReleaseQueue(ServerQueue[0]);
                        }

                    }
                }

            }
        }

        private void SiteSendUpdate(string s)
        {
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.UpdateList);
                sendData.ReadData.Add("ChatMessage", s);
                sendData.ReadData.Add("ChatId", myId);
                for (int i = 1; i <= 3; i++)
                {
                    IPEndPoint client = IpData.CreateIPEndPoint("127.0.0.1:" + (3000 + i).ToString());
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "SendUpdate", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendReleaseQueue(int i)
        {
            try
            {
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.SiteRelease);
                sendData.ReadData.Add("ChatId", myId);
                IPEndPoint client = IpData.CreateIPEndPoint("127.0.0.1:" + (3000 + i).ToString());
                // Initialise the EndPoint for the client
                epClient = (EndPoint)client;
                byte[] byteData = sendData.GetDataStream();
                serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "SendReleaseQueue", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SiteGetRequest(int id)
        {
            lock (ServerQueue)
            {
                ServerQueue.Add(id);
            }
            DisplayMessage("[Leader] Processo " + id + " adicionado a fila");
        }


        private void SiteSendRequest()
        {
            try
            {
                DisplayMessage("[Site] Requisitando ao lider o acesso ao banco de dados");
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.SiteRequest);
                sendData.ReadData.Add("ChatId", myId);
                IPEndPoint client = IpData.CreateIPEndPoint("127.0.0.1:" + (3000 + leaderId).ToString());
                // Initialise the EndPoint for the client
                epClient = (EndPoint)client;
                byte[] byteData = sendData.GetDataStream();
                serverSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.SendData), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Send Error: " + ex.Message, "SiteSendRequest", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Site_Load(object sender, EventArgs e)
        {
        }

        private void button_leader_Click(object sender, EventArgs e)
        {
            SiteSendCoordinator();
        }
    }
}
