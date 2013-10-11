using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Library.MirNetwork.ServerPackets
{
    public sealed class ClientVersion : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class Disconnect : Packet
    {
        public byte Reason;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Reason = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Reason);
        }
    }

    public sealed class NewAccount : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class ChangePassword : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class ChangePasswordBanned : Packet
    {
        public string Reason;
        public DateTime ExpiryDate;

        protected override void ReadPacket(BinaryReader BReader)
        {
            Reason = BReader.ReadString();
            ExpiryDate = DateTime.FromBinary(BReader.ReadInt64());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Reason ?? string.Empty);
            BWriter.Write(ExpiryDate.ToBinary());
        }
    }
    public sealed class Login : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class LoginBanned : Packet
    {
        public string Reason;
        public DateTime ExpiryDate;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Reason = BReader.ReadString();
            ExpiryDate = DateTime.FromBinary(BReader.ReadInt64());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Reason ?? string.Empty);
            BWriter.Write(ExpiryDate.ToBinary());
        }
    }
    public sealed class LoginSuccess : Packet
    {
        public List<SelectCharacterInfo> CharacterList = new List<SelectCharacterInfo>();

        protected override void ReadPacket(BinaryReader BReader)
        {
            int Count = BReader.ReadInt32();
            for (int I = 0; I < Count; I++)
                CharacterList.Add(new SelectCharacterInfo(BReader));
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(CharacterList.Count);

            for (int I = 0; I < CharacterList.Count; I++)
                CharacterList[I].Save(BWriter);
        }
    }

    public sealed class NewCharacter : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class NewCharacterSuccess : Packet
    {
        public SelectCharacterInfo CharInfo;
        protected override void ReadPacket(BinaryReader BReader)
        {
            CharInfo = new SelectCharacterInfo(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            CharInfo.Save(BWriter);
        }
    }
    public sealed class DeleteCharacter : Packet
    {    
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class StartGame : Packet
    {
        public byte Result;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Result = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Result);
        }
    }
    public sealed class StartGameDelay : Packet
    {
        public byte Seconds;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Seconds = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Seconds);
        }

    }
    public sealed class StartGameBanned : Packet
    {
        public string Reason;
        public DateTime ExpiryDate;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Reason = BReader.ReadString();
            ExpiryDate = DateTime.FromBinary(BReader.ReadInt64());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Reason ?? string.Empty);
            BWriter.Write(ExpiryDate.ToBinary());
        }
    }

    public sealed class LogOutSuccess : Packet
    {
        public List<SelectCharacterInfo> CharacterList = new List<SelectCharacterInfo>();

        protected override void ReadPacket(BinaryReader BReader)
        {
            int Count = BReader.ReadInt32();
            for (int I = 0; I < Count; I++)
                CharacterList.Add(new SelectCharacterInfo(BReader));
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(CharacterList.Count);

            for (int I = 0; I < CharacterList.Count; I++)
                CharacterList[I].Save(BWriter);
        }
    }

    public sealed class ObjectChat : Packet
    {
        public string Name;
        public string Message;
        public MirChatType Type;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Name = BReader.ReadString();
            Message = BReader.ReadString();
            Type = (MirChatType)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Name ?? string.Empty);
            BWriter.Write(Message ?? string.Empty);
            BWriter.Write((byte)Type);
        }
    }
    public sealed class WinExperience : Packet
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

    public sealed class MapInfomation : Packet
    {
        public ClientMapInfo Details;

        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new ClientMapInfo(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);

            if (Details != null) Details.Save(BWriter);
        }
    }
    public sealed class UserInformation : Packet
    {
        public UserDetails Details;
        public int Gold;

        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new UserDetails(BReader);
            
            Gold = BReader.ReadInt32();
            
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);
            if (Details != null)
                Details.Save(BWriter);

            BWriter.Write(Gold);            
        }
    }
    public sealed class UpdateUserStats : Packet
    {
        public UserStats Details;

        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new UserStats(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);
            if (Details != null)
                Details.Save(BWriter);
        }
    }
    public sealed class UserInventory : Packet
    {
        public int Gold;
        public UserItem[] Inventory, Equipment, Stroage;

        protected override void ReadPacket(BinaryReader BReader)
        {
            Gold = BReader.ReadInt32();

            if (BReader.ReadBoolean())
            {
                Inventory = new UserItem[BReader.ReadInt32()];
                for (int I = 0; I < Inventory.Length; I++)
                    if (BReader.ReadBoolean()) Inventory[I] = new UserItem(BReader);

            }

            if (BReader.ReadBoolean())
            {
                Equipment = new UserItem[BReader.ReadInt32()];
                for (int I = 0; I < Equipment.Length; I++)
                    if (BReader.ReadBoolean()) Equipment[I] = new UserItem(BReader);

            }

            if (BReader.ReadBoolean())
            {
                Stroage = new UserItem[BReader.ReadInt32()];
                for (int I = 0; I < Stroage.Length; I++)
                    if (BReader.ReadBoolean()) Stroage[I] = new UserItem(BReader);
            }

        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            UserItem U;

            BWriter.Write(Gold);

            BWriter.Write(Inventory != null);
            if (Inventory != null)
            {
                BWriter.Write(Inventory.Length);

                for (int I = 0; I < Inventory.Length; I++)
                {
                    U = Inventory[I];
                    BWriter.Write(U != null);
                    if (U == null) continue;
                    U.Save(BWriter);
                }
            }

            BWriter.Write(Equipment != null);
            if (Equipment != null)
            {
                BWriter.Write(Equipment.Length);

                for (int I = 0; I < Equipment.Length; I++)
                {
                    U = Equipment[I];
                    BWriter.Write(U != null);
                    if (U == null) continue;
                    U.Save(BWriter);
                }
            }

            BWriter.Write(Stroage != null);
            if (Stroage != null)
            {
                BWriter.Write(Stroage.Length);

                for (int I = 0; I < Stroage.Length; I++)
                {
                    U = Stroage[I];
                    BWriter.Write(U != null);
                    if (U == null) continue;
                    U.Save(BWriter);
                }
            }

        }
    }
    public sealed class WeightChanged : Packet
    {
        protected override void ReadPacket(BinaryReader BReader)
        {
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {         
        }
    }

    public sealed class PlayerLocation : Packet
    {
        public Point Location;
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Location.X);
            BWriter.Write(Location.Y);
            BWriter.Write((byte)Direction);
        }
    }

    public sealed class NewPlayerObject : Packet
    {
        public UserDetails Details;
            
        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new UserDetails(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);
            if (Details != null)
                Details.Save(BWriter);  
        }
    }
    public sealed class NewMonsterObject : Packet
    {
        public MonsterDetails Details;

        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new MonsterDetails(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);
            if (Details != null)
                Details.Save(BWriter);
        }
    }
    public sealed class NewItemObject : Packet
    {
        public ItemDetails Details;

        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Details = new ItemDetails(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Details != null);
            if (Details != null)
                Details.Save(BWriter);
        }
    }

    public sealed class RemoveMapObject : Packet
    {
        public long ObjectID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
        }
    }
    public sealed class ObjectTurn : Packet
    {
        public long ObjectID;
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write((byte)Direction);
        }
    }
    public sealed class ObjectWalk : Packet
    {
        public long ObjectID;
        public Point Location;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(Location.X);
            BWriter.Write(Location.Y);
        }
    }
    public sealed class ObjectRun : Packet
    {
        public long ObjectID;
        public Point Location;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(Location.X);
            BWriter.Write(Location.Y);
        }
    }
    public sealed class ObjectAttack : Packet
    {
        public long ObjectID;
        public MirDirection Direction;
        public byte Type;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Direction = (MirDirection)BReader.ReadByte();
            Type = BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write((byte)Direction);
            BWriter.Write(Type);
        }
    }
    public sealed class ObjectHarvest : Packet
    {
        public long ObjectID;
        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Direction = (MirDirection)BReader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write((byte)Direction);
        }
    }
    public sealed class ObjectStruck : Packet
    {
        public long ObjectID, AttackerID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            AttackerID = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(AttackerID);
        }
    }
    public sealed class ObjectDied : Packet
    {
        public long ObjectID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
        }
    }
    public sealed class ObjectLevelUp : Packet
    {
        public long ObjectID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
        }
    }
    public sealed class ObjectSkeleton : Packet
    {
        public long ObjectID;
        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
        }
    }
    public sealed class ObjectHealthChanged : Packet
    {
        public long ObjectID;
        public int HP, MaxHP;
        public int MP, MaxMP;

        protected override void ReadPacket(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            HP = BReader.ReadInt32();
            MaxHP = BReader.ReadInt32();
            MP = BReader.ReadInt32();
            MaxMP = BReader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(HP);
            BWriter.Write(MaxHP);
            BWriter.Write(MP);
            BWriter.Write(MaxMP);
        }
    }

    public sealed class GainedGold : Packet
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
    public sealed class GainedItem : Packet
    {
        public UserItem Item;
        protected override void ReadPacket(BinaryReader BReader)
        {
            if (BReader.ReadBoolean())
                Item = new UserItem(BReader);
        }
        protected override void WritePacket(BinaryWriter BWriter)
        {
            BWriter.Write(Item != null);
            if (Item != null)
                Item.Save(BWriter);
        }
    }
}
