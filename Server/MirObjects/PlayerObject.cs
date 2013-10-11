using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.MirNetwork;
using Server.MirDatabase;
using Server.MirNetwork;
using Server.MirEnvir;
using S = Library.MirNetwork.ServerPackets;
using Library;
using System.Drawing;
using System.Threading;

namespace Server.MirObjects
{
    class PlayerObject : MapObject
    {
        private MirConnection Con;
        private CharacterInfo Data;

        List<MapObject> OldObjects = new List<MapObject>();

        public bool CanHarvest
        {
            get { return Main.Time >= HarvestTime; }
        }
        public bool CanUseItem
        {
            get { return !Dead; }
        }
        public int FreeSpace
        {
            get { return Inventory.Count(O => O == null); }
        }

        public long HarvestTime, HarvestDelay = 300, ShoutTime, ShoutDelay = 10000;
        public bool SpiritSetComplete;


        public MirClass Class;
        public MirGender Gender;

        public UserItem[] Inventory, Equipment, Storage;

        public int CurrentHandWeight, MaxHandWeight,
                      CurrentBodyWeight, MaxBodyWeight,
                      CurrentBagWeight, MaxBagWeight;

        public PlayerObject(CharacterInfo TempData, MirConnection Con)
        {
            TempData.Lock = true;

            this.Con = Con;
            this.Data = TempData;


            LoadData();

            if (Level == 0)
                NewCharacter();
            else
            {
                RefreshAll();
                if (HP <= 0)
                {
                    HP = 10 + (int)(MaxHP * 0.1F);
                    MP = 10 + (int)(MaxMP * 0.1F);
                    Location = HomeLocation;
                    MapIndex = HomeMapIndex;
                }
            }
        }

        public override bool Spawn()
        {
            if (!base.Spawn()) return false;

            QueuePacket(new S.MapInfomation { Details = CurrentMap.GetDetails() });
            QueuePacket(new S.UserInformation { Details = GetDetails() });
            QueuePacket(new S.UpdateUserStats { Details = GetStats() });
            QueuePacket(new S.UserInventory { Gold = Con.Account.Gold, Equipment = Equipment, Inventory = Inventory });

            GetNewObjects();

            return true;
        }
        public override void DeSpawn()
        {
            base.DeSpawn();

            Data.Lock = false;
            SaveData();
        }

        public void LoadData()
        {
            Name = Data.CharacterName;
            
            Inventory = Data.Inventory.ToArray();
            Equipment = Data.Equipment.ToArray();
            Storage = Data.Storage.ToArray();

            Class = Data.Class;
            Gender = Data.Gender;

            Level = Data.Level;
            HairType = Data.HairType;
            
            MapIndex = Data.MapIndex;
            Location = Data.Location;
            Direction = Data.Direction;

            HomeMapIndex = Data.HomeIndex;
            HomeLocation = Data.HomeLocation;

            HP = Data.HP;
            MP = Data.MP;
            Experience = Data.Experience;                        
        }
        public void SaveData()
        {
            Data.CharacterName = Name;

            Data.Inventory = Inventory.ToArray();
            Data.Equipment = Equipment.ToArray();
            Data.Storage = Storage.ToArray();

            Data.Class = Class;
            Data.Gender = Gender;

            Data.Level = Level;
            Data.HairType = HairType;

            Data.MapIndex = MapIndex;
            Data.Location = Location;
            Data.Direction = Direction;

            Data.HomeIndex = HomeMapIndex;
            Data.HomeLocation = HomeLocation;

            Data.HP = HP;
            Data.MP = MP;
            Data.Experience = Experience;

            Data.LastAccess = Main.Now;
            Data.LastIP = Con.IPAddress;
        }

        private void NewCharacter()
        {
            if (MirDB.StartPoints.Count == 0) return;

            Level = Settings.StartLevel;
            HairType = 1;
            
            for (int I = 0; I < MirDB.StartItems.Count; I++)            
                if (CorrectStartItem(MirDB.StartItems[I]))
                    AddBagItem(MirDB.NewItem(MirDB.StartItems[I]));


            SafeZoneInfo SZI = MirDB.StartPoints[Envir.Rand.Next(MirDB.StartPoints.Count)];

            MapIndex = SZI.MapIndex;
            Location = SZI.Location;

            HomeMapIndex = MapIndex;
            HomeLocation = Location;

            RefreshAll();
            Recover(MaxHP, MaxMP);
        }


        public override void HealthChanged()
        {
            QueuePacket(new S.ObjectHealthChanged() { ObjectID = ObjectID, HP = HP, MP = MP, MaxHP = MaxHP, MaxMP = MaxMP });
        }

        public override void RefreshAll()
        {
            RefreshLevelStats();
            RefreshEquipmentStats();
            base.RefreshAll();
        }
        public void RefreshLevelStats()
        {
            //Update Max Exp here.
            MaxExperience = Level < Settings.ExperienceList.Count ? Settings.ExperienceList[Level - 1] : Settings.ExperienceList[Settings.ExperienceList.Count - 1];

            MaxHP = 0; MaxMP = 0;
            MinAC = 0; MaxAC = 0;
            MinMAC = 0; MaxMAC = 0;
            MinDC = 0; MaxDC = 0;
            MinMC = 0; MaxMC = 0;
            MinSC = 0; MaxSC = 0;

            Accuracy = 5; Agility = 15;

            //Other Stats;
            MaxBagWeight = 0;
            MaxBodyWeight = 0;
            MaxHandWeight = 0;
            ASpeed = 0;
            Luck = 0;
            Light = 0;
            MagicResist = 1;
            PoisonResist = 0;
            HealthRegen = 0;
            ManaRegen = 0;
            Holy = 0;

            switch (Class)
            {
                case MirClass.Warrior:
                    MaxHP = 14 + (int)((Level / 4F + 4.5F + Level / 20F) * Level);
                    MaxMP = 11 + (int)(Level * 3.5F);

                    MaxAC = Level / 7;
                    MinDC = Level / 5;
                    MaxDC = Level / 5 + 1;


                    MaxBagWeight = (ushort)(50 + Level / 3F * Level);
                    MaxBodyWeight = (ushort)(15 + Level / 20F * Level);
                    MaxHandWeight = (ushort)(12 + Level / 13F * Level);
                    break;
                case MirClass.Wizard:
                    MaxHP = 14 + (int)((Level / 15 + 1.8F) * Level);
                    MaxMP = 13 + (int)((Level / 5F + 2F) * 2.2F * Level);

                    MinDC = Level / 7;
                    MaxDC = Level / 7 + 1;
                    MinMC = Level / 7;
                    MaxMC = Level / 7 + 1;

                    MaxBagWeight = (ushort)(50 + Level / 5F * Level);
                    MaxBodyWeight = (ushort)(15 + Level / 100F * Level);
                    MaxHandWeight = (ushort)(12 + Level / 90F * Level);
                    break;
                case MirClass.Taoist:
                    MaxHP = 14 + (int)((Level / 6F + 2.5F) * Level);
                    MaxMP = 13 + (int)(Level / 8F * 2.2F * Level);

                    MinMAC = Level / 12;
                    MaxMAC = Level / 6 + 1;
                    MinDC = Level / 7;
                    MaxDC = Level / 7 + 1;
                    MinSC = Level / 7;
                    MaxSC = Level / 7 + 1;

                    MaxBagWeight = (ushort)(50 + Level / 4F * Level);
                    MaxBodyWeight = (ushort)(15 + Level / 50F * Level);
                    MaxHandWeight = (ushort)(12 + Level / 42F * Level);

                    Agility += 3;
                    break;
                case MirClass.Assassin:
                    MaxHP = 14 + (int)((Level / 15F + 1.8F) * Level);
                    MaxMP = 11 + (int)(Level * 5.7F);

                    MinDC = Level / 6;
                    MaxDC = Level / 6 + 1;

                    MaxBagWeight = (ushort)(50 + Level / 4F * Level);
                    MaxBodyWeight = (ushort)(15 + Level / 50F * Level);
                    MaxHandWeight = (ushort)(12 + Level / 30F * Level);
                    break;
            }
        }
        public void RefreshEquipmentStats()
        {
            UserItem Temp;
            CurrentBagWeight = Inventory.Sum(O => O == null ? 0 : (O.Info.Weight * O.Amount));
            CurrentBodyWeight = 0;
            CurrentHandWeight = 0;

            bool MundaneBrace = false, MundaneRing = false, NokChiBrace = false, NokChiRing = false, TaoProtectBrace = false, TaoProtectRing = false;
            bool SpiritBlade = false, SpiritHelmet = false, SpiritNecklace = false, SpiritBrace = false, SpiritRing = false;

            for (int I = 0; I < Equipment.Length; I++)
            {
                if ((Temp = Equipment[I]) == null) continue;

                MaxHP += Temp.AddedHealth + Temp.Info.Health;
                MaxMP += Temp.AddedMana + Temp.Info.Mana;
                MinAC += Temp.Info.MinAC;
                MaxAC += Temp.AddedAC + Temp.Info.MaxAC;
                MinMAC += Temp.Info.MinMAC;
                MaxMAC += Temp.AddedMAC + Temp.Info.MaxMAC;
                MinDC += Temp.Info.MinDC;
                MaxDC += Temp.AddedDC + Temp.Info.MaxDC;
                MinMC += Temp.Info.MinMC;
                MaxMC += Temp.AddedMC + Temp.Info.MaxMC;
                MinSC += Temp.Info.MinSC;
                MaxSC += Temp.AddedSC + Temp.Info.MaxSC;

                Accuracy += Temp.AddedAccuracy + Temp.Info.Accuracy;
                Agility += Temp.AddedAgility + Temp.Info.Agility;
                MagicResist += Temp.AddedMagicResist + Temp.Info.MagicResist;
                PoisonResist += Temp.AddedPoisonResist + Temp.Info.PoisonResist;
                HealthRegen += Temp.AddedHealthRegen + Temp.Info.HealthRegen;
                ManaRegen += Temp.AddedManaRegen + Temp.Info.ManaRegen;
                Holy += Temp.Info.Holy;
                ASpeed += Temp.AddedAttackSpeed + Temp.Info.AttackSpeed;
                Luck += Temp.AddedLuck + Temp.Info.Luck;

                if (Temp.Info.Light > Light) Light = Temp.Info.Light;

                MaxBagWeight += Temp.AddedBagWeight + Temp.Info.BagWeight;
                MaxBodyWeight += Temp.AddedBodyWeight + Temp.Info.BodyWeight;
                MaxHandWeight += Temp.AddedHandWeight + Temp.Info.HandWeight;

                if (Temp.Info.ItemType == MirItemType.Weapon || Temp.Info.ItemType == MirItemType.Torch)
                    CurrentHandWeight += Temp.Info.Weight;
                else
                    CurrentBodyWeight += Temp.Info.Weight;

                //Sets
                switch (Temp.Info.Set)
                {
                    case SetType.Mundane:
                        switch (Temp.Info.ItemType)
                        {
                            case MirItemType.Bracelet:
                                MundaneBrace = true;
                                break;
                            case MirItemType.Ring:
                                MundaneRing = true;
                                break;
                        }
                        break;
                    case SetType.NokChi:
                        switch (Temp.Info.ItemType)
                        {
                            case MirItemType.Bracelet:
                                NokChiBrace = true;
                                break;
                            case MirItemType.Ring:
                                NokChiRing = true;
                                break;
                        }
                        break;
                    case SetType.TaoProtect:
                        switch (Temp.Info.ItemType)
                        {
                            case MirItemType.Bracelet:
                                TaoProtectBrace = true;
                                break;
                            case MirItemType.Ring:
                                TaoProtectRing = true;
                                break;
                        }
                        break;
                    case SetType.Spirit:
                        switch (Temp.Info.ItemType)
                        {
                            case MirItemType.Weapon:
                                SpiritBlade = true;
                                break;
                            case MirItemType.Helmet:
                                SpiritHelmet = true;
                                break;
                            case MirItemType.Necklace:
                                SpiritNecklace = true;
                                break;
                            case MirItemType.Bracelet:
                                SpiritBrace = true;
                                break;
                            case MirItemType.Ring:
                                SpiritRing = true;
                                break;
                        }
                        break;
                }
            }


            if (MundaneBrace && MundaneRing)
                MaxHP += 50;

            if (NokChiBrace && NokChiRing)
                MaxMP += 50;

            if (TaoProtectBrace && TaoProtectRing)
            {
                MaxHP += 30;
                MaxMP += 30;
            }

            SpiritSetComplete = SpiritBlade && SpiritHelmet && SpiritNecklace && SpiritBrace && SpiritRing;

            if (SpiritSetComplete)
            {
                MinDC += 2;
                MaxDC += 5;
                ASpeed += 2;
            }
        }

        public void Chat(string Message)
        {
            if (Message.Length == 0) return;

            int TempI;
            string TempS;
            Packet TempP = null;

            switch (Message[0])
            {
                case '/': // PM
                    TempS = Message.Substring(1);
                    TempI = TempS.IndexOf(' ');
                    if (TempI != -1)
                    {
                        TempS = TempS.Substring(0, TempI);
                        TempI += 2;
                    }
                    else TempI = Message.Length;

                    PlayerObject P = Network.GetPlayer(TempS);
                    if (P == null)
                    {
                        QueuePacket(new S.ObjectChat { Message = string.Format("User {0} not found.", TempS), Type = MirChatType.RedSystem });
                        return;
                    }
                    P.QueuePacket(new S.ObjectChat { Name = Name, Message = Message.Substring(TempI), Type = MirChatType.Whisper });
                    return;
                case '!':
                    if (Message.Length == 1)
                    {
                        if (ShoutTime > Main.Time)
                        {
                            ReceiveMessage(string.Format("You cannot shout for another {0} seconds.", (ShoutTime - Main.Time) / 1000), MirChatType.RedSystem);
                            return;
                        }

                        TempP = new S.ObjectChat { Name = Name, Message = Message.Substring(1), Type = MirChatType.Shout };
                        
                        for (int I = 0; I < CurrentMap.Players.Count; I++)
                        {
                            if (CurrentMap.Players[I] == this) continue;
                            CurrentMap.Players[I].QueuePacket(TempP);
                        }

                        ShoutTime = Main.Time + ShoutDelay;
                        break;
                    }
                    switch (Message[1])
                    {
                        case '!': // Group

                            break;
                        case '~': //Guild

                            break;
                        default ://Shout
                            if (ShoutTime > Main.Time)
                            {
                                ReceiveMessage(string.Format("You cannot shout for another {0} seconds.", (ShoutTime - Main.Time) / 1000), MirChatType.RedSystem);
                                return;
                            }

                            TempP = new S.ObjectChat { Name = Name, Message = Message.Substring(1), Type = MirChatType.Shout };

                            for (int I = 0; I < CurrentMap.Players.Count; I++)
                            {
                                if (CurrentMap.Players[I] == this) continue;
                                CurrentMap.Players[I].QueuePacket(TempP);
                            }

                            ShoutTime = Main.Time + ShoutDelay;
                            break;
                    }
                    break;

                case '@': //Command
                    TempS = Message.Substring(1);
                    TempI = TempS.IndexOf(' ');
                    if (TempI != -1)
                    {
                        TempS = TempS.Substring(0, TempI);
                        TempI += 2;
                    }
                    else TempI = Message.Length;

                    ReceiveMessage(string.Format("Command {0} not found.", TempS), MirChatType.RedSystem);
                    return;
                default: // Normal
                    TempP = new S.ObjectChat { Name = Name, Message = Message, Type = MirChatType.Normal };
                    List<PlayerObject> TempList = NearByPlayers;
                    for (int I = 0; I < TempList.Count; I++)
                        TempList[I].QueuePacket(TempP);
                    break;
            }
            if (TempP != null)
                QueuePacket(TempP);
        }

        public void ReceiveMessage(string Message, MirChatType Type, string Name = "")
        {
            S.ObjectChat P = new S.ObjectChat { Message = Message, Name = Name, Type = Type };
            QueuePacket(P);
        }

        public override void MapChanged()
        {
            QueuePacket(new S.MapInfomation { Details = CurrentMap.GetDetails() });

            base.MapChanged();
        }
        public override bool Turn(MirDirection D)
        {
            bool R = base.Turn(D);

            QueuePacket(new S.PlayerLocation { Location = Location, Direction = Direction });

            return R;
        }
        public override bool Walk(MirDirection D)
        {
            MapInfo OldMInfo = MInfo;
            bool R = base.Walk(D);
            
            QueuePacket(new S.PlayerLocation { Location = Location, Direction = Direction });

            if (OldMInfo == MInfo) GetNewObjects();
            
            return R;
        }
        public override bool Run(MirDirection D)
        {
            MapInfo OldMInfo = MInfo;
            bool R = base.Run(D);

            QueuePacket(new S.PlayerLocation { Location = Location, Direction = Direction });

            if (OldMInfo == MInfo) GetNewObjects();

            return R;
        }
        public override bool Attack(MirDirection D)
        {
            //Type
            bool R = base.Attack(D);

            if (!R)
            {
                //QueuePacket(new S.PlayerLocation { Location = CurrentLocation, Direction = Direction });
                RegenTime = Main.Time + RegenDelay;
            }

            return R;
        }
        public void Harvest(MirDirection D)
        {
            if (!CanHarvest) return;

            HarvestTime = Main.Time + HarvestDelay;
            Direction = D;
            Broadcast(new S.ObjectHarvest { ObjectID = ObjectID, Direction = Direction });

            Point P = Functions.PointMove(Location, D, 1);
            List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, P, 2);

            for (int I = 0; I < TempList.Count; I++)            
                if (TempList[I].Harvested(this)) return;            
        }

        public override void Struck(MapObject Attacker)
        {
            base.Struck(Attacker);

            QueuePacket(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = Attacker.ObjectID });
            RegenTime = Main.Time + RegenDelay;
        }

        public override void WinExp(int Exp)
        {
            QueuePacket(new S.WinExperience { Amount = Exp });
            base.WinExp(Exp);
        }
        public override void LevelUp()
        {
            base.LevelUp();
            ReceiveMessage(string.Format("Congratulations, your level has been increased, your level now is {0}.", Level), MirChatType.BlueSystem);
            QueuePacket(new S.UpdateUserStats { Details = GetStats() });
        }

        public void QueuePacket(Packet P)
        {
            Con.QueuePacket(P);
        }
        public override void AddObject(MapObject O)
        {
            if (OldObjects.Contains(O)) return;

            OldObjects.Add(O);

            if (O is PlayerObject) OldPlayers.Add(O as PlayerObject);

            QueuePacket(O.GetMapData());
        }
        public override void RemoveObject(MapObject O)
        {
            base.RemoveObject(O);
            OldObjects.Remove(O);
            QueuePacket(new S.RemoveMapObject { ObjectID = O.ObjectID });
        }
        public void GetNewObjects()
        {
            List<MapObject> TempList = NearbyObjects;

            MapObject Temp; 
            for (int I = 0; I < TempList.Count; I++)
            {
                Temp = TempList[I];
                if (OldObjects.Contains(Temp)) continue;
                Temp.AddObject(this);
                QueuePacket(Temp.GetMapData());
            }


            for (int I = 0; I < OldObjects.Count; I++)
            {
                Temp = OldObjects[I];
                if (TempList.Contains(Temp)) continue;
                Temp.RemoveObject(this);
                QueuePacket(new S.RemoveMapObject { ObjectID = Temp.ObjectID });
            }

            OldObjects = TempList;
        }

        public override Packet GetMapData()
        {
            return new S.NewPlayerObject
            {
                Details = GetDetails(),
            };
        }

        private UserDetails GetDetails()
        {
            return new UserDetails
            {
                ObjectID = ObjectID,
                Name = Name,
                Class = Class,
                Gender = Gender,
                Direction = Direction,
                Location = Location,
                Hair = HairType,
            };
        }
        private UserStats GetStats()
        {
            return new UserStats
            {
                Level = Level,
                HP = HP,
                MaxHP = MaxHP,
                MP = MP,
                MaxMP = MaxMP,
                MinAC = MinAC,
                MaxAC = MaxAC,
                MinMAC = MinMAC,
                MaxMAC = MaxMAC,
                MinDC = MinDC,
                MaxDC = MaxDC,
                MinMC = MinMC,
                MaxMC = MaxMC,
                MinSC = MinSC,
                MaxSC = MaxSC,
                Accuracy = Accuracy,
                Agility = Agility,
                Light = Light,
                MaxBagWeight = MaxBagWeight,
                MagicResist = MagicResist,
                PoisonResist = PoisonResist,
                HealthRegen = HealthRegen,
                ManaRegen = ManaRegen,
                Holy = Holy,
                ASpeed = ASpeed,
                Luck = Luck,
                Experience = Experience,
                MaxExperience = MaxExperience,
                CurrentHandWeight = CurrentHandWeight,
                MaxHandWeight = MaxHandWeight,
                CurrentBodyWeight = CurrentBodyWeight,
                MaxBodyWeight = MaxBodyWeight,
                CurrentBagWeight = CurrentBagWeight,
            };
        }

        public void MoveItem(MirGridType G, int F, int T)
        {
            UserItem[] Array;
            switch (G)
            {
                case MirGridType.Inventory:
                    Array = Inventory;
                    break;
                case MirGridType.Storage:
                    Array = Storage;
                    break;
                default: 
                    return;
            }

            if (F >= 0 && T >= 0 && F < Array.Length && T < Array.Length)
            {
                UserItem I = Array[T];
                Array[T] = Array[F];
                Array[F] = I;
            }
            //Stacking
        }
        public void EquipItem(MirGridType G, int U, int T)
        {
            if (!CanUseItem || T < 0 || T >= Equipment.Length) return;

            UserItem[] Array;
            switch (G)
            {
                case MirGridType.Inventory:
                    Array = Inventory;
                    break;
                case MirGridType.Storage:
                    Array = Storage;
                    break;
                default:
                    return;
            }


            int Index = -1;
            UserItem Temp = null;

            for (int I = 0; I < Array.Length; I++)
            {
                Temp = Array[I];
                if (Temp != null && Temp.UniqueID == U)
                {
                    Index = I;
                    break;
                }
            }


            if (Temp == null || Index == -1)
            {
                //Send Fail
                return;
            }

            if (CanEquipItem(Temp, T))
            {
                Array[Index] = Equipment[T];
                Equipment[T] = Temp;
                RefreshAll();
                QueuePacket(new S.UpdateUserStats { Details = GetStats() });
                //Broadcast Change in Details
            }
        }
        public void RemoveItem(MirGridType G, int U, int T)
        {
            if (!CanUseItem) return;

            UserItem[] Array;
            switch (G)
            {
                case MirGridType.Inventory:
                    Array = Inventory;
                    break;
                case MirGridType.Storage:
                    Array = Storage;
                    break;
                default:
                    return;
            }

            if (T < 0 || T >= Array.Length) return;

            UserItem Temp = null;
            int Index = -1;

            for (int I = 0; I < Equipment.Length; I++)
            {
                Temp = Equipment[I];
                if (Temp != null && Temp.UniqueID == U)
                {
                    Index = I;
                    break;
                }
            }

            if (Temp == null || Index == -1)
            {
                //Send Fail
                return;
            }

            if (!CanRemoveItem(G, Temp)) return;
            Equipment[Index] = null;

            if (Array[T] == null)
                Array[T] = Temp;
            else for (int I = 0; I < Array.Length; I++)
                    if (Array[I] == null)
                    {
                        Array[I] = Temp;
                        break;
                    }


            RefreshAll();
            QueuePacket(new S.UpdateUserStats { Details = GetStats() });
        }
        public void UseItem(MirGridType G, int U)
        {
            if (!CanUseItem)
            {
                //Send Fail
                return;
            }
            
            UserItem Temp = null;
            int Index = -1;

            for (int I = 0; I < Inventory.Length; I++)
            {
                Temp = Inventory[I];
                if (Temp != null && Temp.UniqueID == U)
                {
                    Index = I;
                    break;
                }
            }

            if (Temp == null || Index == -1)
            {
                //Send Fail (?)
                return;
            }

            switch (Temp.Info.ItemType)
            {
                case MirItemType.Potion:
                    Interlocked.Add(ref PotHealthAmount, Temp.Info.Health);
                    Interlocked.Add(ref PotManaAmount, Temp.Info.Mana);
                    break;
                default:
                    return;
            }
            if (Temp.Amount > 1) Temp.Amount--;
            else Inventory[Index] = null;
        }

        public bool CanEquipItem(UserItem I, int EquipmentSlot)
        {
            switch ((MirEquipmentSlot)EquipmentSlot)
            {
                case MirEquipmentSlot.Weapon:
                    if (I.Info.ItemType != MirItemType.Weapon)
                        return false;
                    break;
                case MirEquipmentSlot.Armour:
                    if (Gender == MirGender.Male)
                    {
                        if (I.Info.ItemType != MirItemType.ArmourMale)
                            return false;
                    }
                    else if (I.Info.ItemType != MirItemType.ArmourFemale)
                        return false;
                    break;
                case MirEquipmentSlot.Helmet:
                    if (I.Info.ItemType != MirItemType.Helmet)
                        return false;
                    break;
                case MirEquipmentSlot.Torch:
                    if (I.Info.ItemType != MirItemType.Torch)
                        return false;
                    break;
                case MirEquipmentSlot.Necklace:
                    if (I.Info.ItemType != MirItemType.Necklace)
                        return false;
                    break;
                case MirEquipmentSlot.BraceletL:
                case MirEquipmentSlot.BraceletR:
                    if (I.Info.ItemType != MirItemType.Bracelet)
                        return false;
                    break;
                case MirEquipmentSlot.RingL:
                case MirEquipmentSlot.RingR:
                    if (I.Info.ItemType != MirItemType.Ring)
                        return false;
                    break;
                case MirEquipmentSlot.Amulet:
                    if (I.Info.ItemType != MirItemType.Amulet)
                        return false;
                    break;
                case MirEquipmentSlot.Boots:
                    if (I.Info.ItemType != MirItemType.Boots)
                        return false;
                    break;
                case MirEquipmentSlot.Belt:
                    if (I.Info.ItemType != MirItemType.Belt)
                        return false;
                    break;
                case MirEquipmentSlot.Stone:
                    if (I.Info.ItemType != MirItemType.Stone)
                        return false;
                    break;
                case MirEquipmentSlot.Tiger:
                    if (I.Info.ItemType != MirItemType.Tiger)
                        return false;
                    break;
                default:
                    return false;
            }


            switch (Class)
            {
                case MirClass.Warrior:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Warrior))
                        return false;
                    break;
                case MirClass.Wizard:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Wizard))
                        return false;
                    break;
                case MirClass.Taoist:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Taoist))
                        return false;
                    break;
                case MirClass.Assassin:
                    if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Assassin))
                        return false;
                    break;
            }
            switch (I.Info.RequiredClass)
            {
                case MirRequiredClass.Warrior:
                    if (Class != MirClass.Warrior)
                        return false;
                    break;
                case MirRequiredClass.Wizard:
                    if (Class != MirClass.Wizard)
                        return false;
                    break;
                case MirRequiredClass.Taoist:
                    if (Class != MirClass.Taoist)
                        return false;
                    break;
                case MirRequiredClass.Assassin:
                    if (Class != MirClass.Assassin)
                        return false;
                    break;
                case MirRequiredClass.WarWizTao:
                    if (Class == MirClass.Assassin)
                        return false;
                    break;
            }


            switch (I.Info.RequiredType)
            {
                case MirRequiredType.Level:
                    if (Level < I.Info.RequiredAmount)
                        return false;
                    break;
                case MirRequiredType.AC:
                    if (MaxAC < I.Info.RequiredAmount)
                        return false;
                    break;
                case MirRequiredType.MAC:
                    if (MaxMAC < I.Info.RequiredAmount)
                        return false;
                    break;
                case MirRequiredType.DC:
                    if (MaxMAC < I.Info.RequiredAmount)
                        return false;
                    break;
                case MirRequiredType.MC:
                    if (MaxMAC < I.Info.RequiredAmount)
                        return false;
                    break;
                case MirRequiredType.SC:
                    if (MaxMAC < I.Info.RequiredAmount)
                        return false;
                    break;
            }

            if (I.Info.ItemType == MirItemType.Weapon || I.Info.ItemType == MirItemType.Torch)
            {
                if (I.Info.Weight - (Equipment[EquipmentSlot] != null ? Equipment[EquipmentSlot].Info.Weight : 0) + CurrentHandWeight > MaxHandWeight)
                    return false;
            }
            else
                if (I.Info.Weight - (Equipment[EquipmentSlot] != null ? Equipment[EquipmentSlot].Info.Weight : 0) + CurrentBodyWeight > MaxBodyWeight)
                    return false;

            return true;
        }
        public bool CanRemoveItem(MirGridType G, UserItem I)
        {
            UserItem[] Array;
            switch (G)
            {
                case MirGridType.Inventory:
                    Array = Inventory;
                    break;
                case MirGridType.Storage:
                    Array = Storage;
                    break;
                default:
                    return false;
            }

            return Array.Count(O => O == null) > 0;
        }
        public bool CorrectStartItem(ItemInfo II)
        {
            if (Gender == MirGender.Male && II.ItemType == MirItemType.ArmourFemale) return false;
            if (Gender == MirGender.Female && II.ItemType == MirItemType.ArmourMale) return false;

            switch (Class)
            {
                case MirClass.Warrior:
                    if (!II.RequiredClass.HasFlag(MirRequiredClass.Warrior)) return false;
                    break;
                case MirClass.Wizard:
                    if (!II.RequiredClass.HasFlag(MirRequiredClass.Wizard)) return false;
                    break;
                case MirClass.Taoist:
                    if (!II.RequiredClass.HasFlag(MirRequiredClass.Taoist)) return false;
                    break;
                case MirClass.Assassin:
                    if (!II.RequiredClass.HasFlag(MirRequiredClass.Assassin)) return false;
                    break;
            }

            return true;
        }

        public void AddBagItem(UserItem NI)
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
        public void GainGold(int Amount)
        {
            Con.Account.Gold += Amount;

            QueuePacket(new S.GainedGold { Amount = Amount });
        }
        public void GainItem(UserItem Item)
        {
            QueuePacket(new S.GainedItem { Item = Item });

            AddBagItem(Item);
        }        
        public bool CanGainItem(UserItem NI)
        {
            if (FreeSpace > 0) return true;
            
            UserItem BI;
            int Amount = NI.Amount;

            if (NI.Info.StackSize > 1)
            {
                List<UserItem> Temp = new List<UserItem>(Inventory.Where(O => O != null && O.Info.ItemIndex == NI.Info.ItemIndex && 
                                                                              O.Amount < O.Info.StackSize));

                for (int I = 0; I < Temp.Count; I++)
                {
                    BI = Temp[I];

                    if (BI.Amount + Amount <= BI.Info.StackSize) return true;

                    Amount -= BI.Info.StackSize - BI.Amount;
                }
            }

            return false;

        }
        public void PickUp()
        {
            List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, 0);

            if (TempList.Count == 0) return;

            ItemObject Temp = TempList.FirstOrDefault(O => O is ItemObject) as ItemObject;

            if (Temp == null) return;


            if (Temp.Gold > 0)
            {
                if (Temp.Gold + Con.Account.Gold >= int.MaxValue) return;

                int Amount = Temp.GetGold();

                if (Amount == 0) return;
                GainGold(Amount);
            }
            else if (Temp.Item != null)
            {
                if (!CanGainItem(Temp.Item)) return;

                UserItem TempItem = Temp.GetItem();

                if (TempItem == null) return;

                GainItem(TempItem);
            }

        }

        public void DropItem(MirGridType G, int U)
        {
            UserItem[] Array;
            switch (G)
            {
                case MirGridType.Inventory:
                    Array = Inventory;
                    break;
                case MirGridType.Storage:
                    Array = Storage;
                    break;
                default:
                    return;
            }

            UserItem Temp = null;
            int Index = -1;

            for (int I = 0; I < Inventory.Length; I++)
            {
                Temp = Inventory[I];
                if (Temp != null && Temp.UniqueID == U)
                {
                    Index = I;
                    break;
                }
            }

            if (Temp == null || Index == -1)
            {
                //Send Fail (?)
                return;
            }

            Array[Index] = null;
            DropItem(Temp);
        }
        public void DropGold(int Amount)
        {
            if (Con.Account.Gold < Amount || Amount < 0)            
                return;

            Con.Account.Gold -= Amount;

            ItemObject Ob = new ItemObject(Amount, this);

            if (!Ob.Drop(this is PlayerObject ? 1 : Settings.DropRange))
                GainGold(Amount);
        }
    }
}
