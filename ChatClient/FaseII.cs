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
        public class FaseII
        {
            public bool FaseIIstarted = false;

            List<int> QueueWait = new List<int>();//used only for the leader for managing the requests

            public Client Client { get; internal set; }

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

            public FaseII(Client client)
            {
                this.Client = client;
            }

            public void StartFaseII(bool requestedbyme)
            {
                //loop to simulate some work
                if (FaseIIstarted) return;
                if (requestedbyme == false) ;
                else if (leaderId == -1)
                {
                    MessageBox.Show("Função não disponivel enquanto um lider não for escolhido", "FaseII Start", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else if (leaderId != myId)
                {
                    MessageBox.Show("Função somente disponível para o lider.", "FaseII Start", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (requestedbyme)
                {
                    LeaderSendStartFase2();
                }
                FaseIIstarted = true;
                Thread t = new Thread(() =>
                {
                    var rand = new Random();
                    int oldleader = leaderId;
                    while (true)
                    {
                        if (leaderAlive == true)
                        {
                            //Step 1 outside critical region
                            this.Client.AtualizaUiRegiaoCritica(regiaocritica.LendoNaoCritico);
                            Thread.Sleep(rand.Next(2000, 16000));//sleep btw 2s up to 4
                                                                 //Step 2
                            Thread.Sleep(rand.Next(200, 400));//Delay to send packet
                            UsrSendRequestCriticalArea(this.Client.GetClientById(leaderId));
                            this.Client.AtualizaUiRegiaoCritica(regiaocritica.Esperando);
                            while (AllowCriticalArea == false && oldleader == leaderId)
                            {
                                Thread.Sleep(10);
                            }
                        }
                        if (leaderId != oldleader)
                        {
                            AllowCriticalArea = false;
                            this.Client.AtualizaUiRegiaoCritica(regiaocritica.Desativado);
                            this.Client.DisplayMessage("[system] o lider alterou, resetando sistema");
                            oldleader = leaderId;
                            this.Client.UpdateCriticalRegionQueueSize(0, 1);
                            QueueWait.Clear();
                            //reset
                        }
                        else if (leaderAlive == false) ;
                        else
                        {
                            this.Client.AtualizaUiRegiaoCritica(regiaocritica.LendoCritico);
                            AllowCriticalArea = false;
                            Thread.Sleep(rand.Next(2000, 4000));//sleep btw 2s up to 4
                            this.Client.AtualizaUiRegiaoCritica(regiaocritica.LendoNaoCritico);
                            Thread.Sleep(rand.Next(200, 400));//Delay to send packet
                            UsrSendReleaseCriticalArea(this.Client.GetClientById(oldleader));
                        }
                    }

                });
                t.IsBackground = true;
                t.Start();
            }

            #region Leader Requests

            public void LeaderSendStartFase2()
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        // Initialise a packet object to store the data to be sent
                        Packet sendData = new Packet();
                        sendData.ReadData.Add("ChatName", this.name);
                        sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.RaStart);
                        sendData.ReadData.Add("ChatId", myId);

                        // Get packet as byte array
                        byte[] byteData = sendData.GetDataStream();
                        // Send packet to the server
                        IPEndPoint client = null;
                        for (int id = 1; id < clientList.Count(); id++)
                        {
                            client = IpData.CreateIPEndPoint(clientList[id].IP);
                            // Initialise the EndPoint for the client
                            epClient = (EndPoint)client;
                            byteData = sendData.GetDataStream();
                            clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient,
                                new AsyncCallback(this.Client.SendData), null);
                        }
                    }
                    catch (System.Net.Sockets.SocketException ex)
                    {
                        this.Client.faseI.RequireElectionByCode();
                        AllowCriticalArea = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Send Error: " + ex.Message, "UDP Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                t.IsBackground = true;
                t.Start();
            }

            public void LeaderSendRequestCriticalArea(ClientData c)
            {
                try
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", this.name);
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.RaOk);
                    sendData.ReadData.Add("ChatId", myId);
                    string usrname = c.name;

                    // Get packet as byte array
                    int id = c.Id;
                    IPEndPoint client = IpData.CreateIPEndPoint(c.IP);
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.Client.SendData), null);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    this.Client.faseI.RequireElectionByCode();
                    AllowCriticalArea = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Send Error: " + ex.Message, "Fase II LeaderSendRequestCriticalArea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            #endregion

            #region User Requests

            public void UsrSendRequestCriticalArea(ClientData c)
            {
                try
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", this.name);
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.RaRequest);
                    sendData.ReadData.Add("ChatId", myId);
                    string usrname = c.name;

                    // Get packet as byte array
                    int id = c.Id;
                    IPEndPoint client = IpData.CreateIPEndPoint(c.IP);
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.Client.SendData), null);
                }
            catch (System.Net.Sockets.SocketException ex)
            {
                this.Client.faseI.RequireElectionByCode();
                AllowCriticalArea = false;
            }
            catch (Exception ex)
                {
                    MessageBox.Show("Send Error: " + ex.Message, "Fase II UserSendRequestCriticalArea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            public void UsrSendReleaseCriticalArea(ClientData c)
            {
                try
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", this.name);
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.RaRelease);
                    sendData.ReadData.Add("ChatId", myId);
                    string usrname = c.name;

                    // Get packet as byte array
                    int id = c.Id;
                    IPEndPoint client = IpData.CreateIPEndPoint(c.IP);
                    // Initialise the EndPoint for the client
                    epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, epClient, new AsyncCallback(this.Client.SendData), null);
                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    this.Client.faseI.RequireElectionByCode();
                    AllowCriticalArea = false;
                }
                catch (Exception ex)
                    {
                        MessageBox.Show("Send Error: " + ex.Message, "Fase II UsrSendReleaseCriticalArea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


            #endregion

            #region Leader Functions
            public void AddQueue(int id, string name)
            {
                lock (QueueWait)
                {
                    if (QueueWait.Count == 0)
                    {
                        //first to join critical region is allowed to get into
                        LeaderSendRequestCriticalArea(this.Client.GetClientById(id));
                    }
                    QueueWait.Add(id);
                    this.Client.DisplayMessage("[system] Adicionado " + name + " a lista de espera da região critica");
                    this.Client.UpdateCriticalRegionQueueSize(QueueWait.Count, clientList.Count);
                }
            }

            public void RemoveQueue(int id, string name)
            {
                lock (QueueWait)
                {
                    if (QueueWait.Count > 0)
                    {
                        QueueWait.RemoveAt(0);
                        ClientData c;
                        if (QueueWait.Count() > 0)
                        {
                            c = this.Client.GetClientById(QueueWait[0]);
                            this.Client.DisplayMessage("[system] removendo " + name + " da lista de espera da região critica e informando ao " + c.name + " que ele pode entrar na região critica");
                            Thread.Sleep(new Random().Next(200, 400));//Delay to send packet
                            LeaderSendRequestCriticalArea(c);
                        }
                        else
                        {
                            this.Client.DisplayMessage("[system] removendo " + name + " da lista de espera da região critica");
                        }
                        this.Client.UpdateCriticalRegionQueueSize(QueueWait.Count, clientList.Count);

                    }
                }
            }

            #endregion

            #region User Functions
            public void ReleaseCriticalZone()
            {
                AllowCriticalArea = true;
            }

            #endregion

        }
    }