using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Library.MirNetwork;
using S = Library.MirNetwork.ServerPackets;
using C = Library.MirNetwork.ClientPackets;
using Server.MirDatabase;
using Server.MirObjects;
using Library;


namespace Server.MirNetwork
{
    public enum GameStage { None, Login, Select, Game, Disconnected }

    class MirConnection
    {
        public readonly int SessionID;
        public readonly string IPAddress;
        public AccountInfo Account;
        public PlayerObject Player;

        public bool Connected = true;
        public long TimeConnected, TimeDisconnected, TimeOutTime;

        private TcpClient TClient;
        private Queue<Packet> ReceiveList = new Queue<Packet>(), SendList = new Queue<Packet>();
        byte[] RawData = new byte[0];
        
        public GameStage Stage;
                
        public MirConnection(int _SessionID, TcpClient _TClient)
        {
            SessionID = _SessionID;
            IPAddress = _TClient.Client.RemoteEndPoint.ToString().Split(':')[0];

            TClient = _TClient;

            TimeConnected = Main.Time;
            TimeOutTime = TimeConnected + Settings.TimeOut;

            BeginReceive();
        }

        private void BeginReceive()
        {
            if (!Connected) return;

            byte[] RawBytes = new byte[8 * 1024];

            try
            {
                TClient.Client.BeginReceive(RawBytes, 0, RawBytes.Length, SocketFlags.None, ReceiveData, RawBytes);
            }
            catch
            {
                Connected = false;
                SendList.Clear();
                Disconnect();
            }
        }
        private void ReceiveData(IAsyncResult Result)
        {
            if (!Connected) return;

            int DataRead;

            try
            {
                DataRead = TClient.Client.EndReceive(Result);
            }
            catch
            {
                Connected = false;
                SendList.Clear();
                Disconnect();
                return;
            }

            if (DataRead == 0) { Disconnect(); return; }
            
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

        private void BeginSend(Packet P)
        {
            if (!Connected || P == null) return;

            byte[] RawBytes = P.GetPacketBytes();

            try
            {
                TClient.Client.BeginSend(RawBytes, 0, RawBytes.Length, SocketFlags.None, SendData, null);
            }
            catch
            {
                Connected = false;
                SendList.Clear();
                Disconnect();
            }

        }
        private void SendData(IAsyncResult Result)
        {
            try
            {
                TClient.Client.EndSend(Result);
            }
            catch { }

        }

        public void QueuePacket(Packet P)
        {
            if (P == null || SendList == null) return;
            SendList.Enqueue(P);
        }

        public void Process()
        {
            while (ReceiveList.Count > 0)
            {
                ProcessPacket(ReceiveList.Dequeue());
                TimeOutTime = Main.Time + Settings.TimeOut;
            }

            if (Main.Time > TimeOutTime)
            {
                Connected = false;
                SendList.Clear();
                Disconnect();
                return;
            }

            while (SendList.Count > 0)
                BeginSend(SendList.Dequeue());

        }
        private void ProcessPacket(Packet P)
        {
            if (P == null || P is C.KeepAlive) return;
            try
            {
                GetType().GetMethod(P.GetType().Name).Invoke(this, new object[] { P });
            }
            catch (IOException IEx)
            {
                Main.EnqueueException(IEx);
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
            }
        }
        public void Disconnect()
        {
            while (Connected && SendList.Count > 0)
                BeginSend(SendList.Dequeue());

            Connected = false;

            if (Stage == GameStage.Disconnected) return;

            if (Stage == GameStage.Game)
                Player.DeSpawn();

            Stage = GameStage.Disconnected;

            TimeDisconnected = Main.Time;

            if (TClient != null) TClient.Client.Dispose();
            TClient = null;

            Account = null;
            SendList.Clear();
            ReceiveList.Clear();

            Network.ActiveConnections.Remove(this);
            if (!Network.ExpiredConnections.Contains(this)) Network.ExpiredConnections.Add(this);
        }

        public void ClientVersion(C.ClientVersion P)
        {
            if (Stage != GameStage.None) return;

           /* if (Settings.CheckVersion)
                if (!Functions.CompareBytes(Config.VersionHash, P.VersionHash))
                {
                    QueuePacket(new S.ClientVersion { Result = 0 });
                    Connected = false;
                    Main.EnqueueMessage(SessionID + ", Disconnnected - Wrong Client Version.");
                    return;
                }*/
            QueuePacket(new S.ClientVersion { Result = 1 });
            Stage = GameStage.Login;
        }
        public void Disconnect(C.Disconnect P)
        {
            Disconnect();
        }

        public void NewAccount(C.NewAccount P)
        {
            if (Stage != GameStage.Login) return;

            MirDB.CreateAccount(P, this);
        }
        public void ChangePassword(C.ChangePassword P)
        {
            if (Stage != GameStage.Login) return;

            MirDB.ChangePassword(P, this);
        }
        public void Login(C.Login P)
        {
            if (Stage != GameStage.Login) return;

            MirDB.Login(P, this);
        }

        public void NewCharacter(C.NewCharacter P)
        {
            if (Stage != GameStage.Select) return;

            MirDB.CreateCharacter(P, this);
        }
        public void DeleteCharacter(C.DeleteCharacter P)
        {
            if (Stage != GameStage.Select) return;

            if (!Settings.AllowDeleteCharacter)
            {
                QueuePacket(new S.DeleteCharacter { Result = 1 });
                return;
            }

            CharacterInfo Temp = null;

            for (int I = 0; I < Account.Characters.Count; I++)
                if (Account.Characters[I].CharacterIndex == P.CharacterIndex)
                {
                    Temp = Account.Characters[I];
                    break;
                }

            if (Temp == null)
            {
                QueuePacket(new S.DeleteCharacter { Result = 2 });
                return;
            }

            Temp.Deleted = true;
            Temp.DeleteCount++;
            Temp.DeleteDate = Main.Now;


            QueuePacket(new S.DeleteCharacter { Result = 3 });
        }
        public void StartGame(C.StartGame P)
        {
            if (Stage != GameStage.Select) return;

            if (Account == null)
            {
                QueuePacket(new S.StartGame { Result = 0 });
                return;
            }

            if (!Settings.AllowStartGame)
            {
                QueuePacket(new S.StartGame { Result = 1 });
                return;
            }


            CharacterInfo TempData = Account.Characters.FirstOrDefault(C => C.CharacterIndex == P.CharacterIndex && !C.Deleted);

            if (TempData == null)
            {
                QueuePacket(new S.StartGame { Result = 2 });
                return;
            }

            if (TempData.Lock)
            {
                QueuePacket(new S.StartGame { Result = 0 });
                return;
            }


            if (TempData.Banned)
            {
                if (TempData.ExpiryDate > DateTime.Now)
                {
                     QueuePacket(new S.StartGameBanned { Reason = TempData.BanReason, ExpiryDate = TempData.ExpiryDate });
                    return;
                }
                else
                {
                    TempData.Banned = false;
                    TempData.BanReason = string.Empty;
                    TempData.ExpiryDate = DateTime.MinValue;
                }
            }

            double Seconds = (Main.Now - TempData.LastAccess).TotalSeconds;

            if (Seconds < Settings.ReLogTime)
            {
                QueuePacket(new S.StartGameDelay { Seconds = (byte)(Settings.ReLogTime - Seconds + 1) });
                return;
            }

            Player = new PlayerObject(TempData, this);

            if (!Player.Spawn())
            {
                QueuePacket(new S.StartGame { Result = 3 });
                Player = null;
                return;
            }
            
            QueuePacket(new S.StartGame { Result = 4 });
            Stage = GameStage.Game;
        }

        public void Chat(C.Chat P)
        {
            if (P.Message.Length > Globals.MaxChatLength)
            {
                Disconnect();
                return;
            }

            if (Stage != GameStage.Game) return;
            Player.Chat(P.Message);
        }

        public void Turn(C.Turn P)
        {
            if (Stage != GameStage.Game) return;
            Player.Turn(P.Direction);
        }
        public void Walk(C.Walk P)
        {
            if (Stage != GameStage.Game) return;
            Player.Walk(P.Direction);
        }
        public void Run(C.Run P)
        {
            if (Stage != GameStage.Game) return;
            Player.Run(P.Direction);
        }
        public void Attack(C.Attack P)
        {
            if (Stage != GameStage.Game) return;
            Player.Attack(P.Direction);
        }
        public void Harvest(C.Harvest P)
        {
            if (Stage != GameStage.Game) return;
            Player.Harvest(P.Direction);
        }

        public void MoveItem(C.MoveItem P)
        {
            if (Stage != GameStage.Game) return;

            Player.MoveItem(P.Grid, P.From, P.To);
        }
        public void EquipItem(C.EquipItem P)
        {
            if (Stage != GameStage.Game) return;

            Player.EquipItem(P.Grid, P.UniqueID, P.To);
        }
        public void RemoveItem(C.RemoveItem P)
        {
            if (Stage != GameStage.Game) return;

            Player.RemoveItem(P.Grid, P.UniqueID, P.To);
        }
        public void UseItem(C.UseItem P)
        {
            if (Stage != GameStage.Game) return;

            Player.UseItem(P.Grid, P.UniqueID);
        }
        public void PickUp(C.PickUp P)
        {
            if (Stage != GameStage.Game) return;

            Player.PickUp();
        }

        public void LogOut(C.LogOut P)
        {
            if (Stage != GameStage.Game) return;

            Player.DeSpawn();

            Stage = GameStage.Select;

            QueuePacket(new S.LogOutSuccess { CharacterList = Account.GetSelectInfo() });
        }

        public void DropItem(C.DropItem P)
        {
            if (Stage != GameStage.Game) return;

            Player.DropItem(P.Grid, P.UniqueID);
        }
        public void DropGold(C.DropGold P)
        {
            if (Stage != GameStage.Game) return;

            Player.DropGold(P.Amount);
        }
    }
}
