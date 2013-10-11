using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Server.MirObjects;
using Server.MirDatabase;
using System.Drawing;
using System.IO;
using Library.MirNetwork;

namespace Server.MirEnvir
{
    class Map
    {
        public MapInfo MapInfo;        
        public Size MapSize;
        public MapCell[,] MapCells;


        public LightSetting CurrentLights
        {
            get { return MapInfo.LightMode == LightSetting.Normal ? Envir.CurrentLights : MapInfo.LightMode; }
        }

        private List<MapObject> Objects = new List<MapObject>();
        public List<PlayerObject> Players = new List<PlayerObject>();
        public List<MapRespawn> RespawnList = new List<MapRespawn>();

        public Map(MapInfo M)
        {
            MapInfo = M;
        }

        public bool InMapSize(Point P)
        {
            return P.X >= 0 && P.Y >= 0 && P.X < MapSize.Width && P.Y < MapSize.Height;
        }
        public bool ValidPoint(Point P)
        {
            return MapCells[P.X, P.Y].Valid;
        }
        public bool InMapSize(int X, int Y)
        {
            return X >= 0 && Y >= 0 && X < MapSize.Width && Y < MapSize.Height;
        }
        public bool ValidPoint(int X, int Y)
        {
            return MapCells[X, Y].Valid;
        }

        public void Process()
        {
            MapRespawn MR;
            for (int I = 0; I < RespawnList.Count; I++)
            {
                MR = RespawnList[I];

                int SpawnCount = MR.RI.Count - MR.Count;

                if (SpawnCount <= 0) continue;

                if (Main.Time >= MR.NextSpawn)
                {
                    for (int R = 0; R < SpawnCount; R++)
                    {
                        MonsterObject.MakeMonster(MR).TrySpawn();
                    }

                    MR.NextSpawn = Main.Time + (long)MR.RI.Delay.TotalMilliseconds;
                }

            }

            for (int I = Objects.Count - 1; I >= 0; I--)
                if (Objects[I].CanOperate)
                    Objects[I].Process();  
        }
        public bool LoadMap()
        {
            try
            {
                string FileName = Path.Combine(Settings.MapPath, MapInfo.FileName + ".map");
                if (File.Exists(FileName))
                {
                    byte[] FileBytes = File.ReadAllBytes(FileName);

                    int OffSet = 21;

                    int W = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    int Xor = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    int H = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    MapSize = new Size((short)W ^ (short)Xor, H ^ Xor);
                    MapCells = new MapCell[MapSize.Width, MapSize.Height];

                    OffSet = 54;

                    for (short X = 0; X < MapSize.Width; X++)
                        for (short Y = 0; Y < MapSize.Height; Y++)
                        {
                            if (((BitConverter.ToInt32(FileBytes, OffSet) ^ 0xAA38AA38) & 0x20000000) != 0)
                                MapCells[X, Y] = MapCell.LowWall; //Can Fire Over.

                            OffSet += 6;
                            if (((BitConverter.ToInt16(FileBytes, OffSet) ^ Xor) & 0x8000) != 0)
                                MapCells[X, Y] = MapCell.LowWall;//Can't Fire Over.

                            if (MapCells[X, Y] == null) MapCells[X, Y] = new MapCell { Attribute = CellAttribute.Walk };
                            
                            OffSet += 9;
                        }

                    for (int I = 0; I < MapInfo.RespawnList.Count; I++)
                        if (MapInfo.RespawnList[I].Monster != null)
                            RespawnList.Add(new MapRespawn { RI = MapInfo.RespawnList[I] });
                    
                    return true;
                }
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
                return false;
            }
            return false;
        }

        public bool Spawn(MapObject M)
        {
            if (!InMapSize(M.Location) || !ValidPoint(M.Location)) return false;

            Objects.Add(M);
            MapCells[M.Location.X, M.Location.Y].AddObject(M);

            if (M is PlayerObject) Players.Add(M as PlayerObject);

            return true;
        }
        public void DeSpawn(MapObject M)
        {
            Objects.Remove(M);
            MapCells[M.Location.X, M.Location.Y].RemoveObject(M);
            if (M is PlayerObject) Players.Remove(M as PlayerObject);            
        }

        public ClientMapInfo GetDetails()
        {
            return new ClientMapInfo
            {
                MapIndex = MapInfo.MapIndex,
                FileName = MapInfo.FileName,
                MapName = MapInfo.MapName,    
                MiniMap = MapInfo.MiniMap,
                Lights = CurrentLights,
            };
        }

        public List<PlayerObject> GetNearbyPlayers(MapObject M, int Range = Globals.DataRange)
        {
            List<PlayerObject> Temp = new List<PlayerObject>();

            for (int I = 0; I < Players.Count; I++)
            {
                PlayerObject P = Players[I];
                if (M != P && Functions.InRange(P.Location, M.Location, Range))
                    Temp.Add(P);
            }

            return Temp;

        }
        public List<PlayerObject> GetNearbyPlayers(MapObject M, Point Location, int Range = Globals.DataRange)
        {
            List<PlayerObject> Temp = new List<PlayerObject>();

            for (int I = 0; I < Players.Count; I++)
            {
                PlayerObject P = Players[I];
                if (M != P && Functions.InRange(P.Location, Location, Range))
                    Temp.Add(P);
            }

            return Temp;

        }
        public List<MapObject> GetNearbyObjects(MapObject M, int Range = Globals.DataRange)
        {
            return GetNearbyObjects(M, M.Location, Range);
        }
        public List<MapObject> GetNearbyObjects(MapObject M, Point Location, int Range = Globals.DataRange)
        {
            List<MapObject> Temp = new List<MapObject>();

            MapCell Cell;
            MapObject P;

            for (int Y = -Range + Location.Y; Y <= Range + Location.Y; Y++)
                for (int X = -Range + Location.X; X <= Range + Location.X; X++)
                {
                    if (!InMapSize(X, Y) || !ValidPoint(X, Y)) continue;

                    Cell = MapCells[X, Y];

                    if (Cell.Objects != null)
                    {

                        if (X == M.Location.X && Y == M.Location.Y)
                        {
                            for (int I = 0; I < Cell.Objects.Count; I++)
                            {
                                P = Cell.Objects[I];
                                if (M != P) Temp.Add(P);
                            }
                        }
                        else
                            Temp.AddRange(Cell.Objects);
                    }
                }


            return Temp;

        }
    }

    class MapRespawn
    {
        public RespawnInfo RI;
        public int Count;
        public long NextSpawn;
    }
}
