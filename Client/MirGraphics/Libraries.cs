using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MirGraphics
{
    static class Libraries
    {
        //Interface
        public static ImageLibrary
            ChrSel = new ImageLibrary("ChrSel"),
            Prguse = new ImageLibrary("Prguse"),
            Prguse2 = new ImageLibrary("Prguse2"),
            Title = new ImageLibrary("Title"),
            MMap = new ImageLibrary("MMap");

        //Items
        public static ImageLibrary
            Items = new ImageLibrary("Items"),
            StateItems = new ImageLibrary("StateItem"),
            DNItems = new ImageLibrary("DNItems");

        //Common
        public static ImageLibrary
            Hum = new ImageLibrary("Hum"),
            Hair = new ImageLibrary("Hair"),
            Weapon = new ImageLibrary("Weapon"),
            Weapon_Killer_Right = new ImageLibrary("Weapon_Killer_Right"),
            Weapon_Killer_Left = new ImageLibrary("Weapon_Killer_Left");

        public static ImageLibrary
            Hum_Killer = new ImageLibrary("Hum_Killer"),
            Hair_Killer = new ImageLibrary("Hair_Killer");

        //Map
        public static ImageLibrary
            Tiles = new ImageLibrary("Tiles"),
            SmallTiles = new ImageLibrary("SmTiles");

        public static ImageLibrary[] Objects = {
            new ImageLibrary("Objects"),
            new ImageLibrary("Objects2"),
            new ImageLibrary("Objects3"),
            new ImageLibrary("Objects4"),
            new ImageLibrary("Objects5"),
            new ImageLibrary("Objects6"),
            new ImageLibrary("Objects7"),
            new ImageLibrary("Objects8"),
            new ImageLibrary("Objects9"),
            new ImageLibrary("Objects10"),
            new ImageLibrary("Objects11"),
            new ImageLibrary("Objects12"),
            new ImageLibrary("Objects13"),
            new ImageLibrary("Objects14"),
            new ImageLibrary("Objects15"),
            new ImageLibrary("Objects16"),
            new ImageLibrary("Objects17"),
            new ImageLibrary("Objects18"),
            new ImageLibrary("Objects19")
        };

        public static ImageLibrary[] Monsters =   {
            new ImageLibrary("Mon1"),
            new ImageLibrary("Mon2"),
            new ImageLibrary("Mon3"),
            new ImageLibrary("Mon4"),
            new ImageLibrary("Mon5"),
            new ImageLibrary("Mon6"),
            new ImageLibrary("Mon7"),
            new ImageLibrary("Mon8"),
            new ImageLibrary("Mon9"),
            new ImageLibrary("Mon10"),
            new ImageLibrary("Mon11"),
            new ImageLibrary("Mon12"),
            new ImageLibrary("Mon13"),
            new ImageLibrary("Mon14"),
            new ImageLibrary("Mon15"),
            new ImageLibrary("Mon16"),
            new ImageLibrary("Mon17"),
            new ImageLibrary("Mon18"),
            new ImageLibrary("Mon19"),
            new ImageLibrary("Mon20"),
            new ImageLibrary("Mon21"),
            new ImageLibrary("Mon22"),
            new ImageLibrary("Mon23"),
            new ImageLibrary("Mon24"),
            new ImageLibrary("Mon25"),
            new ImageLibrary("Mon26"),
            new ImageLibrary("Mon27"),
            new ImageLibrary("Mon28"),
            new ImageLibrary("Mon29"),
            new ImageLibrary("Mon30"),
            new ImageLibrary("Mon31"),
            new ImageLibrary("Mon32"),
            new ImageLibrary("Mon33"),
            new ImageLibrary("Mon34"),
            new ImageLibrary("Mon35"),
            new ImageLibrary("Mon36"),
            new ImageLibrary("Mon37")
        };


    }
}
