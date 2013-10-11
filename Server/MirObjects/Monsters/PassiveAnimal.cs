using System;
using Library;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using System.Threading;
using Server.MirDatabase;
using Library.MirNetwork;
using S = Library.MirNetwork.ServerPackets;

namespace Server.MirObjects.Monsters
{
    class PassiveAnimal : MonsterObject
    {
        public bool Skinned, RunAway;
        public int RemainingSkinCount;
        public int Quality;


        public override bool CanAttack
        {
            get
            {
                return base.CanAttack && !RunAway;
            }
        }

        public PassiveAnimal(MapRespawn R)
            : base(R)
        {
            if (MI.AI == 1)
            {
                RemainingSkinCount = 5;
            }
            else if (MI.AI == 2)
            {
                RunAway = Envir.Rand.Next(7) == 0;
                RemainingSkinCount = 10;

                if (RunAway)
                {
                    Quality = Envir.Rand.Next(8) * 2000;
                    MoveSpeed -= 300;
                }
            }

        }

        public override void RefreshBaseStats()
        {
            base.RefreshBaseStats();

            if (RunAway)
                MoveSpeed -= 300;
        }

        public override void Drop() { }
       
        public override bool Harvested(PlayerObject O)
        {
            if (!Dead || Skinned) return false;

            if (Interlocked.Decrement(ref RemainingSkinCount) == 0)
            {
                DropInfo DInfo;
                UserItem TempItem;
                bool FoundItem = false;
                int Rate;

                for (int I = 0; I < MI.DropList.Count; I++)
                {
                    DInfo = MI.DropList[I];

                    Interlocked.Increment(ref DInfo.TotalCount);

                    Rate = (int)(DInfo.Rate / Settings.DropRate); if (Rate < 1) Rate = 1;
                    if (Envir.Rand.Next(Rate) == 0)
                    {
                        if (DInfo.Group != 0)
                        {
                            List<DropInfo> TempList = MI.DropList.Where(D => D.Group == DInfo.Group && D.TriggerCount < DInfo.TriggerCount).ToList();

                            if (TempList.Count != 0)
                                DInfo = TempList[Envir.Rand.Next(TempList.Count)];
                        }
                        Interlocked.Increment(ref DInfo.TriggerCount);

                        if (DInfo.Item == MirDB.GoldItem)
                            if (DInfo.Amount <= 0) continue;
                            else
                                O.GainGold(DInfo.Amount);
                        else 
                        {
                            TempItem = MirDB.NewItem(DInfo.Item);
                            if (TempItem.Info.ItemType == MirItemType.Meat)
                                TempItem.CurrentDurability += Quality;
                            if (O.CanGainItem(TempItem))
                                O.GainItem(TempItem);
                            else
                                O.DropItem(TempItem);
                            
                        }
                        FoundItem = true;
                    }
                }

                if (!FoundItem)
                    O.ReceiveMessage("You found nothing.", MirChatType.RedSystem);

                Skinned = true;
                Packet Pack = new S.ObjectSkeleton { ObjectID = ObjectID };
                Broadcast(Pack);
            }

            return true;
        }

        public override void AIOperations()
        {
            if (RunAway && Target != null)
            {
                MirDirection D = Functions.DirectionFromPoint(Target.Location, Location);
                if (!Walk(D))
                {
                    int X = Envir.Rand.Next(4) == 0 ? -1 : 1;

                    for (int I = 0; I < 7; I++)
                        Walk((MirDirection)(((int)D + I * X) % 8));
                }
            }
            base.AIOperations();
        }
    }
}
