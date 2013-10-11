using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Library.MirNetwork.ClientPackets
{
    public sealed class ClientVersion : Packet
    {
        public byte[] VersionHash;

        protected override void ReadPacket(BinaryReader BReader)
        {
            VersionHash = BReader.ReadBytes(BReader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(VersionHash.Length);
            BWriter.Write(VersionHash);
        }

    }
    public sealed class KeepAlive : Packet
    {
        protected override void ReadPacket(BinaryReader BReader)
        {
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
        }
    }
    public sealed class Disconnect : Packet
    {
        protected override void ReadPacket(BinaryReader BReader)
        {
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
        }
    }

    public sealed class NewAccount : Packet
    {
        public string AccountID;
        public string Password;
        public DateTime BirthDate;
        public string UserName;
        public string SecretQuestion;
        public string SecretAnswer;
        public string EMailAddress;

        protected override void ReadPacket(BinaryReader BReader)
        {
            AccountID = BReader.ReadString();
            Password = BReader.ReadString();
            BirthDate = DateTime.FromBinary(BReader.ReadInt64());
            UserName = BReader.ReadString();
            SecretQuestion = BReader.ReadString();
            SecretAnswer = BReader.ReadString();
            EMailAddress = BReader.ReadString();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(AccountID ?? string.Empty);
            BWriter.Write(Password ?? string.Empty);
            BWriter.Write(BirthDate.ToBinary());
            BWriter.Write(UserName ?? string.Empty);
            BWriter.Write(SecretQuestion ?? string.Empty);
            BWriter.Write(SecretAnswer ?? string.Empty);
            BWriter.Write(EMailAddress ?? string.Empty);
        }
    }
    public sealed class ChangePassword : Packet
    {
        public string AccountID;
        public string CurrentPassword;
        public string NewPassword;

        protected override void ReadPacket(BinaryReader BReader)
        {
            AccountID = BReader.ReadString();
            CurrentPassword = BReader.ReadString();
            NewPassword = BReader.ReadString();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(AccountID ?? string.Empty);
            BWriter.Write(CurrentPassword ?? string.Empty);
            BWriter.Write(NewPassword ?? string.Empty);
        }
    }
    public sealed class Login : Packet
    {
        public string AccountID;
        public string Password;
        protected override void ReadPacket(BinaryReader BReader)
        {
            AccountID = BReader.ReadString();
            Password = BReader.ReadString();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(AccountID ?? string.Empty);
            BWriter.Write(Password ?? string.Empty);
        }
    }
    public sealed class NewCharacter : Packet
    {
        public string CharacterName;
        public MirGender Gender;
        public MirClass Class;
        protected override void ReadPacket(BinaryReader BReader)
        {
            CharacterName = BReader.ReadString();
            Gender = (MirGender)BReader.ReadByte();
            Class = (MirClass)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(CharacterName ?? string.Empty);
            BWriter.Write((byte)Gender);
            BWriter.Write((byte)Class);
        }
    }
    public sealed class DeleteCharacter : Packet
    {
        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader BReader)
        {
            CharacterIndex = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(CharacterIndex);
        }
    }
    public sealed class StartGame : Packet
    {
        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader BReader)
        {
            CharacterIndex = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(CharacterIndex);
        }
    }

    public sealed class LogOut : Packet
    {
        protected override void ReadPacket(BinaryReader BReader)
        {
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
        }
    }


    public sealed class Chat : Packet
    {
        public string Message;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Message = BReader.ReadString();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Message ?? string.Empty);
        }
    }
    public sealed class Turn : Packet
    {
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader BReader)
        {
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Direction);
        }
    }
    public sealed class Walk : Packet
    {
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Direction);
        }
    }
    public sealed class Run : Packet
    {
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Direction);
        }
    }
    public sealed class Attack : Packet
    {
        public MirDirection Direction;
        public byte Type;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Direction = (MirDirection)BReader.ReadByte();
            Type = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Direction);
            BWriter.Write(Type);
        }
    }
    public sealed class Harvest : Packet
    {
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Direction);
        }
    }


    public sealed class MoveItem : Packet
    {
        public MirGridType Grid;
        public int From, To;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Grid = (MirGridType)BReader.ReadByte();
            From = BReader.ReadInt32();
            To = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Grid);
            BWriter.Write(From);
            BWriter.Write(To);
        }
    }
    public sealed class EquipItem : Packet
    {
        public MirGridType Grid;
        public int UniqueID, To;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Grid = (MirGridType)BReader.ReadByte();
            UniqueID = BReader.ReadInt32();
            To = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Grid);
            BWriter.Write(UniqueID);
            BWriter.Write(To);
        }
    }
    public sealed class RemoveItem : Packet
    {
        public MirGridType Grid;
        public int UniqueID, To;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Grid = (MirGridType)BReader.ReadByte();
            UniqueID = BReader.ReadInt32();
            To = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Grid);
            BWriter.Write(UniqueID);
            BWriter.Write(To);
        }
    }
    public sealed class UseItem : Packet
    {
        public MirGridType Grid;
        public int UniqueID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Grid = (MirGridType)BReader.ReadByte();
            UniqueID = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Grid);
            BWriter.Write(UniqueID);
        }
    }
    public sealed class DropItem : Packet
    {
        public MirGridType Grid;
        public int UniqueID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Grid = (MirGridType)BReader.ReadByte();
            UniqueID = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write((byte)Grid);
            BWriter.Write(UniqueID);
        }
    }

    public sealed class PickUp : Packet
    {
        protected override void ReadPacket(BinaryReader BReader)
        {
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
        }
    }
    public sealed class DropGold : Packet
    {
        public int Amount;

        protected override void ReadPacket(BinaryReader BReader)
        {
            Amount = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Amount);
        }

    }
}
