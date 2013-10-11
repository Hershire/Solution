using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Client.MirSound;
using System.Drawing;
using Library;
using Client.MirControls;
using Client.MirObjects;

namespace Client.MirScenes.Game_Scene
{
    class InventoryDialog
    {
        public static MirImageControl Window, WeightBar;
        public static MirButton CloseButton;
        public static MirItemCell[] Grid;
        public static MirLabel GoldLabel, SpaceLabel;

        static InventoryDialog()
        {
            Window = new MirImageControl
            {
                Index = 196,
                Library = Libraries.Title,
                Parent = GameScene.Scene,
                Movable = true,
                Sort = true,
                Visible = false
            };
            Window.BeforeDraw += Window_BeforeDraw;

            WeightBar = new MirImageControl
            {
                Index = 24,
                Library = Libraries.Prguse,
                Location = new Point(182, 217),
                Parent = Window,
                DrawImage = false,
                NotControl = true,
            };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(289, 3),
                Library = Libraries.Prguse2,
                Parent = Window,
                PressedIndex = 362,
                Sound = SoundList.ClickA,
            };
            CloseButton.Click += new EventHandler(CloseButton_Click);

            GoldLabel = new MirLabel
            {
                Parent = Window,
                Font = new Font("Microsoft Sans Serif", 8F),
                Location = new Point(40, 212),
                Size = new Size(111, 14),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (MirItemCell.SelectedCell == null)
                    MirItemCell.PickedUpGold = !MirItemCell.PickedUpGold && MapObject.User.Gold > 0;
            };

            SpaceLabel = new MirLabel
            {
                Parent = Window,
                Font = new Font("Microsoft Sans Serif", 8F),
                Location = new Point(268, 212),
                Size = new Size(26, 14)
            };

            Grid = new MirItemCell[8 * 5];

            for (int X = 0; X < 8; X++)
            {
                for (int Y = 0; Y < 5; Y++)
                {
                    Grid[8 * Y + X] = new MirItemCell
                    {
                        ItemSlot = 8 * Y + X,
                        BorderColor = Color.Lime,
                        GridType = MirGridType.Inventory,
                        Library = Libraries.Items,
                        Parent = Window,
                        Location = new Point(X * 36 + 9 + X, Y * 32 + 37 + Y),
                    };
                }
            }
        }

        static void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            SpaceLabel.Text = MapObject.User.Inventory.Count(Ob => Ob == null).ToString();
            GoldLabel.Text = MapObject.User.Gold.ToString("###,###,##0");
        }

        static void Window_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library != null)
            {
                double Percent = MapObject.User.CurrentBagWeight / (double)MapObject.User.MaxBagWeight;
                if (Percent > 1) Percent = 1;
                if (Percent > 0)
                {
                    Rectangle Section = new Rectangle();
                    Section.Size = new Size((int)((WeightBar.Size.Width - 3) * Percent), WeightBar.Size.Height);

                    WeightBar.Library.Draw(WeightBar.Index, Section, WeightBar.DisplayLocation, Color.White);
                }
            }
        }

        static void CloseButton_Click(object sender, EventArgs e)
        {
            Hide();
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
