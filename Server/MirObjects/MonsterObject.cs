using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.MirEnvir;
using Library;
using System.Threading;
using Library.MirNetwork;
using Server.MirDatabase;
using S = Library.MirNetwork.ServerPackets;

namespace Server.MirObjects
{
    class MonsterObject : MapObject
    {
        public MonsterInfo MI;
        public MapRespawn MR;

        public const short SearchDelay = 2000;
        public long SearchTime;

        public int ViewRange = 7;

        public override int AttackSpeed
        {
            get
            {
                return MI.AttackSpeed;
            }
        }

        public MonsterObject(MapRespawn R)
        {
            MI = R.RI.Monster;
            MR = R;

            RefreshBaseStats();

            HP = MaxHP;
        }
        public void TrySpawn()
        {
            MapIndex = MR.RI.MInfo.MapIndex;
            Direction = (MirDirection)Envir.Rand.Next(8);

            for (int I = 0; I < 10; I++)
            {
                Location = MR.RI.Location;
                Location.Offset(Envir.Rand.Next(-MR.RI.Spread, MR.RI.Spread + 1), Envir.Rand.Next(-MR.RI.Spread, MR.RI.Spread + 1));

                if (Spawn())
                {
                    OperateTime = Envir.Rand.Next(OperateDelay);
                    MR.Count++;
                    break;
                }
            }
        }


        public override void RefreshAll()
        {
            RefreshBaseStats();
            base.RefreshAll();
        }
        public virtual void RefreshBaseStats()
        {
            Level = MI.Level;

            MaxHP = MI.Health;
            MinAC = MI.MinAC;
            MaxAC = MI.MaxAC;
            MinMAC = MI.MinMAC;
            MaxMAC = MI.MaxMAC;
            MinDC = MI.MinDC;
            MaxDC = MI.MaxDC;
            MinMC = MI.MinMC;
            MaxMC = MI.MaxMC;
            MinSC = MI.MinSC;
            MaxSC = MI.MaxSC;
            Accuracy = MI.Accuracy;
            Agility = MI.Agility;
            MoveSpeed = MI.MoveSpeed;
        }


        public override void CommonOperations()
        {
            base.CommonOperations();

            if (Dead && Main.Time >= DeadTime)
                CurrentMap.DeSpawn(this);
        }
        public override void AIOperations()
        {

            if (Main.Time >= SearchTime)
            {
                List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, 1);

                 if (Target == null || Envir.Rand.Next(5) == 0)
                      FindTarget();

                if (TempList.Count(O => O.Location == Location && O.Blocking) > 1)
                {
                    if (!Walk(Direction))
                    {
                        int X = Envir.Rand.Next(4) == 0 ? -1 : 1;

                        for (int I = 0; I < 7; I++)
                            Walk((MirDirection)(((int)Direction + I * X) % 8));
                    }
                }
                SearchTime = Main.Time + SearchDelay;
            }

            if (Target != null)
            {
                if (Location == Target.Location)
                {
                    Walk(Direction);
                }
                else if (Functions.InRange(Location, Target.Location, 1))
                {
                    Attack(Functions.DirectionFromPoint(Location, Target.Location));
                }
                else
                {
                    MirDirection D = Functions.DirectionFromPoint(Location, Target.Location);
                    if (!Walk(D))
                    {
                        int X = Envir.Rand.Next(4) == 0 ? -1 : 1;

                        for (int I = 0; I < 7; I++)
                            Walk((MirDirection)(((int)D + I * X) % 8));
                    }
                }
            }
            else //Insert Target Movement. 
                if (Envir.Rand.Next(10) == 0)
                    Wander();

            base.AIOperations();
        }
        
        public virtual void Wander()
        {
            if (Envir.Rand.Next(5) == 0)
                if (Envir.Rand.Next(2) == 0) Turn((MirDirection)Envir.Rand.Next(8));
                else Walk(Direction);
        }
        public virtual void FindTarget()
        { }

        public override void Die()
        {
            base.Die();

            if (MR != null) Interlocked.Decrement(ref MR.Count);

            Drop();

            if (ExpOwner != null)
            {
                ExpOwner.WinExp((int)(MI.Experience * Settings.ExpRate));
                ExpOwner = null;
            }

            Target = null;
            LastHitter = null;
        }
        public virtual void Drop()
        {
            DropInfo DInfo;
            ItemObject FloorItem;
            int Rate;
            for (int I = 0; I < MI.DropList.Count; I++)
            {
                DInfo = MI.DropList[I];

                Interlocked.Increment(ref DInfo.TotalCount);
                //for (int S = 0; S < 40; S++)
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
                            FloorItem = new ItemObject(Envir.Rand.Next(DInfo.Amount / 2, DInfo.Amount + DInfo.Amount / 2), this);
                    else
                        FloorItem = new ItemObject(MirDB.NewItem(DInfo.Item), this);

                    FloorItem.Drop();
                }
            }
        }

        public override bool Walk(MirDirection D)
        {
            if (base.Walk(D))
            {
                AttackTime = Main.Time + (AttackSpeed / 2);
                return true;
            }
            return false;

        }
        public override bool Run(MirDirection D)
        {
            if (base.Walk(D))
            {
                AttackTime = Main.Time + (AttackSpeed / 2);
                return true;
            }
            return false;

        }


        public override void Struck(MapObject Attacker)
        {
            base.Struck(Attacker);

            if (Target == null || Target.Dead || Envir.Rand.Next(4) == 0)
                Target = Attacker;
        }

        public override Packet GetMapData()
        {
            return new S.NewMonsterObject
            {
                Details = GetDetails(),
            };
        }
        public MonsterDetails GetDetails()
        {
            return new MonsterDetails
            {
                ObjectID = ObjectID,
                Name = MI.MonsterName,
                Image = MI.Image,
                Effect = MI.Effect,
                Direction = Direction,
                Location = Location,
                Dead = Dead,
                //Skeleton?
            };
        }

        public static MonsterObject MakeMonster(MapRespawn MR)
        {
            switch (MR.RI.Monster.AI)
            {
                case 1:
                case 2:
                    return new Monsters.PassiveAnimal(MR);
                case 3:
                    return new Monsters.AgressiveMonster(MR);
                default:
                    return new MonsterObject(MR);
            }


        }
    }
}
