using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Server.MirDatabase;
using Server.MirObjects;

namespace Server.MirNetwork
{
    static class Network
    {
        public static long StartTime;
        private static TcpListener TCPMain;
        public static List<MirConnection> ActiveConnections, ExpiredConnections;
        public static int SessionID;
        public static Thread NetworkThread;
        private static bool StopNetwork;

        static Network()
        {
            ActiveConnections = new List<MirConnection>();
            ExpiredConnections = new List<MirConnection>();
        }

        public static void Start()
        {
            try
            {
                Main.EnqueueMessage("Starting Network.");

                StartTime = Main.Time;

                TCPMain = new TcpListener(IPAddress.Parse(Settings.IPAddress), Settings.Port);
                TCPMain.Start();
                TCPMain.BeginAcceptTcpClient(Connection, null);
                
                NetworkThread = new Thread(Process);
                NetworkThread.IsBackground = true;
                NetworkThread.Start();

                Main.EnqueueMessage("Network Started successfully.");
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
            }

        }
        public static void Stop()
        {
            MirConnection Temp;
            for (int I = 0; I < ActiveConnections.Count; I++)
            {
                Temp = ActiveConnections[I];
                Temp.Disconnect();//Disconnect current connections
            }

            StopNetwork = true;

            if (TCPMain != null) TCPMain.Stop(); //Stop new connections.

            TCPMain = null;
        }

        private static void Process()
        {
            while (!StopNetwork && TCPMain.Server.IsBound)
            {
                Main.UpdateTime();


                for (int I = ActiveConnections.Count - 1; I >= 0; I--)
                {
                    MirConnection C = ActiveConnections[I];
                    if (C.Connected) 
                        C.Process();
                    else
                    {
                        ActiveConnections.RemoveAt(I);
                        ExpiredConnections.Add(C);
                    }
                }


                Thread.Sleep(1);
            }
            NetworkThread = null;
        }

        private static void Connection(IAsyncResult Result)
        {
            if (!StopNetwork && TCPMain.Server.IsBound)
            {
                try
                {
                    TcpClient TempTCPClient = TCPMain.EndAcceptTcpClient(Result);
                    string TempIPAddress = TempTCPClient.Client.RemoteEndPoint.ToString().Split(':')[0];

                    Main.EnqueueMessage(TempIPAddress + ", Connected.");

                    ActiveConnections.Add(new MirConnection(Interlocked.Increment(ref SessionID), TempTCPClient));
                }
                catch (Exception Ex)
                {
                    Main.EnqueueException(Ex);
                }
                finally
                {
                    while (ActiveConnections.Count >= Settings.MaxUser) Thread.Sleep(1);
                    TCPMain.BeginAcceptTcpClient(Connection, null);
                }
            }
        }

        internal static void Disconnect(AccountInfo A, byte Reason = 0)
        {
            MirConnection Temp;
            for (int I = 0; I < ActiveConnections.Count; I++)
            {
                Temp = ActiveConnections[I];
                if (Temp.Account == A)
                {
                    Temp.QueuePacket(new Library.MirNetwork.ServerPackets.Disconnect { Reason = Reason });
                    Temp.Disconnect();
                }
            }
        }

        internal static void UpdateAllPlayers()
        {
            MirConnection Temp;
            for (int I = 0; I < ActiveConnections.Count; I++)
            {
                Temp = ActiveConnections[I];
                if (Temp.Player != null) Temp.Player.SaveData();
            }
        }

        internal static PlayerObject GetPlayer(string CharacterName)
        {
            MirConnection Temp;
            for (int I = 0; I < ActiveConnections.Count; I++)
            {
                Temp = ActiveConnections[I];
                if (Temp.Player != null && string.Compare(CharacterName, Temp.Player.Name, true) == 0)
                    return Temp.Player;
            }
            return null;
        }
    }
}
