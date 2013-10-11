using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Library;
using System.IO;
using System.Data;
using C = Library.MirNetwork.ClientPackets;
using Server.MirNetwork;

namespace Server.MirDatabase
{
    //Server
    public class MapInfo
    {
        public List<RespawnInfo> RespawnList = new List<RespawnInfo>();
        public List<MovementInfo> MovementList = new List<MovementInfo>();
        public List<SafeZoneInfo> SafeZoneList = new List<SafeZoneInfo>();

        public int MapIndex,
                   MiniMap,
                   BigMap;

        public string FileName,
                      MapName;

        public bool CanMine;

        public LightSetting LightMode;
        public FightSetting FightMode;

        public MapInfo(DataRow Row)
        {
            MapIndex = Row["Map Index"] is DBNull ? -1 : (int)Row["Map Index"];
            FileName = Row["File Name"] is DBNull ? string.Empty : Row["File Name"].ToString();
            MapName = Row["Map Name"] is DBNull ? string.Empty : Row["Map Name"].ToString();
            MiniMap = Row["Mini Map"] is DBNull ? -1 : (int)Row["Mini Map"];
            BigMap = Row["Big Map"] is DBNull ? -1 : (int)Row["Big Map"];

            string Flags = Row["Flags"] is DBNull ? string.Empty : Row["Flags"].ToString();

            if (Flags.IndexOf("SAFE", StringComparison.OrdinalIgnoreCase) >= 0)
                FightMode = FightSetting.Safe;
            else if (Flags.IndexOf("FIGHT", StringComparison.OrdinalIgnoreCase) >= 0)
                FightMode = FightSetting.Fight;

            if (Flags.IndexOf("DAY", StringComparison.OrdinalIgnoreCase) >= 0)
                LightMode = LightSetting.Day;
            else if (Flags.IndexOf("NIGHT", StringComparison.OrdinalIgnoreCase) >= 0)
                LightMode = LightSetting.Night;

            if (Flags.IndexOf("MINE", StringComparison.OrdinalIgnoreCase) >= 0)
                CanMine = true;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", MapIndex, MapName); 
        }
    }

    public class SafeZoneInfo
    {
        public int SafeZoneIndex,
                   MapIndex,
                   Size;
        
        public Point Location;

        public bool StartPoint;

        public SafeZoneInfo(DataRow Row)
        {
            SafeZoneIndex = Row["SafeZone Index"] is DBNull ? -1 : (int)Row["SafeZone Index"];
            MapIndex = Row["Map Index"] is DBNull ? -1 : (int)Row["Map Index"];
            Size = Row["Size"] is DBNull ? 0 : (int)Row["Size"];

            Location.X = Row["Location X"] is DBNull ? 0 : (int)Row["Location X"];
            Location.Y = Row["Location Y"] is DBNull ? 0 : (int)Row["Location Y"];

            StartPoint = Row["Start Point"] is DBNull ? false : (bool)Row["Start Point"];
        }
    }
    public class MonsterInfo
    {
        public List<DropInfo> DropList = new List<DropInfo>();

        public int AI;

        public int MonsterIndex,
                   Image,
                   Effect,
                   Level,
                   Health,
                   MinAC,
                   MaxAC,
                   MinMAC,
                   MaxMAC,
                   MinDC,
                   MaxDC,
                   MinMC,
                   MaxMC,
                   MinSC,
                   MaxSC,
                   Experience,
                   Accuracy,
                   Agility,
                   Light,
                   AttackSpeed,
                   MoveSpeed;

        public string MonsterName;


        public MonsterInfo(DataRow Row)
        {
            MonsterIndex = Row["Monster Index"] is DBNull ? -1 : (int)Row["Monster Index"];
            MonsterName = Row["Monster Name"] is DBNull ? string.Empty : Row["Monster Name"].ToString();

            AI = Row["AI"] is DBNull ? 0 : (int)Row["AI"];

            Image = Row["Image"] is DBNull ? 0 : (int)Row["Image"];
            Effect = Row["Effect"] is DBNull ? 0 : (int)Row["Effect"];
            Level = Row["Level"] is DBNull ? 0 : (int)Row["Level"];
            Health = Row["Health"] is DBNull ? 0 : (int)Row["Health"];
            Experience = Row["Experience"] is DBNull ? 0 : (int)Row["Experience"];

            MinAC = Row["Min AC"] is DBNull ? 0 : (int)Row["Min AC"];
            MaxAC = Row["Max AC"] is DBNull ? 0 : (int)Row["Max AC"];
            MinMAC = Row["Min MAC"] is DBNull ? 0 : (int)Row["Min MAC"];
            MaxMAC = Row["Max MAC"] is DBNull ? 0 : (int)Row["Max MAC"];
            MinDC = Row["Min DC"] is DBNull ? 0 : (int)Row["Min DC"];
            MaxDC = Row["Max DC"] is DBNull ? 0 : (int)Row["Max DC"];
            MinMC = Row["Min MC"] is DBNull ? 0 : (int)Row["Min MC"];
            MaxMC = Row["Max MC"] is DBNull ? 0 : (int)Row["Max MC"];
            MinSC = Row["Min SC"] is DBNull ? 0 : (int)Row["Min SC"];
            MaxSC = Row["Max SC"] is DBNull ? 0 : (int)Row["Max SC"];

            Accuracy = Row["Accuracy"] is DBNull ? 0 : (int)Row["Accuracy"];
            Agility = Row["Agility"] is DBNull ? 0 : (int)Row["Agility"];
            Light = Row["Light"] is DBNull ? 0 : (int)Row["Light"];
            AttackSpeed = Row["Attack Speed"] is DBNull ? 300 : (int)Row["Attack Speed"];
            MoveSpeed = Row["Move Speed"] is DBNull ? 300 : (int)Row["Move Speed"];
        }

    }
    public class DropInfo
    {
        public MonsterInfo Monster;
        public ItemInfo Item;
        
        public int DropIndex,
                   MonsterIndex,
                   ItemIndex,
                   Rate,
                   Amount,
                   Group,
                   TriggerCount,
                   TotalCount;

        public DropInfo(DataRow Row)
        {
            DropIndex = Row["Drop Index"] is DBNull ? -1 : (int)Row["Drop Index"];
            MonsterIndex = Row["Monster Index"] is DBNull ? -1 : (int)Row["Monster Index"];
            ItemIndex = Row["Item Index"] is DBNull ? -1 : (int)Row["Item Index"];
            Rate = Row["Rate"] is DBNull ? 0 : (int)Row["Rate"];
            Amount = Row["Amount"] is DBNull ? 1 : (int)Row["Amount"];
            Group = Row["Group"] is DBNull ? 0 : (int)Row["Group"];
            TriggerCount = Row["Trigger Count"] is DBNull ? 0 : (int)Row["Trigger Count"];
            TotalCount = Row["Total Count"] is DBNull ? 0 : (int)Row["Total Count"];
        }
    }
    public class RespawnInfo
    {
        public MapInfo MInfo;
        public MonsterInfo Monster;

        public int RespawnIndex,
                   MonsterIndex,
                   MapIndex,
                   Count,
                   Spread;

        public Point Location;
        public TimeSpan Delay;

        public RespawnInfo(DataRow Row)
        {
            RespawnIndex = Row["Respawn Index"] is DBNull ? -1 : (int)Row["Respawn Index"];

            Location.X = Row["Location X"] is DBNull ? 0 : (int)Row["Location X"];
            Location.Y = Row["Location Y"] is DBNull ? 0 : (int)Row["Location Y"];

            Count = Row["Count"] is DBNull ? 0 : (int)Row["Count"];
            Spread = Row["Spread"] is DBNull ? 0 : (int)Row["Spread"];

            Delay = TimeSpan.FromMinutes(Row["Delay"] is DBNull ? 0 : (int)Row["Delay"]);
        }
    }
    public class MovementInfo
    {
        public int MovementIndex,
                   SourceIndex,
                   DestinationIndex;

        public Point SourceLocation,
                     DestinationLocation;

        public MovementInfo(DataRow Row)
        {
            MovementIndex = Row["Movement Index"] is DBNull ? -1 : (int)Row["Movement Index"];

            SourceLocation.X = Row["Source X"] is DBNull ? 0 : (int)Row["Source X"];
            SourceLocation.Y = Row["Source Y"] is DBNull ? 0 : (int)Row["Source Y"];

            DestinationIndex = Row["Destination Map"] is DBNull ? -1 : (int)Row["Destination Map"];

            DestinationLocation.X = Row["Destination X"] is DBNull ? 0 : (int)Row["Destination X"];
            DestinationLocation.Y = Row["Destination Y"] is DBNull ? 0 : (int)Row["Destination Y"];
        }
    }

    //Users
    public class AccountInfo
    {
        public int AccountIndex,
                   BanCount,
                   Gold;

        public string AccountID,
                      UserName,
                      SecretQuestion,
                      SecretAnswer,
                      EMailAddress,
                      BanReason,
                      Password,
                      CreatorIP;

        public DateTime BirthDate,
                        CreationDate,
                        ExpiryDate;

        public bool Banned;

        public List<CharacterInfo> Characters = new List<CharacterInfo>();
        
        public AccountInfo(DataRow Row)
        {
            AccountIndex = Row["Account Index"] is DBNull ? -1 : (int)Row["Account Index"];
            AccountID = Row["Account ID"] is DBNull ? string.Empty : Row["Account ID"].ToString();

            Password = Row["Password"] is DBNull ? string.Empty : Row["Password"].ToString();

            BirthDate = new DateTime(Row["Birth Date"] is DBNull ? 0 : (long)Row["Birth Date"]);
            UserName = Row["User Name"] is DBNull ? string.Empty : Row["User Name"].ToString();
            SecretQuestion = Row["Secret Question"] is DBNull ? string.Empty : Row["Secret Question"].ToString();
            SecretAnswer = Row["Secret Answer"] is DBNull ? string.Empty : Row["Secret Answer"].ToString();
            EMailAddress = Row["EMail Address"] is DBNull ? string.Empty : Row["EMail Address"].ToString();

            CreatorIP = Row["Creator IP"] is DBNull ? string.Empty : Row["Creator IP"].ToString(); ;
            CreationDate = new DateTime(Row["Creation Date"] is DBNull ? 0 : (long)Row["Creation Date"]);

            Banned = Row["Banned"] is DBNull ? false : (bool)Row["Banned"];
            BanCount = Row["Ban Count"] is DBNull ? 0 : (int)Row["Ban Count"];
            BanReason = Row["Ban Reason"] is DBNull ? string.Empty : Row["Ban Reason"].ToString();
            ExpiryDate = new DateTime(Row["Expiry Date"] is DBNull ? 0 : (long)Row["Expiry Date"]);

            Gold = Row["Gold"] is DBNull ? 0 : (int)Row["Gold"];
        }

        public List<SelectCharacterInfo> GetSelectInfo()
        {
            CharacterInfo C;
            List<SelectCharacterInfo> List = new List<SelectCharacterInfo>();
            for (int I = 0; I < Characters.Count; I++)
            {
                C = Characters[I];
                if (!C.Deleted) List.Add(C.ToSelectInfo());
            }

            return List;
        }
    }
    public class CharacterInfo
    {
        public int CharacterIndex;
        public string CharacterName;
        public int Level;
        public MirClass Class;
        public MirGender Gender;

        public string CreatorIP;
        public DateTime CreationDate;

        public bool Deleted;
        public int DeleteCount;
        public DateTime DeleteDate;

        public bool Banned;
        public int BanCount;
        public string BanReason;
        public DateTime ExpiryDate;

        public string LastIP;
        public DateTime LastAccess;

        public int MapIndex;
        public Point Location;
        public MirDirection Direction;

        public int HomeIndex;
        public Point HomeLocation;

        public int HairType;

        public int Experience;
        public int HP, MP;

        public UserItem[] Inventory = new UserItem[46], Equipment = new UserItem[14];
        public UserItem[] Storage = new UserItem[80];

        public bool Lock;

        public CharacterInfo(DataRow Row)
        {
            CharacterIndex = Row["Character Index"] is DBNull ? -1 : (int)Row["Character Index"];
            CharacterName = Row["Character Name"] is DBNull ? string.Empty : Row["Character Name"].ToString();
            Level = Row["Level"] is DBNull ? 0 : (int)Row["Level"];
            Class = (MirClass)(Row["Class"] is DBNull ? 0 : (int)Row["Class"]);
            Gender = (MirGender)(Row["Gender"] is DBNull ? 0 : (int)Row["Gender"]);

            Deleted = Row["Deleted"] is DBNull ? false : (bool)Row["Deleted"];
            DeleteCount = Row["Delete Count"] is DBNull ? 0 : (int)Row["Delete Count"];
            DeleteDate = new DateTime(Row["Delete Date"] is DBNull ? 0 : (long)Row["Delete Date"]);

            CreatorIP = Row["Creator IP"] is DBNull ? string.Empty : Row["Creator IP"].ToString(); ;
            CreationDate = new DateTime(Row["Creation Date"] is DBNull ? 0 : (long)Row["Creation Date"]);

            Banned = Row["Banned"] is DBNull ? false : (bool)Row["Banned"];
            BanCount = Row["Ban Count"] is DBNull ? 0 : (int)Row["Ban Count"];
            BanReason = Row["Ban Reason"] is DBNull ? string.Empty : Row["Ban Reason"].ToString();
            ExpiryDate = new DateTime(Row["Expiry Date"] is DBNull ? 0 : (long)Row["Expiry Date"]);

            LastAccess = new DateTime(Row["Last Access"] is DBNull ? 0 : (long)Row["Last Access"]);

            Direction = (MirDirection)(Row["Direction"] is DBNull ? 0 : (int)Row["Direction"]);

            HomeIndex = Row["Home Index"] is DBNull ? 0 : (int)Row["Home Index"];
            HomeLocation.X = Row["Home X"] is DBNull ? 0 : (int)Row["Home X"];
            HomeLocation.Y = Row["Home Y"] is DBNull ? 0 : (int)Row["Home Y"];

            MapIndex = Row["Map Index"] is DBNull ? 0 : (int)Row["Map Index"];
            Location.X = Row["Location X"] is DBNull ? 0 : (int)Row["Location X"];
            Location.Y = Row["Location Y"] is DBNull ? 0 : (int)Row["Location Y"];

            HairType = Row["Hair Type"] is DBNull ? 0 : (int)Row["Hair Type"];

            Experience = Row["Experience"] is DBNull ? 0 : (int)Row["Experience"];
            HP = Row["HP"] is DBNull ? 0 : (int)Row["HP"];
            MP = Row["MP"] is DBNull ? 0 : (int)Row["MP"];
        }
        public CharacterInfo(C.NewCharacter P)
        {
            CharacterName = P.CharacterName;
            Class = P.Class;
            Gender = P.Gender;
        }

        public SelectCharacterInfo ToSelectInfo()
        {
            return new SelectCharacterInfo
            {
                Index = CharacterIndex,
                CharacterName = CharacterName,
                Class = Class,
                Gender = Gender,
                Level = Level,
                LastAccess = LastAccess,
            };
        }
    }
}
