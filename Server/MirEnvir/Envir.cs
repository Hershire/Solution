using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Server.MirDatabase;
using Library;

namespace Server.MirEnvir
{
    static class Envir
    {
        private static long _ObjectID;
        public static long ObjectID
        {
            get { return Interlocked.Increment(ref _ObjectID); }
        }

        public static long StartTime;
        
        public static Random Rand;
        public static List<Map> MapList;
        public static Thread EnvirThread;

        public static LightSetting CurrentLights;


        static Envir()
        {
            Rand = new Random();
            MapList = new List<Map>();

        }

        public static void Start()
        {
            try
            {
                StartTime = Main.Time;
                Main.EnqueueMessage("Starting Enviroment.");

                LoadMaps();
                
                //Main.EnqueueMessage(string.Format("{0}/{1}", MapList.Count, MirDB.MapInfoList.Count));

                EnvirThread = new Thread(EnvirLoop);
                EnvirThread.IsBackground = true;
                EnvirThread.Start();

                Main.EnqueueMessage("Enviroment Started successfully.");
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
            }
        }
        
        private static void EnvirLoop()
        {
            while (true)
            {
                Main.UpdateTime();

                for (int I = 0; I < MapList.Count; I++)                
                    MapList[I].Process();

                int Hours = (Main.Now.Hour * 2) % 24;
                if (Hours == 6 || Hours == 7)
                    CurrentLights = LightSetting.Dawn;
                else if (Hours >= 8 && Hours <= 15)
                    CurrentLights = LightSetting.Day;
                else if (Hours == 16 || Hours == 17)
                    CurrentLights = LightSetting.Evening;
                else
                    CurrentLights = LightSetting.Night;



                Thread.Sleep(1);
            }

        }

        private static void LoadMaps()
        {
            
            Main.EnqueueMessage(string.Format("Loading Maps {0}.", MirDB.MapInfoList.Count));

            Map TempMap;

            for (int I = 0; I < MirDB.MapInfoList.Count; I++)
            {
                TempMap = new Map(MirDB.MapInfoList[I]);
                if (TempMap.LoadMap())
                    MapList.Add(TempMap);
            }
            
            Main.EnqueueMessage(string.Format("{0} Maps Loaded.", MapList.Count));            
        }
    }
}
