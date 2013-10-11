using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Client.MirGraphics;
using Library.MirNetwork.ServerPackets;
using Client.MirSound;

namespace Client.MirObjects
{
    class PlayerObject : MapObject
    {
        int GOffSet = 1112, AOffSet = 2224, WOffSet = 1200;

        public MirClass Class;
        public MirGender Gender;
        public int ArmourShape, HairType;

        ImageLibrary WeaponLibrary, WeaponLibrary2, HairLibrary;

        public UserItem[] Inventory, Equipment;
        
        public int Level,
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
                   MagicResist,
                   PoisonResist,
                   HealthRegen,
                   ManaRegen,
                   Holy,
                   Luck, 
                   ASpeed,
                   CurrentHandWeight,
                   MaxHandWeight,
                   CurrentBodyWeight,
                   MaxBodyWeight,
                   CurrentBagWeight,
                   MaxBagWeight,
                   CurrentExperience,
                   MaxExperience,
                   Gold;

        public int AttackSpeed
        {
            get { return 1500 - (Globals.ASpeedRate * ASpeed); }
        }


        public PlayerObject()
        {
            Frames = HumanFrames.This;
            DoAction(MirAction.Standing);
        }

        public void NewInfo(UserDetails PInfo)
        {
            ObjectID = PInfo.ObjectID;
            Name = PInfo.Name;
            HairType = PInfo.Hair;

            Location = PInfo.Location;            
            Direction = PInfo.Direction;
            Class = PInfo.Class;
            Gender = PInfo.Gender;

            RefreshLibraries();
        }

        public void RefreshLibraries()
        {
            if (Class == MirClass.Assassin)
            {
                GOffSet = 728;
                AOffSet = 1456;
                WOffSet = 1456;
                PrimaryLibrary = Libraries.Hum_Killer;
                WeaponLibrary = Libraries.Weapon_Killer_Right;
                WeaponLibrary2 = Libraries.Weapon_Killer_Left;
                HairLibrary = Libraries.Hair_Killer;
            }
            else
            {
                GOffSet = 1112;
                AOffSet = 2224;
                WOffSet = 1200;
                PrimaryLibrary = Libraries.Hum;
                WeaponLibrary = Libraries.Weapon;
                HairLibrary = Libraries.Hair;
            }

            if (this == User)
            {
                WeaponShape = 0;
                ArmourShape = 0;
                if (Equipment != null)
                {
                    if (Equipment[(int)MirEquipmentSlot.Weapon] != null)
                        WeaponShape = Equipment[(int)MirEquipmentSlot.Weapon].Info.Shape;
                    if (Equipment[(int)MirEquipmentSlot.Armour] != null)
                        ArmourShape = Equipment[(int)MirEquipmentSlot.Armour].Info.Shape;
                }
            }

            DieSound = Gender == MirGender.Male ? SoundList.MaleDie : SoundList.FemaleDie;
            FlinchSound = Gender == MirGender.Male ? SoundList.MaleFlinch : SoundList.FemaleFlinch;

            if (WeaponShape == 6 || WeaponShape == 20)
                AttackSound = SoundList.SwingShort;
            else if (WeaponShape == 1 || WeaponShape == 27 || WeaponShape == 28 || WeaponShape == 33)
                AttackSound = SoundList.SwingWood;
            else if (WeaponShape == 2 || WeaponShape == 13 || WeaponShape == 9 || WeaponShape == 5 || WeaponShape == 14 ||
                     WeaponShape == 22 || WeaponShape == 25 || WeaponShape == 30 || WeaponShape == 35 || WeaponShape == 37 || WeaponShape == 37)
                AttackSound = SoundList.SwingSword;
            else if (WeaponShape == 4 || WeaponShape == 17 || WeaponShape == 10 || WeaponShape == 15 || WeaponShape == 16 ||
                     WeaponShape == 23 || WeaponShape == 26 || WeaponShape == 29 || WeaponShape == 31 || WeaponShape == 34)
                AttackSound = SoundList.SwingSword2;
            else if (WeaponShape == 3 || WeaponShape == 7 || WeaponShape == 11)
                AttackSound = SoundList.SwingAxe;
            else if (WeaponShape == 8 || WeaponShape == 12 || WeaponShape == 18 || WeaponShape == 21 || WeaponShape == 32)
                AttackSound = SoundList.SwingLong;
            else if (WeaponShape == 24)
                AttackSound = SoundList.SwingClub;
            else AttackSound = SoundList.SwingFist;
        }


        public override void Draw()
         {
            if (Direction == MirDirection.Left || Direction == MirDirection.Up || Direction == MirDirection.UpLeft || Direction == MirDirection.DownLeft)
                DrawWeapon();
            else
                DrawWeapon2();

            DrawBody();

            DrawHead();

            if (Direction == MirDirection.UpRight || Direction == MirDirection.Right || Direction == MirDirection.DownRight || Direction == MirDirection.Down)
                DrawWeapon();
            else
                DrawWeapon2();
        }

        public void DrawWeapon()
        {
            if (WeaponLibrary != null)
                WeaponLibrary.Draw(ImageIndex + (WeaponShape * WOffSet + (Gender == MirGender.Female && Class == MirClass.Assassin ? GOffSet : 0)),
                                   DisplayRectangle.Location, DrawColour, 1F, true);
        }
        public void DrawWeapon2()
        {
            if (WeaponLibrary2 != null)
                WeaponLibrary2.Draw(ImageIndex + (WeaponShape * WOffSet + (Gender == MirGender.Female && Class == MirClass.Assassin ? GOffSet : 0)),
                                    DisplayRectangle.Location, DrawColour, 1F, true);
        }
        public void DrawBody()
        {
            if (PrimaryLibrary != null)
                PrimaryLibrary.Draw(ImageIndex + (ArmourShape * AOffSet) + (Gender == MirGender.Male ? 0 : GOffSet), DisplayRectangle.Location, DrawColour, 1F, true);

        }
        internal void DrawHead()
        {
          if (HairLibrary != null)
                HairLibrary.Draw(ImageIndex + (1 * AOffSet) + (Gender == MirGender.Male ? 0 : GOffSet), DisplayRectangle.Location, DrawColour, 1F, true);
        }

        internal void NewStats(UserStats SInfo)
        {
            if (SInfo == null) return;

            Level = SInfo.Level;
            CurrentHP = SInfo.HP;
            MaxHP = SInfo.MaxHP;
            CurrentMP = SInfo.MP;
            MaxMP = SInfo.MaxMP;
            MinAC = SInfo.MinAC;
            MaxAC = SInfo.MaxAC;
            MinMAC = SInfo.MinMAC;
            MaxMAC = SInfo.MaxMAC;
            MinDC = SInfo.MinDC;
            MaxDC = SInfo.MaxDC;
            MinMC = SInfo.MinMC;
            MaxMC = SInfo.MaxMC;
            MinSC = SInfo.MinSC;
            MaxSC = SInfo.MaxSC;
            Accuracy = SInfo.Accuracy;
            Agility = SInfo.Agility;
            Light = SInfo.Light;
            MagicResist = SInfo.MagicResist;
            PoisonResist = SInfo.PoisonResist;
            HealthRegen = SInfo.HealthRegen;
            ManaRegen = SInfo.ManaRegen;
            Holy = SInfo.Holy;
            ASpeed = SInfo.ASpeed;
            Luck = SInfo.Luck;
            CurrentExperience = SInfo.Experience;
            MaxExperience = SInfo.MaxExperience;
            CurrentHandWeight = SInfo.CurrentHandWeight;
            MaxHandWeight = SInfo.MaxHandWeight;
            CurrentBodyWeight = SInfo.CurrentBodyWeight;
            MaxBodyWeight = SInfo.MaxBodyWeight;
            CurrentBagWeight = SInfo.CurrentBagWeight;
            MaxBagWeight = SInfo.MaxBagWeight;
            if (Light <= 3)
                Light = 3;
        }
        internal void NewInventory(UserInventory P)
        {
            Gold = P.Gold;

            if (P.Inventory != null)
                Inventory = P.Inventory;

            if (P.Equipment != null)
            {
                Equipment = P.Equipment;
                RefreshLibraries();
            }
           // if (P.Storage != null)
                ;// Storage = P.Storage;
        }


        public override void PlayStepSound()
        {
            int X = Location.X / 2 * 2;
            int Y = Location.Y / 2 * 2;
            int Img = MirScenes.Game_Scene.MapLayer.M2CellInfo[X, Y].BackImage & 0x7FFF;
            Img = MirScenes.Game_Scene.MapLayer.M2CellInfo[X, Y].FileIndex * 10000 + Img - 1;

            if ((Img >= 330 && Img <= 349) || (Img >= 450 && Img <= 454) || (Img >= 550 && Img <= 554) ||
                (Img >= 750 &&
                Img <= 754) || (Img >= 950 && Img <= 954) || (Img >= 1250 && Img <= 1254) ||
                (Img >= 1400 && Img <= 1424) || (Img >= 1455 && Img <= 1474) || (Img >= 1500 && Img <= 1524) ||
                (Img >= 1550 && Img <= 1574))
                MoveSound = SoundList.WalkLawnL;
            else if ((Img >= 250 && Img <= 254) || (Img >= 1005 && Img <= 1009) || (Img >= 1050 && Img <= 1054) ||
                (Img >= 1060 && Img <= 1064) || (Img >= 1450 && Img <= 1454) || (Img >= 1650 && Img <= 1654))
                MoveSound = SoundList.WalkRoughL;
            else if ((Img >= 605 && Img <= 609) || (Img >= 650 && Img <= 654) || (Img >= 660 && Img <= 664) ||
                (Img >= 2000 && Img <= 2049) || (Img >= 3025 && Img <= 3049) || (Img >= 2400 && Img <= 2424) ||
                (Img >= 4625 && Img <= 4649) || (Img >= 4675 && Img <= 4678))
                MoveSound = SoundList.WalkStoneL;
            else if ((Img >= 1825 && Img <= 1924) || (Img >= 2150 && Img <= 2174) || (Img >= 3075 && Img <= 3099) ||
                (Img >= 3325 && Img <= 3349) || (Img >= 3375 && Img <= 3399))
                MoveSound = SoundList.WalkCaveL;
            else if (Img == 3230 || Img == 3231 || Img == 3246 || Img == 3277 || (Img >= 3780 && Img <= 3799))
                MoveSound = SoundList.WalkWoodL;
            else if (Img >= 3825 && Img <= 4434)
                if (Img % 25 == 0) MoveSound = SoundList.WalkWoodL;
                else MoveSound = SoundList.WalkGroundL;
            else if ((Img >= 2075 && Img <= 2099) || (Img >= 2125 && Img <= 2149))
                MoveSound = SoundList.WalkRoomL;
            else if (Img >= 1800 && Img <= 1824)
                MoveSound = SoundList.WalkWaterL;
            else MoveSound = SoundList.WalkGroundL;

            if ((Img >= 825 && Img <= 1349) && (Img - 825) / 25 % 2 == 0) MoveSound = SoundList.WalkStoneL;
            if ((Img >= 1375 && Img <= 1799) && (Img - 1375) / 25 % 2 == 0) MoveSound = SoundList.WalkCaveL;
            if (Img == 1385 || Img == 1386 || Img == 1391 || Img == 1392) MoveSound = SoundList.WalkWoodL;

            Img = (MirScenes.Game_Scene.MapLayer.M2CellInfo[X, Y].MiddleImage & 0x7FFF) - 1;
            if (Img >= 0 && Img <= 115)
                MoveSound = SoundList.WalkGroundL;
            else if (Img >= 120 && Img <= 124)
                MoveSound = SoundList.WalkLawnL;

            Img = (MirScenes.Game_Scene.MapLayer.M2CellInfo[X, Y].FrontImage & 0x7FFF) - 1;
            if ((Img >= 221 && Img <= 289) || (Img >= 583 && Img <= 658) || (Img >= 1183 && Img <= 1206) ||
                (Img >= 7163 && Img <= 7295) || (Img >= 7404 && Img <= 7414))
                MoveSound = SoundList.WalkStoneL;
            else if ((Img >= 3125 && Img <= 3267) || (Img >= 3757 && Img <= 3948) || (Img >= 6030 && Img <= 6999))
                MoveSound = SoundList.WalkWoodL;
            if (Img >= 3316 && Img <= 3589)
                MoveSound = SoundList.WalkRoomL;

            if (CurrentAction == MirAction.Running) MoveSound += 2;
            if (Frame == 4) MoveSound++;

            SoundManager.PlaySound(MoveSound, false);
        }
        public override void PlayStruckSound(int WeaponType)
        {
            if (WeaponType == -1) return;
            if (ArmourShape == 3)
            {
                if (WeaponType == 1 || WeaponType == 2 || (WeaponType >= 4 && WeaponType <= 6) ||
                    WeaponType == 9 || WeaponType == 10 || (WeaponType >= 13 && WeaponType <= 17) ||
                    (WeaponType >= 22 && WeaponType <= 31) || (WeaponType >= 33 && WeaponType <= 37))
                    StruckSound = SoundList.StruckArmourSword;
                else if (WeaponType == 3 || WeaponType == 7 || WeaponType == 1)
                    StruckSound = SoundList.StruckArmourAxe;
                else if (WeaponType == 8 || WeaponType == 12 || WeaponType == 18 || WeaponType == 21 || WeaponType == 32)
                    StruckSound = SoundList.StruckArmourLongStick;
                else StruckSound = SoundList.StruckArmourFist;
                SoundManager.PlaySound(StruckSound, false);
            }
            else base.PlayStruckSound(WeaponType);
        }


        internal void AddBagItem(UserItem NI)
        {
            UserItem BI;

            if (NI.Info.StackSize > 1)
            {
                List<UserItem> Temp = new List<UserItem>(Inventory.Where(O => O != null && O.Info.ItemIndex == NI.Info.ItemIndex &&
                                                                              O.Amount < O.Info.StackSize));

                for (int I = 0; I < Temp.Count; I++)
                {
                    BI = Temp[I];

                    if (BI.Amount + NI.Amount <= BI.Info.StackSize)
                    {
                        BI.Amount += NI.Amount;
                        return;
                    }

                    NI.Amount -= BI.Info.StackSize - BI.Amount;
                    BI.Amount = BI.Info.StackSize;
                }
            }


            if (NI.Info.IsConsumable)
            {
                for (int I = 40; I < Inventory.Length; I++) // BeltStart
                {
                    if (Inventory[I] != null) continue;

                    Inventory[I] = NI;
                    return;
                }
            }


            for (int I = 0; I < Inventory.Length; I++)
            {
                if (Inventory[I] != null) continue;

                Inventory[I] = NI;
                return;
            }
        }

        public void CalculateWeight()
        {
            UserItem Temp;
            CurrentBagWeight = Inventory.Sum(O => O == null ? 0 : (O.Info.Weight * O.Amount));
            CurrentBodyWeight = 0;
            CurrentHandWeight = 0;

            for (int I = 0; I < Equipment.Length; I++)
            {
                if ((Temp = Equipment[I]) == null) continue;

                if (Temp.Info.ItemType == MirItemType.Weapon || Temp.Info.ItemType == MirItemType.Torch)
                    CurrentHandWeight += Temp.Info.Weight;
                else
                    CurrentBodyWeight += Temp.Info.Weight;
            }

        }
    }
}
