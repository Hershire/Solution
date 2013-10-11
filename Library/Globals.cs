using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Runtime.Serialization;
using System.Data;

namespace Library
{
    public enum MirGender : byte
    {
        Male = 0,
        Female = 1
    }
    public enum MirClass : byte
    {
        Warrior = 0,
        Wizard = 1,
        Taoist = 2,
        Assassin = 3
    }
    public enum CellAttribute : byte
    {
        Walk = 0,
        HighWall = 1,
        LowWall = 2,
    }
    public enum MirDirection : byte
    {
        Up = 0,
        UpRight = 1,
        Right = 2,
        DownRight = 3,
        Down = 4,
        DownLeft = 5,
        Left = 6,
        UpLeft = 7
    }
    public enum MirAction : byte
    {
        Standing,
        Turn,
        Harvest,
        Walking,
        Running,
        Attack1,
        Attack2,
        Attack3,
        Stance,
        Struck,
        Die,
        Dead,
        Skeleton,
    }

    [Flags]
    public enum MirRequiredClass : byte
    {
        Warrior = 1,
        Wizard = 2,
        Taoist = 4,
        Assassin = 8,
        WarWizTao = Warrior | Wizard | Taoist, // 7
        None = WarWizTao | Assassin // 15
    }

    public enum SetType
    {
        None = 0,
        Mundane = 1,
        NokChi = 2,
        TaoProtect = 3,
        Spirit = 4,
        Recall = 5,
        RedOrchid = 6,
        RedFlower = 7,
        Smash = 8,
        Hwan = 9,
        Purity = 10,
        FiveString = 11,
        Bone = 12,
        Bug = 13,
        WhiteGold = 14,
        RedJade = 15,
        Nephrite = 16,
        WhiteGoldH = 17,
        RedJadeH = 18,
        NephriteH = 19,
        Mir = 20
    }

    public enum MirEquipmentSlot : byte
    {
        Weapon = 0,
        Armour = 1,
        Helmet = 2,
        Torch = 3,
        Necklace = 4,
        BraceletL = 5,
        BraceletR = 6,
        RingL = 7,
        RingR = 8,
        Amulet = 9,
        Belt = 10,
        Boots = 11,
        Stone = 12,
        Tiger = 13,
    }
    public enum MirGridType : byte
    {
        None = 0,
        Inventory = 1,
        Equipment = 2,
        Trade = 3,
        Storage = 4,
        AccountStorage = 5
    }
    public enum MirItemType
    {
        Nothing = 0,
        Weapon = 1,
        ArmourMale = 2,
        ArmourFemale = 3,
        Helmet = 4,
        Necklace = 5,
        Bracelet = 6,
        Ring = 7,
        Amulet = 8,
        Belt = 9,
        Boots = 10,
        Stone = 11,
        Torch = 12,
        Potion = 13,
        Ore = 14,
        Meat = 15,
        CraftingMaterial = 16,
        Scroll = 17,
        Gem = 18,
        Tiger = 19,
    }
    public enum MirRequiredType : byte
    {
        Level = 0,
        AC = 1,
        MAC = 2,
        DC = 3,
        MC = 4,
        SC = 5,
    }
    public enum LightSetting
    {
        Normal = 0,
        Dawn = 1,
        Day = 2,
        Evening = 3,
        Night = 4
    }
    public enum FightSetting
    {
        Normal = 0,
        Safe = 1,
        Fight = 2
    }

    public enum MirChatType : byte
    {
        Normal,
        Whisper,
        OutBoundWhisper,
        Group,
        Guild,
        Shout,
        Global,
        BlueSystem,
        RedSystem,
        Experience
    }

    public class UserDetails
    {
        public long ObjectID;
        public string Name;
        public int Hair;
        public MirClass Class;
        public MirGender Gender;
        public MirDirection Direction;
        public Point Location;
        public bool Dead;

        public UserDetails()
        { }
        public UserDetails(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Name = BReader.ReadString();
            Hair = BReader.ReadInt32();
            Class = (MirClass)BReader.ReadByte();
            Gender = (MirGender)BReader.ReadByte();
            Direction = (MirDirection)BReader.ReadByte();
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
            Dead = BReader.ReadBoolean();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(Name ?? string.Empty);
            BWriter.Write(Hair);
            BWriter.Write((byte)Class);
            BWriter.Write((byte)Gender);
            BWriter.Write((byte)Direction);
            BWriter.Write(Location.X); BWriter.Write(Location.Y);
            BWriter.Write(Dead);
        }
    }

    public class MonsterDetails
    {
        public long ObjectID;
        public string Name;
        public int Image, Effect;
        public MirDirection Direction;
        public Point Location;
        public bool Dead;

        public MonsterDetails()
        { }
        public MonsterDetails(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            Name = BReader.ReadString();
            Image = BReader.ReadInt32();
            Effect = BReader.ReadInt32();
            Direction = (MirDirection)BReader.ReadByte();
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
            Dead = BReader.ReadBoolean();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(Name ?? string.Empty);
            BWriter.Write(Image);
            BWriter.Write(Effect);
            BWriter.Write((byte)Direction);
            BWriter.Write(Location.X); BWriter.Write(Location.Y);
            BWriter.Write(Dead);
        }
    }

    public class ItemDetails
    {
        public long ObjectID;
        public string ItemName;
        public Point Location;
        public int Gold, ImageIndex;
        public bool Added;
    
    
        public ItemDetails()
        { }
        public ItemDetails(BinaryReader BReader)
        {
            ObjectID = BReader.ReadInt64();
            ItemName = BReader.ReadString();
            Location = new Point(BReader.ReadInt32(), BReader.ReadInt32());
            Gold = BReader.ReadInt32();
            ImageIndex = BReader.ReadInt32();
            Added = BReader.ReadBoolean();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(ObjectID);
            BWriter.Write(ItemName ?? string.Empty);
            BWriter.Write(Location.X); BWriter.Write(Location.Y);
            BWriter.Write(Gold);
            BWriter.Write(ImageIndex);
            BWriter.Write(Added);
        }
    }



    public class UserStats
    {
        public int Level,
                   HP, 
                   MaxHP,
                   MP, 
                   MaxMP,
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
                   Accuracy,
                   Agility, 
                   Light,              
                   MagicResist,
                   PoisonResist,
                   HealthRegen,
                   ManaRegen,
                   Holy,
                   ASpeed,
                   Luck,
                   Experience,                    
                   MaxExperience,
                   CurrentHandWeight,
                   MaxHandWeight,
                   CurrentBodyWeight,
                   MaxBodyWeight,
                   CurrentBagWeight, 
                   MaxBagWeight;

        public UserStats()
        { }
        public UserStats(BinaryReader BReader)
        {
            Level = BReader.ReadInt32();

            HP = BReader.ReadInt32();
            MaxHP = BReader.ReadInt32();
            MP = BReader.ReadInt32();
            MaxMP = BReader.ReadInt32();

            MinAC = BReader.ReadInt32();
            MaxAC = BReader.ReadInt32();
            MinMAC = BReader.ReadInt32();
            MaxMAC = BReader.ReadInt32();
            MinDC = BReader.ReadInt32();
            MaxDC = BReader.ReadInt32();
            MinMC = BReader.ReadInt32();
            MaxMC = BReader.ReadInt32();
            MinSC = BReader.ReadInt32();
            MaxSC = BReader.ReadInt32();

            Accuracy = BReader.ReadInt32();
            Agility = BReader.ReadInt32();
            Light = BReader.ReadInt32();
            MagicResist = BReader.ReadInt32();
            PoisonResist = BReader.ReadInt32();
            HealthRegen = BReader.ReadInt32();
            ManaRegen = BReader.ReadInt32();
            Holy = BReader.ReadInt32();

            ASpeed = BReader.ReadInt32();
            Luck = BReader.ReadInt32();

            Experience = BReader.ReadInt32();
            MaxExperience = BReader.ReadInt32();

            CurrentHandWeight = BReader.ReadInt32();
            MaxHandWeight = BReader.ReadInt32();
            CurrentBodyWeight = BReader.ReadInt32();
            MaxBodyWeight = BReader.ReadInt32();
            CurrentBagWeight = BReader.ReadInt32();
            MaxBagWeight = BReader.ReadInt32();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(Level);
            BWriter.Write(HP);
            BWriter.Write(MaxHP);
            BWriter.Write(MP);
            BWriter.Write(MaxMP);
            BWriter.Write(MinAC);
            BWriter.Write(MaxAC);
            BWriter.Write(MinMAC);
            BWriter.Write(MaxMAC);
            BWriter.Write(MinDC);
            BWriter.Write(MaxDC);
            BWriter.Write(MinMC);
            BWriter.Write(MaxMC);
            BWriter.Write(MinSC);
            BWriter.Write(MaxSC);
            BWriter.Write(Accuracy);
            BWriter.Write(Agility);
            BWriter.Write(Light);
            BWriter.Write(MagicResist);
            BWriter.Write(PoisonResist);
            BWriter.Write(HealthRegen);
            BWriter.Write(ManaRegen);
            BWriter.Write(Holy);
            BWriter.Write(ASpeed);
            BWriter.Write(Luck);
            BWriter.Write(Experience);
            BWriter.Write(MaxExperience);
            BWriter.Write(CurrentHandWeight);
            BWriter.Write(MaxHandWeight);
            BWriter.Write(CurrentBodyWeight);
            BWriter.Write(MaxBodyWeight);
            BWriter.Write(CurrentBagWeight);
            BWriter.Write(MaxBagWeight);
        }
    }
    

    public class UserItem
    {
        public int UniqueID;
        
        public ItemInfo Info;

        public int ItemIndex;

        public int CurrentDurability,
                   MaxDurability,
                   Amount = 1,
                   AddedAC,
                   AddedMAC,
                   AddedDC,
                   AddedMC,
                   AddedSC,
                   AddedHealth,
                   AddedMana,
                   AddedAccuracy,
                   AddedAgility,
                   AddedMagicResist,
                   AddedPoisonResist,
                   AddedHealthRegen,
                   AddedManaRegen,
                   AddedBodyWeight,
                   AddedHandWeight,
                   AddedBagWeight,
                   AddedLuck,
                   AddedAttackSpeed;

        public UserItem(ItemInfo I) { Info = I; ItemIndex = I.ItemIndex; }

        public UserItem(DataRow Row)
        {
            ItemIndex = Row["Item Index"] is DBNull ? -1 : (int)Row["Item Index"];
            UniqueID = Row["Unique ID"] is DBNull ? -1 : (int)Row["Unique ID"];

            CurrentDurability = Row["Current Durability"] is DBNull ? 0 : (int)Row["Current Durability"];
            MaxDurability = Row["Max Durability"] is DBNull ? 0 : (int)Row["Max Durability"];
            Amount = Row["Amount"] is DBNull ? 1 : (int)Row["Amount"];

            AddedAC = Row["Added AC"] is DBNull ? 0 : (int)Row["Added AC"];
            AddedMAC = Row["Added MAC"] is DBNull ? 0 : (int)Row["Added MAC"];
            AddedDC = Row["Added DC"] is DBNull ? 0 : (int)Row["Added DC"];
            AddedMC = Row["Added MC"] is DBNull ? 0 : (int)Row["Added MC"];
            AddedSC = Row["Added SC"] is DBNull ? 0 : (int)Row["Added SC"];
            AddedHealth = Row["Added Health"] is DBNull ? 0 : (int)Row["Added Health"];
            AddedMana = Row["Added Mana"] is DBNull ? 0 : (int)Row["Added Mana"];

            AddedAccuracy = Row["Added Accuracy"] is DBNull ? 0 : (int)Row["Added Accuracy"];
            AddedAgility = Row["Added Agility"] is DBNull ? 0 : (int)Row["Added Agility"];

            AddedMagicResist = Row["Added Magic Resist"] is DBNull ? 0 : (int)Row["Added Magic Resist"];
            AddedPoisonResist = Row["Added Poison Resist"] is DBNull ? 0 : (int)Row["Added Poison Resist"];
            AddedHealthRegen = Row["Added Health Regen"] is DBNull ? 0 : (int)Row["Added Health Regen"];
            AddedManaRegen = Row["Added Mana Regen"] is DBNull ? 0 : (int)Row["Added Mana Regen"];

            AddedBodyWeight = Row["Added Body Weight"] is DBNull ? 0 : (int)Row["Added Body Weight"];
            AddedHandWeight = Row["Added Hand Weight"] is DBNull ? 0 : (int)Row["Added Hand Weight"];
            AddedBagWeight = Row["Added Bag Weight"] is DBNull ? 0 : (int)Row["Added Bag Weight"];

            AddedLuck = Row["Added Luck"] is DBNull ? 0 : (int)Row["Added Luck"];
            AddedAttackSpeed = Row["Added Attack Speed"] is DBNull ? 0 : (int)Row["Added Attack Speed"];
        }

        public UserItem(BinaryReader BReader)
        {
            UniqueID = BReader.ReadInt32();
            
            Info = new ItemInfo(BReader);

            CurrentDurability = BReader.ReadInt32();
            MaxDurability = BReader.ReadInt32();
            Amount = BReader.ReadInt32();

            AddedAC = BReader.ReadInt32();
            AddedMAC = BReader.ReadInt32();
            AddedDC = BReader.ReadInt32();
            AddedMC = BReader.ReadInt32();
            AddedSC = BReader.ReadInt32();
            AddedHealth = BReader.ReadInt32();
            AddedMana = BReader.ReadInt32();

            AddedAccuracy = BReader.ReadInt32();
            AddedAgility = BReader.ReadInt32();

            AddedMagicResist = BReader.ReadInt32();
            AddedPoisonResist = BReader.ReadInt32();
            AddedHealthRegen = BReader.ReadInt32();
            AddedManaRegen = BReader.ReadInt32();

            AddedBodyWeight = BReader.ReadInt32();
            AddedHandWeight = BReader.ReadInt32();
            AddedBagWeight = BReader.ReadInt32();

            AddedLuck = BReader.ReadInt32();
            AddedAttackSpeed = BReader.ReadInt32();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(UniqueID);

            Info.Save(BWriter);

            BWriter.Write(CurrentDurability);
            BWriter.Write(MaxDurability);
            BWriter.Write(Amount);

            BWriter.Write(AddedAC);
            BWriter.Write(AddedMAC);
            BWriter.Write(AddedDC);
            BWriter.Write(AddedMC);
            BWriter.Write(AddedSC);
            BWriter.Write(AddedHealth);
            BWriter.Write(AddedMana);

            BWriter.Write(AddedAccuracy);
            BWriter.Write(AddedAgility);

            BWriter.Write(AddedMagicResist);
            BWriter.Write(AddedPoisonResist);
            BWriter.Write(AddedHealthRegen);
            BWriter.Write(AddedManaRegen);

            BWriter.Write(AddedBodyWeight);
            BWriter.Write(AddedHandWeight);
            BWriter.Write(AddedBagWeight);

            BWriter.Write(AddedLuck);
            BWriter.Write(AddedAttackSpeed);
        }

    }

    [Serializable]
    public class ItemInfo
    {
        public int ItemIndex;
        public string ItemName;
        public MirItemType ItemType;
        public MirRequiredType RequiredType;
        public MirRequiredClass RequiredClass = MirRequiredClass.None;
        public int RequiredAmount,
                   Durability,
                   StackSize = 1,
                   Price, 
                   Image, 
                   Shape,
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
                   Health, 
                   Mana,
                   Weight,
                   Light,    
                   Accuracy,
                   Agility,
                   MagicResist,
                   PoisonResist,
                   HealthRegen,
                   ManaRegen,
                   Holy,
                   BodyWeight,
                   HandWeight,
                   BagWeight,
                   Luck,
                   AttackSpeed;

        public SetType Set;

        public bool CanBreak, CanRepair, CanSRepair, CanDrop, CanTrade, CanStore, CanSell, StartItem;

        public bool IsConsumable
        {
            get { return ItemType == MirItemType.Potion || ItemType == MirItemType.Scroll; }
        }

        public ItemInfo() { }
        public ItemInfo(DataRow Row)
        {
            ItemIndex = Row["Item Index"] is DBNull ? -1 : (int)Row["Item Index"];
            ItemName = Row["Item Name"] is DBNull ? string.Empty : Row["Item Name"].ToString();
            ItemType = (MirItemType)(Row["Item Type"] is DBNull ? 0 : (int)Row["Item Type"]);
            RequiredType = (MirRequiredType)(Row["Required Type"] is DBNull ? 0 : (int)Row["Required Type"]);
            RequiredClass = (MirRequiredClass)(Row["Required Class"] is DBNull ? 0 : (int)Row["Required Class"]);
            RequiredAmount = Row["Required Amount"] is DBNull ? 0 : (int)Row["Required Amount"];

            Durability = Row["Durability"] is DBNull ? 0 : (int)Row["Durability"];
            StackSize = Row["Stack Size"] is DBNull ? 0 : (int)Row["Stack Size"];
            Price = Row["Price"] is DBNull ? 0 : (int)Row["Price"];
            Image = Row["Image Index"] is DBNull ? -1 : (int)Row["Image Index"];

            Shape = Row["Shape"] is DBNull ? 0 : (int)Row["Shape"];

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

            Health = Row["Health"] is DBNull ? 0 : (int)Row["Health"];
            Mana = Row["Mana"] is DBNull ? 0 : (int)Row["Mana"];

            Weight = Row["Weight"] is DBNull ? 0 : (int)Row["Weight"];
            Light = Row["Light"] is DBNull ? 0 : (int)Row["Light"];
            Accuracy = Row["Accuracy"] is DBNull ? 0 : (int)Row["Accuracy"];
            Agility = Row["Agility"] is DBNull ? 0 : (int)Row["Agility"];
            MagicResist = Row["Magic Resist"] is DBNull ? 0 : (int)Row["Magic Resist"];
            PoisonResist = Row["Poison Resist"] is DBNull ? 0 : (int)Row["Poison Resist"];
            HealthRegen = Row["Health Regen"] is DBNull ? 0 : (int)Row["Health Regen"];
            ManaRegen = Row["Mana Regen"] is DBNull ? 0 : (int)Row["Mana Regen"];
            Holy = Row["Holy"] is DBNull ? 0 : (int)Row["Holy"];

            BodyWeight = Row["Body Weight"] is DBNull ? 0 : (int)Row["Body Weight"];
            HandWeight = Row["Hand Weight"] is DBNull ? 0 : (int)Row["Hand Weight"];
            BagWeight = Row["Bag Weight"] is DBNull ? 0 : (int)Row["Bag Weight"];
            Luck = (Row["Luck"] is DBNull ? 0 : (int)Row["Luck"]);
            AttackSpeed = (Row["Attack Speed"] is DBNull ? 0 : (int)Row["Attack Speed"]);

            Set = (SetType)(Row["Set"] is DBNull ? 0 : (int)Row["Set"]);

            CanBreak = Row["Can Break"] is DBNull ? false : (bool)Row["Can Break"];
            CanRepair = Row["Can Repair"] is DBNull ? false : (bool)Row["Can Repair"];
            CanSRepair = Row["Can SRepair"] is DBNull ? false : (bool)Row["Can SRepair"];
            CanDrop = Row["Can Drop"] is DBNull ? false : (bool)Row["Can Drop"];
            CanTrade = Row["Can Trade"] is DBNull ? false : (bool)Row["Can Trade"];
            CanStore = Row["Can Store"] is DBNull ? false : (bool)Row["Can Store"];
            CanSell = Row["Can Sell"] is DBNull ? false : (bool)Row["Can Sell"];
            StartItem = Row["Start Item"] is DBNull ? false : (bool)Row["Start Item"];            
        }
        public ItemInfo(BinaryReader BReader)
        {
            ItemIndex = BReader.ReadInt32();
            ItemName = BReader.ReadString();
            ItemType = (MirItemType)BReader.ReadByte();
            RequiredType = (MirRequiredType)BReader.ReadByte();
            RequiredClass = (MirRequiredClass)BReader.ReadByte();
            RequiredAmount = BReader.ReadInt32();

            Durability = BReader.ReadInt32();
            StackSize = BReader.ReadInt32();
            Price = BReader.ReadInt32();
            Image = BReader.ReadInt32();

            Shape = BReader.ReadInt32();

            MinAC = BReader.ReadInt32();
            MaxAC = BReader.ReadInt32();
            MinMAC = BReader.ReadInt32();
            MaxMAC = BReader.ReadInt32();
            MinDC = BReader.ReadInt32();
            MaxDC = BReader.ReadInt32();
            MinMC = BReader.ReadInt32();
            MaxMC = BReader.ReadInt32();
            MinSC = BReader.ReadInt32();
            MaxSC = BReader.ReadInt32();

            Health = BReader.ReadInt32();
            Mana = BReader.ReadInt32();

            Weight = BReader.ReadInt32();
            Light = BReader.ReadInt32();
            Accuracy = BReader.ReadInt32();
            Agility = BReader.ReadInt32();
            MagicResist = BReader.ReadInt32();
            PoisonResist = BReader.ReadInt32();
            HealthRegen = BReader.ReadInt32();
            ManaRegen = BReader.ReadInt32();
            Holy = BReader.ReadInt32();

            BodyWeight = BReader.ReadInt32();
            HandWeight = BReader.ReadInt32();
            BagWeight = BReader.ReadInt32();

            Luck = BReader.ReadInt32();
            AttackSpeed = BReader.ReadInt32();

            Set = (SetType)BReader.ReadByte();

            CanBreak = BReader.ReadBoolean();
            CanRepair = BReader.ReadBoolean();
            CanSRepair = BReader.ReadBoolean();
            CanDrop = BReader.ReadBoolean();
            CanTrade = BReader.ReadBoolean();
            CanStore = BReader.ReadBoolean();
            CanSell = BReader.ReadBoolean();
            

        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(ItemIndex);
            BWriter.Write(ItemName ?? string.Empty);
            BWriter.Write((byte)ItemType);
            BWriter.Write((byte)RequiredType);
            BWriter.Write((byte)RequiredClass);
            BWriter.Write(RequiredAmount);

            BWriter.Write(Durability);
            BWriter.Write(StackSize);
            BWriter.Write(Price);
            BWriter.Write(Image);
            BWriter.Write(Shape);

            BWriter.Write(MinAC);
            BWriter.Write(MaxAC);
            BWriter.Write(MinMAC);
            BWriter.Write(MaxMAC);
            BWriter.Write(MinDC);
            BWriter.Write(MaxDC);
            BWriter.Write(MinMC);
            BWriter.Write(MaxMC);
            BWriter.Write(MinSC);
            BWriter.Write(MaxSC);

            BWriter.Write(Health);
            BWriter.Write(Mana);

            BWriter.Write(Weight);
            BWriter.Write(Light);
            BWriter.Write(Accuracy);
            BWriter.Write(Agility);
            BWriter.Write(MagicResist);
            BWriter.Write(PoisonResist);
            BWriter.Write(HealthRegen);
            BWriter.Write(ManaRegen);
            BWriter.Write(Holy);

            BWriter.Write(BodyWeight);
            BWriter.Write(HandWeight);
            BWriter.Write(BagWeight);

            BWriter.Write(Luck);
            BWriter.Write(AttackSpeed);

            BWriter.Write((byte)Set);

            BWriter.Write(CanBreak);
            BWriter.Write(CanRepair);
            BWriter.Write(CanSRepair);
            BWriter.Write(CanDrop);
            BWriter.Write(CanTrade);
            BWriter.Write(CanStore);
            BWriter.Write(CanSell);
        }


        public override string ToString()
        {
            return string.Format("{0}:\t{1}", ItemIndex, ItemName);
        }
    }

    public class ClientMapInfo
    {
        public int MapIndex;
        public string FileName;
        public string MapName;
        public int MiniMap;//, BigMap;
        public LightSetting Lights;

        public ClientMapInfo()
        {        }
        public ClientMapInfo(BinaryReader BReader)
        {
            MapIndex = BReader.ReadInt32();
            FileName = BReader.ReadString();
            MapName = BReader.ReadString();
            MiniMap = BReader.ReadInt32();
            Lights = (LightSetting)BReader.ReadByte();
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(MapIndex);
            BWriter.Write(FileName);
            BWriter.Write(MapName);
            BWriter.Write(MiniMap);
            BWriter.Write((byte)Lights);
        }

    }
    public class SelectCharacterInfo
    {
        public int Index;
        public string CharacterName;
        public int Level;
        public MirClass Class;
        public MirGender Gender;
        public DateTime LastAccess;

        public SelectCharacterInfo()
        { }
        public SelectCharacterInfo(BinaryReader BReader)
        {
            Index = BReader.ReadInt32();
            CharacterName = BReader.ReadString();
            Level = BReader.ReadInt32();
            Class = (MirClass)BReader.ReadByte();
            Gender = (MirGender)BReader.ReadByte();
            LastAccess = DateTime.FromBinary(BReader.ReadInt64());
        }
        public void Save(BinaryWriter BWriter)
        {
            BWriter.Write(Index);
            BWriter.Write(CharacterName ?? string.Empty);
            BWriter.Write(Level);
            BWriter.Write((byte)Class);
            BWriter.Write((byte)Gender);
            BWriter.Write(LastAccess.ToBinary());
        }

    }
    public static class Globals
    {
        public const int CellWidth = 48, CellHeight = 32;

        public const int MinAccountIDLength = 3;
        public const int MaxAccountIDLength = 15;

        public const int MinPasswordLength = 5;
        public const int MaxPasswordLength = 15;

        public const int MinCharacterNameLength = 3;
        public const int MaxCharacterNameLength = 15;
        public const int MaxCharacterCount = 4;

        public const int DataRange = 14;

        public const int BaseAttackSpeed = 1500, ASpeedRate = 50;
        public const int MaxChatLength = 40;
    }
}
