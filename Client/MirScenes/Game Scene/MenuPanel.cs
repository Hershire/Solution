using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirScenes;
using Client.MirGraphics;
using Client.MirControls;

namespace Client.MirScenes.Game_Scene
{
    static class MenuPanel
    {
        public static MirImageControl Window;
        public static MirButton ExitButton, LogOutButton, HelpButton, UnkownButton1, UnkownButton2, UnkownButton3, RideButton, FishingButton,
                                FriendButton, MentorButton, RelationshipButton, GroupButton, GuildButton;

        static MenuPanel()
        {
            Window = new MirImageControl
            {
                Index = 1963,
                Parent = GameScene.Scene,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(745, 224),
                Sort = true,
                Visible = false,
            };
            ExitButton = new MirButton
            {
                HoverIndex = 1965,
                Index = 1964,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 12),
                PressedIndex = 1966
            };
            ExitButton.Click += (o, e) => { GameScene.QuitGame(); };

            LogOutButton = new MirButton
            {
                HoverIndex = 1968,
                Index = 1967,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 31),
                PressedIndex = 1969
            };
            LogOutButton.Click += (o, e) => { GameScene.LogOut(); };


            HelpButton = new MirButton
            {
                Index = 1970,
                HoverIndex = 1971,
                PressedIndex = 1972,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 50),
            };
            UnkownButton1 = new MirButton
            {
                Index = 2000,
                HoverIndex = 2001,
                PressedIndex = 2002,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 69),
            };
            UnkownButton2 = new MirButton
            {
                Index = 1997,
                HoverIndex = 1998,
                PressedIndex = 1999,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 88),
            };
            UnkownButton3 = new MirButton
            {
                Index = 1973,
                HoverIndex = 1974,
                PressedIndex = 1975,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 107),
            };
            RideButton = new MirButton
            {
                Index = 1976,
                HoverIndex = 1977,
                PressedIndex = 1978,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 126),
            };
            FishingButton = new MirButton
            {
                Index = 1979,
                HoverIndex = 1980,
                PressedIndex = 1981,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 145),
            };
            FriendButton = new MirButton
            {
                Index = 1982,
                HoverIndex = 1983,
                PressedIndex = 1984,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 164),
            };
            MentorButton = new MirButton
            {
                Index = 1985,
                HoverIndex = 1986,
                PressedIndex = 1987,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 183),
            };

            RelationshipButton = new MirButton
            {
                Index = 1988,
                HoverIndex = 1989,
                PressedIndex = 1990,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 202),
            };

            GroupButton = new MirButton
            {
                Index = 1991,
                HoverIndex = 1992,
                PressedIndex = 1993,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 221),
            };

            GuildButton = new MirButton
            { 
                Index = 1994,
                HoverIndex = 1995,
                PressedIndex = 1996,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(3, 240),
            };
            
        }
        public static void Show()
        {
            Window.Visible = true;
        }
        public static void Hide()
        {
            Window.Visible = false;
        }

    }
}
