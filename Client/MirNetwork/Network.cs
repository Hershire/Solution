using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Library.MirNetwork;

namespace Client.MirNetwork
{
    static class Network
    {
        public static bool Connected;
        public static long TimeConnected, TimeOutTime;

        private static TcpClient Client;
        private static Queue<Packet> ReceiveList = new Queue<Packet>(), SendList = new Queue<Packet>();
        private static byte[] RawData = new byte[0];


        static Network()
        {
            Client = new TcpClient();
            Client.BeginConnect(Settings.IPAddress, Settings.Port, Connection, null);
        }

        private static void Connection(IAsyncResult R)
        {
            try
            {
                Client.EndConnect(R);

                if (!Client.Connected) return;

                Connected = true;
                TimeConnected = Main.Time;
                TimeOutTime = Main.Time + Settings.TimeOut;

                OutBound.ClientVersion();

                BeginReceive();
                
            }
            catch (SocketException)
            {
                ConnectionLost();
                return;
            }
            catch (Exception Ex)
            {
                if (Settings.LogErrors) Main.SaveError(Ex.ToString());
            }
        }

        private static void BeginReceive()
        {
            if (!Connected) return;

            byte[] RawBytes = new byte[8 * 1024];

            try
            {
                Client.Client.BeginReceive(RawBytes, 0, RawBytes.Length, SocketFlags.None, ReceiveData, RawBytes);
            }
            catch
            {
                ConnectionLost();
                return;
            }
        }
        private static void ReceiveData(IAsyncResult Result)
        {
            if (!Connected) return;

            int DataRead;

            try
            {
                DataRead = Client.Client.EndReceive(Result);
            }
            catch
            {
                ConnectionLost();
                return;
            }

            if (DataRead == 0)
            {
                ConnectionLost();
                return;
            }

            byte[] RawBytes = Result.AsyncState as byte[];


            byte[] Temp = RawData;
            RawData = new byte[DataRead + Temp.Length];
            System.Buffer.BlockCopy(Temp, 0, RawData, 0, Temp.Length);
            System.Buffer.BlockCopy(RawBytes, 0, RawData, Temp.Length, DataRead);

            Packet P;
            while ((P = Packet.ReceivePacket(RawData, out RawData)) != null)
                ReceiveList.Enqueue(P);

            BeginReceive();
        }

        private static void ConnectionLost()
        {
            Connected = false;
            SendList.Clear();
            Disconnect();

            MirScenes.SceneFunctions.ConnectionLost();
        }
        private static void BeginSend(Packet P)
        {
            if (!Connected || P == null) return;

            byte[] RawBytes = P.GetPacketBytes();

            try
            {
                Client.Client.BeginSend(RawBytes, 0, RawBytes.Length, SocketFlags.None, SendData, null);
            }
            catch
            {
                ConnectionLost();
                return;
            }

        }
        private static void SendData(IAsyncResult Result)
        {
            try
            {
                Client.Client.EndSend(Result);
            }
            catch { }

        }

        public static void Process()
        {
            if (!Connected)
            {
                if (TimeConnected == 0 && Main.Time >= 10000)
                    MirScenes.SceneFunctions.ConnectionClosed();
                return;
            }

            while (ReceiveList.Count > 0)
                InBound.ProcessPacket(ReceiveList.Dequeue());

            if (Main.Time > TimeOutTime)
                OutBound.KeepAlive();

            while (SendList.Count > 0)
            {
                TimeOutTime = Main.Time + Settings.TimeOut;
                BeginSend(SendList.Dequeue());
            }
        }

        public static void Enqueue(Packet P)
        {
            if (SendList != null && P != null)
                SendList.Enqueue(P);
        }

        public static void Disconnect()
        {
            while (Connected && SendList.Count > 0)
                BeginSend(SendList.Dequeue());

            Connected = false;

            if (Client != null) Client.Client.Dispose();
            Client = null;
            SendList.Clear();
            ReceiveList.Clear();
        }
    }
}
