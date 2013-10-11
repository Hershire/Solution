using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.MirDatabase;
using Server.MirEnvir;
using System.Drawing;
using Library.MirNetwork;
using Library;
using S = Library.MirNetwork.ServerPackets;

namespace Server.MirObjects
{
    class MapObject
    {
        public readonly long ObjectID = Envir.ObjectID;
        public MapInfo MInfo;

        public Map CurrentMap;

        public List<PlayerObject> OldPlayers = new List<PlayerObject>();
        public List<PlayerObject> NearByPlayers
        {
            get { return CurrentMap.GetNearbyPlayers(this); }
        }
        public List<MapObject> NearbyObjects
        {
            get { return CurrentMap.GetNearbyObjects(this); }
        }

        public const short OperateDelay = 200, ExpOwnerDelay = 5000, LastHitDelay = 5000, RegenDelay = 10000, PotDelay = 400, HealDelay = 600;

        public virtual bool Blocking
        {
            get
            {
                return !Dead;
            }
        }
        public virtual bool CanMove
        {
            get
            {
                return !Dead && (Main.Time >= MoveTime);
            }
        }
        public virtual bool CanAttack
        {
            get
            {
                return !Dead && (Main.Time >= AttackTime);
            }
        }
        public virtual bool CanOperate
        {
            get
            {
                return Main.Time >= OperateTime;
            }
        }
        public virtual bool CanRegen
        {
            get { return !Dead && Main.Time >= RegenTime; }
        }
        public virtual bool ShowHealth
        {
            get { return ShowHealthTime > Main.Time; }
        }

        public long MoveTime, AttackTime, DeadTime, LastHitTime, ExpOwnerTime, OperateTime, RegenTime, PotTime, HealTime, ShowHealthTime;
        public int DeadDelay = 60000;
        public int MoveSpeed = 500;

        public virtual int AttackSpeed
        {
            get { return Globals.BaseAttackSpeed - (ASpeed * Globals.ASpeedRate) - 100; } 
        }

        public int PotHealthAmount, PotManaAmount, HealAmount;

        public MapObject ExpOwner, LastHitter, Target, Master;

        public string Name;

        public int MapIndex, HomeMapIndex;
        public Point Location, HomeLocation;
        public MirDirection Direction;

        public bool Dead;

        public int HairType;

        public int Level;
        public int HP, MaxHP;
        public int MP, MaxMP;

        public int MinAC, MaxAC,
                   MinMAC, MaxMAC,
                   MinDC, MaxDC,
                   MinMC, MaxMC,
                   MinSC, MaxSC;

        public int Accuracy, Agility, Light, MagicResist, PoisonResist, HealthRegen, ManaRegen, Holy;
        public int ASpeed, Luck;

        public int Experience, MaxExperience;
        
        protected MapObject()
        {
            RegenTime = Main.Time + RegenDelay;
        }
        public MapObject(int MapIndex)
        {
            this.MapIndex = MapIndex;
            RegenTime = Main.Time + RegenDelay;
        }

        public virtual bool Spawn()
        {
            if (MInfo != null && MInfo.MapIndex != MapIndex)
            {
                CurrentMap.DeSpawn(this);
                MInfo = null;
                CurrentMap = null;
            }

            if (MInfo == null)
            {
                MInfo = MirDB.MapInfoList.FirstOrDefault(M => M.MapIndex == MapIndex);
                CurrentMap = Envir.MapList.FirstOrDefault(M => M.MapInfo == MInfo);
            }

            if (CurrentMap == null) return false;

            if (!CurrentMap.Spawn(this)) return false;
            

            List<MapObject> TempList = NearbyObjects;

            MapObject Temp;
            for (int I = 0; I < TempList.Count; I++)
            {
                Temp = TempList[I];
                if (Temp is PlayerObject) OldPlayers.Add((PlayerObject)Temp);
                Temp.AddObject(this);
            }


            return true;
        }
        public virtual void DeSpawn()
        {
            if (CurrentMap == null) return;

            List<MapObject> TempList = NearbyObjects;
            for (int I = 0; I < TempList.Count; I++)
                TempList[I].RemoveObject(this);

            CurrentMap.DeSpawn(this);
        }
        public virtual void MapChanged()
        {
            for (int I = 0; I < OldPlayers.Count; I++)
                OldPlayers[I].RemoveObject(this);
            
            List<MapObject> TempList = NearbyObjects;

            MapObject Temp;
            for (int I = 0; I < TempList.Count; I++)
            {
                Temp = TempList[I];
                if (Temp is PlayerObject) OldPlayers.Add((PlayerObject)Temp);
                Temp.AddObject(this);
            }

        }
        public virtual bool CheckMovement()
        {
            MovementInfo MI = MInfo.MovementList.FirstOrDefault(O => O.SourceLocation == Location);

            if (MI != null)
            {
                Map M = Envir.MapList.FirstOrDefault(O => O.MapInfo.MapIndex == MI.DestinationIndex);

                if (M != null && M.InMapSize(MI.DestinationLocation) && M.ValidPoint(MI.DestinationLocation))
                {
                    CurrentMap.DeSpawn(this);
                    Location = MI.DestinationLocation;
                    M.Spawn(this);
                    MInfo = M.MapInfo;
                    CurrentMap = M;
                    MapChanged();
                    return true;
                }
            }
            return false;
        }

        public void Process()
        {
            AIOperations();
            CommonOperations();
        }
        public virtual void CommonOperations()
        {
            OperateTime = Main.Time +OperateDelay;

            if (ExpOwner != null && (Main.Time >= ExpOwnerTime || ExpOwner.Dead || ExpOwner.MInfo == null))
                ExpOwner = null;

            if (LastHitter != null && (LastHitter.Dead || LastHitter.MInfo == null)) 
                LastHitter = null;

            if (Target != null && (Target.Dead || Target.MInfo != MInfo || !Functions.InRange(Location, Target.Location, 15)))
                Target = null;


            if (!Dead)
            {
                int HealthRegen = 0, ManaRegen = 0;

                if (CanRegen)
                {
                    RegenTime = Main.Time + RegenDelay;


                    if (HP < MaxHP)
                        HealthRegen += (int)(MaxHP * 0.03F) + 1;

                    if (MP < MaxMP)
                        ManaRegen += (int)(MaxMP * 0.03F) + 1;
                }

                if (Main.Time > PotTime)
                {
                    PotTime = Main.Time + PotDelay;
                    if (PotHealthAmount > 5)
                    {
                        HealthRegen += 5;
                        PotHealthAmount -= 5;
                    }
                    else
                    {
                        HealthRegen += PotHealthAmount;
                        PotHealthAmount = 0;
                    }

                    if (PotManaAmount > 5)
                    {
                        ManaRegen += 5;
                        PotManaAmount -= 5;
                    }
                    else
                    {
                        ManaRegen += PotManaAmount;
                        PotManaAmount = 0;
                    }

                }

                if (Main.Time > HealTime)
                {
                    HealTime = Main.Time + HealDelay;

                    if (HealAmount > 5)
                    {
                        HealthRegen += 5;
                        HealAmount -= 5;
                    }
                    else
                    {
                        HealthRegen += HealAmount;
                        HealAmount = 0;
                    }
                }

                if (HealthRegen != 0 || ManaRegen != 0)
                    Recover(HealthRegen, ManaRegen);

                if (HP >= MaxHP)
                {
                    PotHealthAmount = 0;
                    HealAmount = 0;
                }

                if (MP >= MaxMP)
                    PotManaAmount = 0;
            }
        }
        public void Recover(int Health, int Mana)
        {
            Health += (int)(Health / 10F * HealthRegen);
            Mana += (int)(Mana / 10F * ManaRegen);

            if (HP + Health > int.MaxValue)
                HP = int.MaxValue;
            else if ((HP += Health) > MaxHP)
                HP = MaxHP;

            if (MP + Mana > int.MaxValue)
                MP = int.MaxValue;
            else if ((MP += Mana) > MaxMP)
                MP = MaxMP;

            HealthChanged();
        }
        public virtual void AIOperations()
        {

        }


        public bool IsAttackTarget(MapObject Ob)
        {
            if (this is PlayerObject && Ob is MonsterObject)
                return Ob.Master == null || IsAttackTarget(Ob.Master);

            if (this is MonsterObject && Ob is PlayerObject)
                return Master == null || Master.IsAttackTarget(Ob);

            //Mob vs Mob

            if (this is PlayerObject && Ob is PlayerObject)
            {
                //if (MInfo.FightMode == FightSetting.Safe)
                //    return false;
                //Mode

                return true;
            }

            return false;
        }
        public virtual bool Attack(MapObject Ob)
        {
            if (Ob == null || Ob == this || Envir.Rand.Next(Ob.Agility + 1) > Accuracy) return false;

            int Armour = Envir.Rand.Next(Ob.MinAC, Ob.MaxAC + 1);
            int Damage = Envir.Rand.Next(MinDC, MaxDC + 1); // Get Luck

            if (Damage - Armour <= 0) return false;

            Ob.HP -= Damage - Armour;

            Ob.Struck(this);

            return true;
        }

        public virtual bool Turn(MirDirection D)
        {
            if (!CanMove) return false;


            Direction = D;

            if (CheckMovement()) return true;

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction });

            return true;
        }
        public virtual bool Walk(MirDirection D)
        {
            if (!CanMove) return false;

            Point P = Functions.PointMove(Location, D, 1);

            if (!CurrentMap.InMapSize(P) || !CurrentMap.ValidPoint(P)) return false;

            List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, P, 0);

            for (int I = 0; I < TempList.Count; I++)
                if (TempList[I].Blocking)
                    return false;

            CurrentMap.MapCells[Location.X, Location.Y].RemoveObject(this);
                       
            Direction = D;
            Location = P;

            if (CheckMovement()) return true;

            MoveTime = Main.Time + MoveSpeed;
            CurrentMap.MapCells[Location.X, Location.Y].AddObject(this);

            Movement(new S.ObjectWalk { ObjectID = ObjectID, Location = Location });

            return true;
        }
        public virtual bool Run(MirDirection D)
        {
            if (!CanMove) return false;

            Point P = Functions.PointMove(Location, D, 1);
            Point P2 = Functions.PointMove(Location, D, 2);

            if (!CurrentMap.InMapSize(P) || !CurrentMap.ValidPoint(P) ||
                !CurrentMap.InMapSize(P2) || !CurrentMap.ValidPoint(P2)) return false;


            List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, P, 1);

            for (int I = 0; I < TempList.Count; I++)
                if ((TempList[I].Location == P || TempList[I].Location == P2) && TempList[I].Blocking)
                    return false;

            CurrentMap.MapCells[Location.X, Location.Y].RemoveObject(this);

            // Door Movements

            Direction = D;
            Location = P2;

            if (CheckMovement()) return true;

            MoveTime = Main.Time + MoveSpeed;

            CurrentMap.MapCells[Location.X, Location.Y].AddObject(this);

            Movement(new S.ObjectRun { ObjectID = ObjectID, Location = Location });

            return true;
        }
        public virtual bool Attack(MirDirection D)
        {
            if (!CanAttack) return false;

            Direction = D;
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction });


            Point P = Functions.PointMove(Location, D, 1);
            MapObject M = null;

            List<MapObject> TempList = CurrentMap.GetNearbyObjects(this, P, 0);

            for (int I = 0; I < TempList.Count; I++)
            {
                M = TempList[I];
                if (!M.Dead && M.IsAttackTarget(this))
                    break;
                else M = null;
            }

            Attack(M);

            AttackTime = Main.Time + AttackSpeed;

            return true;
        }
        public virtual void Struck(MapObject Attacker)
        {
            if (ExpOwner == null || ExpOwner.Dead)
            {
                ExpOwner = Attacker;
                ExpOwnerTime = Main.Time + ExpOwnerDelay;
            }
            else if (ExpOwner == Attacker)
                ExpOwnerTime = Main.Time + ExpOwnerDelay;

            LastHitter = Attacker;
            LastHitTime = Main.Time + LastHitDelay;

            HealthChanged();

            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = Attacker.ObjectID });

            if (!Dead && HP <= 0) Die();

            return;
        }
        public virtual bool Harvested(PlayerObject O)
        {
            return false;
        }

        public virtual void AddObject(MapObject O)
        {
            if (O is PlayerObject)
            {
                if (OldPlayers.Contains((PlayerObject)O)) return;
                OldPlayers.Add((PlayerObject)O);
            }
        }

        public virtual void RemoveObject(MapObject O)
        {
            if (O is PlayerObject) OldPlayers.Remove((PlayerObject)O);
            
            if (Target == O) Target = null;
            if (LastHitter == O) LastHitter = null;
            if (ExpOwner == O) ExpOwner = null;
            if (Master == O)
            {
                Die();
                Master = null;
            }

        }

        public virtual void Die()
        {
            HP = 0;
            
            HealAmount = 0;
            PotManaAmount = 0;
            PotHealthAmount = 0;
            
            DeadTime = Main.Time + DeadDelay;
            Dead = true;
            Broadcast(new S.ObjectDied { ObjectID = ObjectID });
        }

        public virtual void Movement(Packet P)
        {
            List<PlayerObject> Temp = NearByPlayers;

            PlayerObject O;
            for (int I = 0; I < OldPlayers.Count; I++)
            {
                O = OldPlayers[I];
                if (Temp.Contains(O)) O.QueuePacket(P);
                else O.RemoveObject(this);                
            }

            for (int I = 0; I < Temp.Count; I++)
            {
                O = Temp[I];
                if (OldPlayers.Contains(O)) continue;
                O.AddObject(this);
            }

            OldPlayers = Temp;
        }

        public virtual void WinExp(int Exp)
        {
            Experience += Exp;

            if (Experience >= MaxExperience)
            {
                Experience -= MaxExperience;
                LevelUp();
            }
        }
        public virtual void LevelUp()
        {
            if (Level < byte.MaxValue)
                Level++;

            RefreshAll();
            Recover(MaxHP, MaxMP);
            Broadcast(new S.ObjectLevelUp { ObjectID = ObjectID });
        }

        public virtual void RefreshAll()
        {
            if (HP > MaxHP)
                HP = MaxHP;

            if (MP > MaxMP)
                MP = MaxMP;
        }               


        public virtual void HealthChanged()
        {
            if (ShowHealth)
                Broadcast(new S.ObjectHealthChanged { ObjectID = ObjectID, HP = HP, MaxHP = MaxHP });
        }

        public virtual Packet GetMapData()
        {
            return null;
        }

        public void Broadcast(Packet P)
        {
            if (P == null) return;
            List<PlayerObject> TempList = NearByPlayers;
            for (int I = 0; I < TempList.Count; I++)
                TempList[I].QueuePacket(P);
        }


        public virtual bool DropItem(UserItem I)
        {
            ItemObject Ob = new ItemObject(I, this);

            if (!Ob.Drop(this is PlayerObject ? 1 : Settings.DropRange))
                return false;

            return true;
        }

    }
}
