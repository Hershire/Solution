using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Library;

namespace Client
{
    class Settings
    {
        //Graphics
        public static readonly Size ScreenSize = new Size(800, 600);
        public readonly static Point PlayerOffSet = new Point(ScreenSize.Width / Globals.CellWidth / 2, (ScreenSize.Height - 150) / Globals.CellHeight / 2);

        private const string ConfigPath = @".\Mir2Config.ini";

        public static string DataPath = @".\Data\";
        public static string MapPath = @".\Map\";
        public static string SoundPath = @".\Wav\";

        //Graphics
        public static bool FullScreen = false;
        public static bool TopMost = true;

        //Network
        public static string IPAddress = "127.0.0.1";
        public static int Port = 7000;
        public static int TimeOut = 5000;

        //Sound
        public static bool SoundOn = false;
        public static int SoundOverLap = 5;

        //Logs
        public static bool LogErrors = true;
        public static bool LogChat = true;
        public static int MaxErrorLogCount = 100;

        public static bool ShowItemNames = true;

        static Settings()
        {
            if (File.Exists(ConfigPath))
            {
                InIReader IReader = new InIReader(ConfigPath);

                //Paths
                DataPath = IReader.ReadString("Paths", "DataPath", DataPath);
                MapPath = IReader.ReadString("Paths", "MapPath", MapPath);
                SoundPath = IReader.ReadString("Paths", "SoundPath", SoundPath);
                
                //Graphics
                FullScreen = IReader.ReadBoolean("Graphics", "FullScreen", FullScreen);
                TopMost = IReader.ReadBoolean("Graphics", "AlwaysOnTop", TopMost);

                //Network
                IPAddress = IReader.ReadString("Network", "IPAddress", IPAddress);
                Port = IReader.ReadInt32("Network", "Port", Port);

                //Sound
                SoundOverLap = IReader.ReadInt32("Sound", "SoundOverLap", SoundOverLap);
                SoundOn = IReader.ReadBoolean("Sound", "SoundOn", SoundOn);

                //Logs
                LogErrors = IReader.ReadBoolean("Logs", "LogErrors", LogErrors);
                LogChat = IReader.ReadBoolean("Logs", "LogChat", LogChat);

                //Game Settings
                ShowItemNames = IReader.ReadBoolean("Game Settings", "ShowItemNames", ShowItemNames);
            }
            else
                Save();

        }
        public static void Save()
        {
            InIReader IReader = new InIReader(ConfigPath) { AutoSave = false };

            //Paths
            IReader.Write("Paths", "DataPath", DataPath);
            IReader.Write("Paths", "MapPath", MapPath);
            IReader.Write("Paths", "SoundPath", SoundPath);

            //Graphics
            IReader.Write("Graphics", "FullScreen", FullScreen);
            IReader.Write("Graphics", "AlwaysOnTop", TopMost);

            //Network
            IReader.Write("Network", "IPAddress", IPAddress);
            IReader.Write("Network", "Port", Port);

            //Sound
            IReader.Write("Sound", "SoundOverLap", SoundOverLap);
            IReader.Write("Sound", "SoundOn", SoundOn);

            //Logs
            IReader.Write("Logs", "LogErrors", LogErrors);
            IReader.Write("Logs", "LogChat", LogChat);

            //Game Settings
            IReader.Write("Game Settings", "ShowItemNames", ShowItemNames);
            
            IReader.AutoSave = true;
            IReader.Save();
        }




        public const string WarriorDescription =
            "Warriors are a class of great strength and vitality. They are not easily killed in battle and have the advantage of being able to use" +
            " a variety of heavy weapons and Armour. Therefore, Warriors favor attacks that are based on melee physical damage. They are weak in ranged" +
            " attacks, however the variety of equipment that are developed specifically for Warriors complement their weakness in ranged combat.";

        public const string WizardDescription =
            "Wizards are a class of low strength and stamina, but have the ability to use powerful spells. Their offensive spells are very effective, but" +
            " because it takes time to cast these spells, they're likely to leave themselves open for enemy's attacks. Therefore, the phyiscally weak wizards" +
            " must aim to attack their enemies from a safe distance.";

        public const string TaoistDescription =
            "Taoists are well disciplined in the study of Astronomy, Medicine, and others aside from Mu-Gong. Rather then directly engaging the enemies, their" +
            " specialty lies in assisting their allies with support. Taoists can summon powerful creatures and have a high resistance to magic, and is a class" +
            " with well balanced offensive and defensive abilities.";

        public const string AssassinDescription =
            "Assassins are members of a secret organization and their history is relatively unknown. They're capable of hiding themselves and performing attacks" +
            " while being unseen by others, which naturally makes them excellend at making fast kills. It is necessary for them to avoid being in battles with" +
            " multiple enemies due to their weak vitality and strength.";
    }

}
