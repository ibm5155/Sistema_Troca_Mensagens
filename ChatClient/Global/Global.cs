using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    public static class Global
    {



        public static List<ClientData> clientList = new List<ClientData>();

        #region Private Members

        // Request Close Window
        // Client socket
        public static Socket clientSocket;

        // Client name
        public static string name;

        public static bool clientLeader = false;
        public static int myId = -1;
        public static int ElectionOKCount = 0;
        public static bool leaderAlive = false;
        public static int leaderId = -1;

        // Server End Point
        public static EndPoint epServer;
        public static EndPoint epClient;

        // Data stream
        public static byte[] dataStream = new byte[1024];

        // Display message delegate
        public static string GetNewLineLog;

        #endregion

    }
}
