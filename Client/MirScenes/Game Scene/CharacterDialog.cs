using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirControls;
using Client.MirObjects;
using Client.MirGraphics;
using System.Drawing;
using Client.MirSound;
using Library;
using Microsoft.DirectX.Direct3D;

namespace Client.MirScenes.Game_Scene
{
    static class CharacterDialog
    {
        public static MirImageControl Window, CharacterPage, StatusPage, StatePage, SkillPage, ClassImage;
        public static MirLabel NameLabel, GuildLabel, LoverLabel;
        public static MirButton CloseButton, CharacterButton, StatusButton, StateButton, SkillButton;
        public static MirItemCell
            WeaponCell, ArmorCell, HelmetCell, TorchCell,
            NecklaceCell, BraceletLCell, BraceletRCell, RingLCell, RingRCell,
            AmuletCell, BeltCell, BootsCell, StoneCell;

        //Stats
        public static MirLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        //State
        public static MirLabel HeadingLabel, StatLabel;

        static CharacterDialog()
        {
            Window = new MirImageControl
            {
                Index = 504,
                Library = Libraries.Title,
                Location = new Point(536, 0),
                Parent = GameScene.Scene,
                Movable = true,
                Sort = true,
                Visible = false,
            };
            Window.VisibleChanged += new EventHandler(Window_VisibleChanged);

            CharacterPage = new MirImageControl
            {
                Index = 345,
                Parent = Window,
                Library = Libraries.Prguse,
                Location = new Point(8, 90),
            };
            CharacterPage.AfterDraw += new EventHandler(CharacterPage_AfterDraw);

            StatusPage = new MirImageControl
            {
                Index = 506,
                Parent = Window,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false,
            };
            StatusPage.BeforeDraw += new EventHandler(StatusPage_BeforeDraw);

            StatePage = new MirImageControl
            {
                Index = 507,
                Parent = Window,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };
            StatePage.BeforeDraw += new EventHandler(StatePage_BeforeDraw);

            SkillPage = new MirImageControl
            {
                Index = 508,
                Parent = Window,
                Library = Libraries.Title,
                Location = new Point(8, 90),
                Visible = false
            };


            CharacterButton = new MirButton
            {
                Index = 500,
                Library = Libraries.Title,
                Location = new Point(8, 70),
                Parent = Window,
                PressedIndex = 500,
                Size = new Size(64, 20),
                Sound = SoundList.ClickA,
            };
            CharacterButton.Click += new EventHandler(CharacterButton_Click);

            StatusButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(70, 70),
                Parent = Window,
                PressedIndex = 501,
                Size = new Size(64, 20),
                Sound = SoundList.ClickA
            };
            StatusButton.Click += new EventHandler(StatusButton_Click);

            StateButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(132, 70),
                Parent = Window,
                PressedIndex = 502,
                Size = new Size(64, 20),
                Sound = SoundList.ClickA
            };
            StateButton.Click += new EventHandler(StateButton_Click);

            SkillButton = new MirButton
            {
                Library = Libraries.Title,
                Location = new Point(194, 70),
                Parent = Window,
                PressedIndex = 503,
                Size = new Size(64, 20),
                Sound = SoundList.ClickA
            };
            SkillButton.Click += new EventHandler(SkillButton_Click);

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(241, 3),
                Library = Libraries.Prguse2,
                Parent = Window,
                PressedIndex = 362,
                Sound = SoundList.ClickA,
            };
            CloseButton.Click += new EventHandler(CloseButton_Click);

            NameLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.VerticalCenter | DrawTextFormat.Center,
                Parent = Window,
                Location = new Point(50, 12),
                Size = new Size(190, 20),
                NotControl = true,
            };
            ClassImage = new MirImageControl
            {
                Index = 100,
                Library = Libraries.Prguse,
                Location = new Point(15, 33),
                Parent = Window,
                NotControl = true,
            };

            WeaponCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Weapon,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(125, 7),
            };

            ArmorCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Armour,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(164, 7),
            };

            HelmetCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Helmet,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(203, 7),
            };


            TorchCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Torch,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(203, 134),
            };

            NecklaceCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Necklace,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(203, 98),
            };

            BraceletLCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.BraceletL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 170),
            };
            BraceletRCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.BraceletR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(203, 170),
            };
            RingLCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.RingL,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 206),
            };
            RingRCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.RingR,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(203, 206),
            };

            AmuletCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Amulet,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(8, 241),
            };

            BootsCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Boots,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(47, 241),
            };
            BeltCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Belt,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(86, 241),
            };

            StoneCell = new MirItemCell
            {
                ItemSlot = (int)MirEquipmentSlot.Stone,
                GridType = MirGridType.Equipment,
                Parent = CharacterPage,
                Location = new Point(125, 241),
            };


            ACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 62),
                NotControl = true,
                Text = "0-0"
            };

            MACLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 89),
                NotControl = true,
                Text = "0-0"
            };

            DCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 115),
                NotControl = true,
                Text = "0-0"
            };

            MCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 143),
                NotControl = true,
                Text = "0-0"
            };
            SCLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 170),
                NotControl = true,
                Text = "0-0"
            };
            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 196),
                NotControl = true,
                Text = "0/0"
            };
            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatusPage,
                Location = new Point(105, 222),
                NotControl = true,
                Text = "0/0"
            };

            HeadingLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(10, 59),
                NotControl = true,
                Text = "Experience\nBag Weight\nBody Weight\nHand Weight\nAccuracy\nAgility\nMagic Resist\nPoison Resist\nHealth Regen\nMana Regen\nLuck\nHoly"
            };
            StatLabel = new MirLabel
            {
                AutoSize = true,
                Parent = StatePage,
                Location = new Point(120, 59),
                NotControl = true,
            };
        }

        static void StatusPage_BeforeDraw(object sender, EventArgs e)
        {
            ACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinAC, MapObject.User.MaxAC);
            MACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMAC, MapObject.User.MaxMAC);
            DCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinDC, MapObject.User.MaxDC);
            MCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMC, MapObject.User.MaxMC);
            SCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinSC, MapObject.User.MaxSC);
            HealthLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentHP, MapObject.User.MaxHP);
            ManaLabel.Text = string.Format("{0}/{1}", MapObject.User.CurrentMP, MapObject.User.MaxMP);
        }

        static void StatePage_BeforeDraw(object sender, EventArgs e)
        {
            string Text = string.Format("{0:#0.##%}\n{1}/{2}\n{3}/{4}\n{5}/{6}\n{7}\n{8}\n+{9}\n+{10}\n+{11}\n+{12}\n{13}\n{14}",
                MapObject.User.CurrentExperience / (double)MapObject.User.MaxExperience,
                MapObject.User.CurrentBagWeight, MapObject.User.MaxBagWeight,
                MapObject.User.CurrentBodyWeight, MapObject.User.MaxBodyWeight,
                MapObject.User.CurrentHandWeight, MapObject.User.MaxHandWeight,
                MapObject.User.Accuracy, MapObject.User.Agility,
                MapObject.User.MagicResist, MapObject.User.PoisonResist,
                MapObject.User.HealthRegen, MapObject.User.ManaRegen,
                MapObject.User.Luck, MapObject.User.Holy);

            StatLabel.Text = Text;
        }

        static void CharacterPage_AfterDraw(object sender, EventArgs e)
        {
            if (Libraries.StateItems != null)
            {
                if (ArmorCell.Item != null)
                    Libraries.StateItems.Draw(ArmorCell.Item.Info.Image, Window.DisplayLocation, Color.White, 1F, true);

                if (WeaponCell.Item != null)
                    Libraries.StateItems.Draw(WeaponCell.Item.Info.Image, Window.DisplayLocation, Color.White, 1F, true);

                if (HelmetCell.Item != null)
                    Libraries.StateItems.Draw(HelmetCell.Item.Info.Image, Window.DisplayLocation, Color.White, 1F, true);
                else
                    Libraries.Prguse.Draw(440 + PlayerObject.User.HairType + (PlayerObject.User.Gender == MirGender.Male ? 0 : 40), Window.DisplayLocation, Color.White, 1F, true);
            }
        }

        static void Window_VisibleChanged(object sender, EventArgs e)
        {
            int OffSet = MapObject.User.Gender == Library.MirGender.Male ? 0 : 1;

            Window.Index = 504 + OffSet;
            CharacterPage.Index = 345 + OffSet;

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100 + OffSet * 5;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101 + OffSet * 5;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102 + OffSet * 5;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103 + OffSet * 5;
                    break;
            }

            NameLabel.Text = MapObject.User.Name;

        }

        public static void SkillButton_Click(object sender, EventArgs e)
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = true;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = 503;
        }

        public static void StateButton_Click(object sender, EventArgs e)
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = 502;
            SkillButton.Index = -1;
        }

        public static void StatusButton_Click(object sender, EventArgs e)
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = 501;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        public static void CharacterButton_Click(object sender, EventArgs e)
        {
            CharacterPage.Visible = true;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = 500;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = -1;
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
