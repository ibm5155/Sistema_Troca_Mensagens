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
        #endregion

        #region Fases
        FaseI faseI;
        FaseII faseII;
        FaseIII faseIII;
        #endregion
        #region Constructor

        public Client()
        {
            InitializeComponent();
            faseI = new FaseI(this);
            faseII = new FaseII(this);
            var all = new ClientData();
            all.name = "All";
            clientList.Add(all);
            userslistbox.Items.Add(all.name);
            faseI.CheckLeaderThread();
        }

        #endregion

        #region UI Events

        private void Client_Load(object sender, EventArgs e)
        {
            // Initialise delegate
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
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

                if(txtServerIP.Text == "")
                {
                    txtServerIP.Text = "127.0.0.1";
                }
                // Initialise a packet object to store the data to be sent
                Packet sendData = new Packet();
                sendData.ReadData.Add("ChatName", this.name);
                sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.LogIn);
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

                dataStream = new byte[1024];
                // Begin listening for broadcasts
                clientSocket.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref Global.epServer, new AsyncCallback(this.ReceiveData), null);
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

        private void IniciarFaseII_Click(object sender, EventArgs e)
        {
            faseII.StartFaseII(true);
        }


        #endregion

        #region Send And Receive

        public void SendData(IAsyncResult ar)
        {
//            try
//            {
                clientSocket.EndSend(ar);
//            }
//            catch (Exception ex)
//            {
      //          MessageBox.Show("Send Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
        }

        private void ReceiveData(IAsyncResult ar)
        {
            var t = new Thread(() =>
            {
//                try
 //               {
                    // Receive all data
                    this.clientSocket.EndReceive(ar);

                    // Initialise a packet object to store the received data

                    Packet receivedData = new Packet(dataStream);

                    DataIdentifier ChatDataIdentifier = receivedData.GetDataIdentifier;
                if (ChatDataIdentifier == DataIdentifier.OK)
                {
                    DisplayMessage("You Logged in");
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
                        faseI.SendOkElection(chatId);
                        faseI.StartElection();
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
                    DisplayMessage(receivedData.ReadData["ChatName"] as string + " tem nivel maior");
                    receivedData.ReadData["ChatName"] = name;
                    clientLeader = false;

                    updatetxtlider("Escravo");
                }
                else if (ChatDataIdentifier == DataIdentifier.Coordinator)
                {
                    int chatId = receivedData.GetInt("ChatId");
                    if (myId > chatId)
                    {
                        DisplayMessage(receivedData.ReadData["ChatName"] as string + " acha que é o coordenador mas eu tenho id maior, resolvendo isso");
                        receivedData.ReadData["ChatName"] = name;
                        faseI.SendOkElection(chatId);
                    }
                    else
                    {
                        leaderId = receivedData.GetInt("ChatId");
                        DisplayMessage(receivedData.ReadData["ChatName"] as string + " é o coordenador");
                        clientLeader = false;
                        updatetxtlider("Escravo");
                        receivedData.ReadData["ChatName"] = name;
                    }
                    receivedData.ReadData["ChatName"] = name;
                }
                else if (ChatDataIdentifier == DataIdentifier.AmAlive)
                {
                    leaderAlive = true;
                    DisplayMessage(receivedData.ReadData["ChatName"] as string + " Lider está vivo ");
                    receivedData.ReadData["ChatName"] = name;
                }
                else if (ChatDataIdentifier == DataIdentifier.IsAlive)
                {
                    leaderAlive = true;
                    DisplayMessage(receivedData.ReadData["ChatName"] as string + " Lider está vivo ");
                    receivedData.ReadData["ChatName"] = name;
                    faseI.SendIsAlive(receivedData.GetInt("ChatId"));
                }

                else if (ChatDataIdentifier == DataIdentifier.UpdateList)
                {
                    var c = new ClientData();
                    c.name = receivedData.ReadData["ChatName"] as string;
                    c.IP = receivedData.ReadData["ChatIp"] as string;
                    c.Id = (receivedData.GetInt("ChatId"));
                    clientList.Add(c);
                    refresh_list();
                }

                else if (ChatDataIdentifier == DataIdentifier.RaStart)
                {
                    DisplayMessage("[system] " + receivedData.ReadData["ChatName"] as string + " Requisitou o inicio da fase 2");
                    faseII.StartFaseII(false);
                }
                else if (ChatDataIdentifier == DataIdentifier.RaOk)
                {
                    DisplayMessage("[system] " + receivedData.ReadData["ChatName"] as string + " Liberou o uso da área critica");
                    faseII.ReleaseCriticalZone();
                }
                else if (ChatDataIdentifier == DataIdentifier.RaRelease)
                {
                    faseII.RemoveQueue(receivedData.GetInt("ChatId"), receivedData.ReadData["ChatName"] as string);
                }
                else if (ChatDataIdentifier == DataIdentifier.RaRequest)
                {
                    faseII.AddQueue(receivedData.GetInt("ChatId"), receivedData.ReadData["ChatName"] as string);
                }

                else if (ChatDataIdentifier == DataIdentifier.SiteOk || ChatDataIdentifier == DataIdentifier.SiteFail) ;

                else if (receivedData.ReadData["ChatName"].Equals(name)) ;//duplicate
                else
                {
                    // Update display through a delegate
                    if (receivedData.ReadData["ChatMessage"] != null)
                        DisplayMessage(receivedData.ReadData["ChatMessage"] as string);
                }



                    // Reset data stream
                    dataStream = new byte[1024];

                    //check if its a new login
                    if (ChatDataIdentifier == DataIdentifier.LogIn)
                    {
                        var c = new ClientData();
                        c.name = receivedData.ReadData["ChatName"] as string;
                        c.IP = receivedData.ReadData["ChatIp"] as string;
                        c.Id = receivedData.GetInt("ChatId");
                        clientList.Add(c);
                        refresh_list();
                        if (clientLeader)
                        {
                            faseI.SendIsCoordinator(c.Id);
                            if (faseII.FaseIIstarted)
                            {
                                faseII.LeaderSendStartFase2();
                            }
                        }
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
                    clientSocket.BeginReceiveFrom(dataStream, 0, dataStream.Length, SocketFlags.None, ref Global.epServer, new AsyncCallback(this.ReceiveData), null);
   //             }
     //           catch (ObjectDisposedException)
       //         { }
         //       catch (Exception ex)
           //     {
             //       MessageBox.Show("Receive Data: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
               // }
            });
            t.IsBackground = true;
            t.Start();
        }

        #endregion

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

        public void userslistbox_Click(object sender, EventArgs e)
        {
            var x = sender as ListBox;
            //ToStr.Text = (string)x.SelectedItem;
        }

        public void refresh_list()
        {
            var lu = new string[clientList.Count];
            for (int i = 0; i < clientList.Count(); i++)
            {
                lu[i] = clientList[i].name;
            }
            this.Invoke(new MethodInvoker(delegate ()
            {
                userslistbox.Items.Clear();
                userslistbox.Items.AddRange(lu);
            }));
        }


        public void updatetxtlider(string nome)
        {

            this.Invoke((MethodInvoker)delegate
            {
                txtLider.Text = nome;
            });
        }

        public string GettxtMessage()
        {
            return txtMessage.Text.Trim();
        }

        public void ElectionRequested(object sender, EventArgs e)
        {
            faseI.StartElection();
        }

        public void AtualizaUiRegiaoCritica(regiaocritica rc)
        {
            this.Invoke((MethodInvoker)delegate
            {
                switch (rc)
                {
                    case regiaocritica.Desativado:
                        fase2pic.Image = Properties.Resources.SII_Idle;
                        break;
                    case regiaocritica.Esperando:
                        fase2pic.Image = Properties.Resources.SII_Wait;
                        break;
                    case regiaocritica.LendoCritico:
                        fase2pic.Image = Properties.Resources.SII_ReadCrit;
                        break;
                    case regiaocritica.LendoNaoCritico:
                        fase2pic.Image = Properties.Resources.SII_ReadNorm;
                        break;
                }
            });
        }

        public ClientData GetClientById(int id)
        {
            ClientData c = null;
            for (int i =0; i < clientList.Count(); i++)
            {
                if (clientList[i].Id == id)
                {
                    c = clientList[i];
                    i = clientList.Count();
                }
            }
            return c;
        }

        public void UpdateCriticalRegionQueueSize(int size, int max)
        {
            this.Invoke((MethodInvoker)delegate
            {
                RcQueueSize.Maximum = max;
                RcQueueSize.Value = (size > max? max : size);
            });
        }

        private void button_faseIII_Click(object sender, EventArgs e)
        {

            int id = int.Parse(siteID.Text) + 3000;
            faseIII = new FaseIII(this, "127.0.0.1:" +  id.ToString());
            faseIII.Insercao();
            faseIII.Insercao();
            faseIII.Busca();
            faseIII.Busca();
        }

        private void button_index_Click(object sender, EventArgs e)
        {

        }
    }
}
