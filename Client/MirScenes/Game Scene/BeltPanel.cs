using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Client.MirControls;
using Client.MirObjects;
using Client.MirSound;
using System.Drawing;
using Library;

namespace Client.MirScenes.Game_Scene
{
    static class BeltPanel
    {
        public static MirImageControl Window;
        public static MirButton CloseButton, RotateButton;
        public static MirItemCell[] Grid;

        public static void Create()
        {
            Window = new MirImageControl
            {
                Index = 1932,
                Library = Libraries.Prguse,
                Parent = GameScene.Scene,
                Movable = true,
                Sort = true,
                Visible = true,
                Location = new Point(230, 450)
            };
            Window.BeforeDraw += new EventHandler(Window_BeforeDraw);

            RotateButton = new MirButton
            {
                HoverIndex = 1927,
                Index = 1926,
                Location = new Point(222, 3),
                Library = Libraries.Prguse,
                Parent = Window,
                PressedIndex = 1928,
                Sound = SoundList.ClickA,
            };
            CloseButton = new MirButton
            {
                HoverIndex = 1924,
                Index = 1923,
                Location = new Point(222, 19),
                Library = Libraries.Prguse,
                Parent = Window,
                PressedIndex = 1925,
                Sound = SoundList.ClickA,
            };

            Grid = new MirItemCell[6];

            for (int X = 0; X < 6; X++)
            {
                Grid[X] = new MirItemCell
                {
                    ItemSlot = 40 + X, // 
                    Size = new Size(32, 32),
                    BorderColor = Color.Lime,
                    GridType = MirGridType.Inventory,
                    Library = Libraries.Items,
                    Parent = Window,
                    Location = new Point(X * 35 + 12, 3),
                };
            }

        }

        static void Window_BeforeDraw(object sender, EventArgs e)
        {
            //if Transparent return

            if (Libraries.Prguse != null)
                Libraries.Prguse.Draw(1933, Window.DisplayLocation, Color.White, 0.5F);
        }

        public static void Hide()
        {
            Window.Visible = false;
        }
        public static void Show()
        {
            Window.Visible = true;
        }
        public static void Flip()
        {

        }
    }
}
