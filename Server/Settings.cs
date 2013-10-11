using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Library;

namespace Server
{
    class Settings
    {
        const string ConfigPath = @".\Config.ini";
        const string ExpListPath = @".\Exps.ini";

        //Paths
        public static string MapPath = @".\Maps\",
                             LogPath = @".\Logs\",
                             ClientPath = @".\Mir 2.Exe";

        //Database
        internal static string SQLServer = @"JAMIE-PC\SQLEXPRESS",
                               SQLDatabase = @"Mir2DB",
                               SQLUserID = "sa",
                               SQLPassword = "SQLPass";

        //Database
        public static TimeSpan DBSaveTimer = TimeSpan.FromMinutes(15);

        //NetWork
        public static string IPAddress = "127.0.0.1";
        public static int Port = 7000;
        public static long TimeOut = 10000;
        public static int MaxUser = 50;
        public static double ReLogTime = 10;



        public static List<int> ExperienceList = new List<int>();

        public static bool CheckVersion;
        public static byte[] VersionHash;

        //Permission
        public static bool
            AllowNewAccount = true,
            AllowChangePassword = true,
            AllowLogin = true,
            AllowNewCharacter = true,
            AllowDeleteCharacter = true,
            AllowStartGame = true;

        /// 
        /// Game Settings
        ///

        //Drops
        public static int
            DropRange = 4,
            DropStackSize = 5,
            ItemTimeOut = 60;

        //Rates
        public static float ExpRate = 1, DropRate = 1;

        //Starting
        public static byte StartLevel = 1;
        public static long StartGold = 0;

        static Settings()
        {
            Main.EnqueueMessage("Loading Config file.");

            if (File.Exists(ConfigPath))
            {
                InIReader IReader = new InIReader(ConfigPath);

                //Path
                MapPath = IReader.ReadString("Path", "MapPath", MapPath);
                LogPath = IReader.ReadString("Path", "LogPath", LogPath);
                ClientPath = IReader.ReadString("Path", "ClientPath", ClientPath);

                //Database
                SQLServer = IReader.ReadString("Database", "SQLServer", SQLServer);
                SQLDatabase = IReader.ReadString("Database", "SQLDatabase", SQLDatabase);
                SQLUserID = IReader.ReadString("Database", "SQLUserID", SQLUserID);
                SQLPassword = IReader.ReadString("Database", "SQLPassword", SQLPassword);

                //Network
                IPAddress = IReader.ReadString("Network", "IPAddress", IPAddress);
                Port = IReader.ReadInt32("Network", "Port", Port);
                TimeOut = IReader.ReadInt64("Network", "TimeOut", TimeOut);
                MaxUser = IReader.ReadInt32("Network", "MaxUser", MaxUser);
                ReLogTime = IReader.ReadDouble("Network", "ReLogTime", ReLogTime);

                //Security
                CheckVersion = IReader.ReadBoolean("Security", "CheckVersion", CheckVersion);

                //Permission
                AllowNewAccount = IReader.ReadBoolean("Permission", "AllowNewAccount", AllowNewAccount);
                AllowChangePassword = IReader.ReadBoolean("Permission", "AllowChangePassword", AllowChangePassword);
                AllowLogin = IReader.ReadBoolean("Permission", "AllowLogin", AllowLogin);
                AllowNewCharacter = IReader.ReadBoolean("Permission", "AllowNewCharacter", AllowNewCharacter);
                AllowDeleteCharacter = IReader.ReadBoolean("Permission", "AllowDeleteCharacter", AllowDeleteCharacter);
                AllowStartGame = IReader.ReadBoolean("Permission", "AllowStartGame", AllowStartGame);

                //Game
                DropRange = IReader.ReadInt32("Game", "DropRange", DropRange);
                DropStackSize = IReader.ReadInt32("Game", "DropStackSize", DropStackSize);
                ItemTimeOut = IReader.ReadInt32("Game", "ItemTimeOut", ItemTimeOut);

                ExpRate = IReader.ReadSingle("Game", "ExpRate", ExpRate);
                DropRate = IReader.ReadSingle("Game", "DropRate", DropRate);

                StartLevel = IReader.ReadByte("Game", "StartLevel", StartLevel);
                StartGold = IReader.ReadInt64("Game", "StartGold", StartGold);

                if (ExpRate <= 0) ExpRate = 1;
                if (StartLevel <= 0) StartLevel = 1;

                Main.EnqueueMessage("Config file loaded.");
            }
            else
            {
                Main.EnqueueMessage("Config file not found, creating Config file.");
                Save();
            }
            LoadVersion();
            LoadExperienceList();
        }

        public static void Save()
        {
            InIReader IReader = new InIReader(ConfigPath) { AutoSave = false };

            //Path
            IReader.Write("Path", "MapPath", MapPath);
            IReader.Write("Path", "LogPath", LogPath);
            IReader.Write("Path", "ClientPath", ClientPath);

            //Database
            IReader.Write("Database", "SQLServer", SQLServer);
            IReader.Write("Database", "SQLDatabase", SQLDatabase);
            IReader.Write("Database", "SQLUserID", SQLUserID);
            IReader.Write("Database", "SQLPassword", SQLPassword);

            //Network
            IReader.Write("Network", "IPAddress", IPAddress);
            IReader.Write("Network", "Port", Port);
            IReader.Write("Network", "TimeOut", TimeOut);
            IReader.Write("Network", "MaxUser", MaxUser);
            IReader.Write("Network", "ReLogTime", ReLogTime);

            //Security
            IReader.Write("Security", "CheckVersion", CheckVersion);

            //Permission
            IReader.Write("Permission", "AllowNewAccount", AllowNewAccount);
            IReader.Write("Permission", "AllowChangePassword", AllowChangePassword);
            IReader.Write("Permission", "AllowLogin", AllowLogin);
            IReader.Write("Permission", "AllowNewCharacter", AllowNewCharacter);
            IReader.Write("Permission", "AllowDeleteCharacter", AllowDeleteCharacter);
            IReader.Write("Permission", "AllowStartGame", AllowStartGame);

            //Game
            IReader.Write("Game", "DropRange", DropRange);
            IReader.Write("Game", "DropStackSize", DropStackSize);
            IReader.Write("Game", "ItemTimeOut", ItemTimeOut);

            IReader.Write("Game", "ExpRate", ExpRate);
            IReader.Write("Game", "DropRate", DropRate);

            IReader.Write("Game", "StartLevel", StartLevel);
            IReader.Write("Game", "StartGold", StartGold);
            
            IReader.AutoSave = true;
            IReader.Save();

            Main.EnqueueMessage("Config file Saved.");
        }

        public static void LoadVersion()
        {
            try
            {
                if (File.Exists(ClientPath))
                    using (FileStream FStream = new FileStream(ClientPath, FileMode.Open, FileAccess.Read))
                    using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
                        VersionHash = MD5.ComputeHash(FStream);
            }
            catch (Exception Ex)
            {
                Main.EnqueueException(Ex);
            }
        }
        public static void LoadExperienceList()
        {
            if (!File.Exists(ExpListPath))
                ExperienceList.Add(100);
            else
            {
                int Exp = 100;
                InIReader IReader = new InIReader(ExpListPath);

                for (int I = 1; I <= 50; I++)
                {
                    Exp = IReader.ReadInt32("Exp", "Level" + I.ToString(), Exp);
                    ExperienceList.Add(Exp);
                }

            }

        }
    }
}

