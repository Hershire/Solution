using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.MirDatabase;
using Server.MirEnvir;
using Library;
using System.Drawing;
using Library.MirNetwork;
using S = Library.MirNetwork.ServerPackets;

namespace Server.MirObjects
{
    class ItemObject : MapObject
    {
        public int Gold;
        public UserItem Item;
        public bool Added;

        public ItemObject(int Gold, MapObject Dropper)
        {
            this.Gold = Gold;
            DeadTime = Main.Time + Settings.ItemTimeOut * 60 * 1000;

            MInfo = Dropper.MInfo;
            CurrentMap = Dropper.CurrentMap;
            MapIndex = Dropper.MapIndex;
            Location = Dropper.Location;
        }
        public ItemObject(UserItem Item, MapObject Dropper)
        {
            this.Item = Item;
            DeadTime = Main.Time + Settings.ItemTimeOut * 60 * 1000;

            MInfo = Dropper.MInfo;
            CurrentMap = Dropper.CurrentMap;
            MapIndex = Dropper.MapIndex;
            Location = Dropper.Location;
        }

        public override void CommonOperations()
        {
            if (Main.Time >= DeadTime || (Gold == 0 && Item == null))
                DeSpawn();
        }

        public override bool Blocking
        {
            get
            {
                return false;
            }
        }
        public override bool CanAttack
        {
            get
            {
                return false;
            }
        }
        public override bool CanMove
        {
            get
            {
                return false;
            }
        }
        public override bool CanRegen
        {
            get
            {
                return false;
            }
        }


        public override void Die() { }
        public override void HealthChanged() { }
        public override void LevelUp() { }
        public override void RefreshAll() { }

        public override bool Attack(MirDirection D) { return false; }
        public override bool Run(MirDirection D) { return false; }
        public override bool Turn(MirDirection D) { return false; }
        public override void Struck(MapObject O) { }
        public override bool Walk(MirDirection D) { return false; }
        public override void WinExp(int Exp) { }

        public bool Drop(int Distance = -1)
        {
            if (CurrentMap == null) return false;

            int Range = Distance == -1 ? Settings.DropRange : Distance;

            int DropX = Location.X;
            int DropY = Location.Y;

            List<MapObject> Objects = CurrentMap.GetNearbyObjects(this, Range);
            Point Temp;

            for (int Count = 0; Count <= Settings.DropStackSize; Count++)
            {
                for (int Dist = 1; Dist <= Range; Dist++)
                {
                    for (int Y = DropY - Dist; Y <= DropY + Dist; Y++)
                    {
                        if (Y < 0) continue;
                        if (Y >= CurrentMap.MapSize.Height) break;

                        for (int X = DropX - Dist; X <= DropX + Dist; X += (Y == DropY - Dist || Y == DropY + Dist || Dist == 1) ? 1 : Dist * 2)
                        {
                            if (X < 0) continue;
                            if (X >= CurrentMap.MapSize.Width) break;
                            Temp = new Point(X, Y);
                            //if (Y != DropY - Dist && Y != DropY + Dist && X != DropX - Dist && X != DropX + Dist) 
                            if (!CurrentMap.ValidPoint(Temp)) continue;

                            if (Objects.Count(Ob => Ob.Location == Temp) > 0)
                            {
                                if (Objects.Count(Ob => Ob.Blocking && Ob.Location == Temp) > 0)
                                    continue;

                                if (Objects.Count(Ob => Ob is ItemObject && Ob.Location == Temp) > Count)
                                    continue;
                            }
                            
                            Location = Temp;
                            if (Spawn()) return true;
                        }
                    }
                }
            }
            return false;
        }
        
        public override Packet GetMapData()
        {
            return new S.NewItemObject { Details = GetDetails() };
        }
        public ItemDetails GetDetails()
        {
            return new ItemDetails
            {
                ObjectID = ObjectID,
                Location = Location,
                Gold = Gold,
                ImageIndex = Item == null ? -1 : Item.Info.Image,
                ItemName = Item == null ? string.Empty : Item.Info.ItemName,
                Added = Added,
            };
        }

        public UserItem GetItem()
        {
            lock (this)
            {
                UserItem Temp = Item;
                Item = null;
                return Temp;
            }
        }
        public int GetGold()
        {
            lock (this)
            {
                int Amount = Gold;
                Gold = 0;
                return Amount;
            }
        }
    }
}
