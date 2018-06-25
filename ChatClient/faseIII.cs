using ChatApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    public class FaseIII
    {

        public Client Client { get; internal set; }
        string _serverIP;

        public FaseIII(Client client, string ServerIP)
        {
            _serverIP = ServerIP;
            Client = client;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void Insercao()
        {
            Thread t = new Thread(() =>
           {

               while (true)
               {
                   var texto = "Mi//" + RandomString(random.Next(1, 127 - 4));
                   // Initialise a packet object to store the data to be sent
                   Packet sendData = new Packet();
                   sendData.ReadData.Add("ChatName", Global.name);
                   sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.Message);
                   sendData.ReadData.Add("ChatId", Global.myId);

                   // Get packet as byte array
                   IPEndPoint client = IpData.CreateIPEndPoint(_serverIP);
                   // Initialise the EndPoint for the client
                   Global.epClient = (EndPoint)client;
                   byte[] byteData = sendData.GetDataStream();
                   Global.clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, Global.epClient, new AsyncCallback(this.Client.SendData), null);
                    Thread.Sleep(random.Next(100, 400));
               }
           });
            t.IsBackground = true;
            t.Start();
        }

        public void Busca()
        {
            Thread t = new Thread(() =>
            {

                while (true)
                {
                    // Initialise a packet object to store the data to be sent
                    Packet sendData = new Packet();
                    sendData.ReadData.Add("ChatName", Global.name);
                    sendData.ReadData.Add("ChatDataIdentifier", DataIdentifier.SiteIndex);
                    sendData.ReadData.Add("ChatId", Global.myId);
                    sendData.ReadData.Add("Index", random.Next(0, 1000000000));
                    // Get packet as byte array
                    IPEndPoint client = IpData.CreateIPEndPoint(_serverIP);
                    // Initialise the EndPoint for the client
                    Global.epClient = (EndPoint)client;
                    byte[] byteData = sendData.GetDataStream();
                    Global.clientSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, Global.epClient, new AsyncCallback(this.Client.SendData), null);
                    Thread.Sleep(random.Next(200, 800));
                }
            });
            t.IsBackground = true;
            t.Start();
        }

    }
}
