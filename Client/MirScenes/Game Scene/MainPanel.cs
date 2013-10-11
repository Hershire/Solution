using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Client.MirControls;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using Client.MirGraphics;
using Client.MirObjects;
using Client.MirSound;

namespace Client.MirScenes.Game_Scene
{
    static class MainPanel
    {
        public static MirImageControl Window, ExperienceBar, WeightBar;
        public static MirControl HealthOrb;
        public static MirLabel HealthLabel, ManaLabel, LevelLabel, CharacterName, ExperienceLabel, GoldLabel, SpaceLabel;
        public static MirButton MenuButton, InventoryButton, CharacterButton, SkillButton, QuestButton, OptionButton;

        public static void Create()
        {
            Window = new MirImageControl
            {
                Index = 1,
                Library = Libraries.Prguse,
                Parent = GameScene.Scene,
                Location = new Point(0, 450),
                PixelDetect = true,
                Visible = true
            };
            Window.BeforeDraw += new EventHandler(Window_BeforeDraw);

            HealthOrb = new MirControl
            {
                Parent = Window,
                Location = new Point(0, 30),
                NotControl = true
            };
            HealthOrb.BeforeDraw += new EventHandler(HealthOrb_BeforeDraw);

            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Parent = HealthOrb,
                Location = new Point(0, 32),
                Text = string.Format("HP {0}/{1}", 0, 0),
            };
            HealthLabel.SizeChanged += new EventHandler(Label_SizeChanged);
            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Parent = HealthOrb,
                Location = new Point(0, 50),
                Text = string.Format("HP {0}/{1}", 0, 0),
            };
            ManaLabel.SizeChanged += new EventHandler(Label_SizeChanged);

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 8F),
                Parent = Window,
                Location = new Point(5, 108),
            };
            CharacterName = new MirLabel
            {
                DrawFormat = DrawTextFormat.Center | DrawTextFormat.VerticalCenter,
                Parent = Window,
                Location = new Point(6, 122),
                Size = new Size(90, 16)
            };


            ExperienceBar = new MirImageControl
            {
                Index = 7,
                Library = Libraries.Prguse,
                Location = new Point(9, 143),
                Parent = Window,
                DrawImage = false,
                NotControl = true,
            };
            ExperienceBar.BeforeDraw += new EventHandler(ExperienceBar_BeforeDraw);

            ExperienceLabel = new MirLabel
            {
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 8F),
                Parent = ExperienceBar,
                NotControl = true,
            };

            GoldLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.VerticalCenter,
                Parent = Window,
                Font = new Font("Microsoft Sans Serif", 8F),
                Location = new Point(695, 120),
                Size = new Size(99, 11),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (MirItemCell.SelectedCell == null)
                    MirItemCell.PickedUpGold = !MirItemCell.PickedUpGold && MapObject.User.Gold > 0;
            };

            

            WeightBar = new MirImageControl
            {
                Index = 76,
                Library = Libraries.Prguse,
                Location = new Point(695, 103),
                Parent = Window,
                DrawImage = false,
                NotControl = true,
            };
            WeightBar.BeforeDraw += new EventHandler(WeightBar_BeforeDraw);

            SpaceLabel = new MirLabel
            {
                Parent = Window,
                Font = new Font("Microsoft Sans Serif", 8F),
                Location = new Point(770, 101),
                Size = new Size(26, 14)
            };


            InventoryButton = new MirButton
            {
                HoverIndex = 1904,
                Index = 1903,
                Library = Libraries.Prguse,
                Location = new Point(704, 76),
                Parent = Window,
                PressedIndex = 1905,
                Sound = SoundList.ClickA,
            };
            InventoryButton.Click += new EventHandler(InventoryButton_Click);

            CharacterButton = new MirButton
            {
                HoverIndex = 1901,
                Index = 1900,
                Library = Libraries.Prguse,
                Location = new Point(681, 76),
                Parent = Window,
                PressedIndex = 1902,
            };
            CharacterButton.Click += new EventHandler(CharacterButton_Click);
            
            SkillButton = new MirButton
            {
                HoverIndex = 1907,
                Index = 1906,
                Library = Libraries.Prguse,
                Location = new Point(727, 76),
                Parent = Window,
                PressedIndex = 1908,
                Sound = SoundList.ClickA,
            };
            SkillButton.Click += new EventHandler(SkillButton_Click);

            QuestButton = new MirButton
            {
                HoverIndex = 1910,
                Index = 1909,
                Library = Libraries.Prguse,
                Location = new Point(750, 76),
                Parent = Window,
                PressedIndex = 1911,
                Sound = SoundList.ClickA,
            };
            QuestButton.Click += new EventHandler(QuestButton_Click);

            OptionButton = new MirButton
            {
                HoverIndex = 1913,
                Index = 1912,
                Library = Libraries.Prguse,
                Location = new Point(773, 76),
                Parent = Window,
                PressedIndex = 1914,
            };
            OptionButton.Click += new EventHandler(OptionButton_Click);

            MenuButton = new MirButton
            {
                HoverIndex = 1961,
                Index = 1960,
                Library = Libraries.Prguse,
                Location = new Point(745, 35),
                Parent = Window,
                PressedIndex = 1962,
                Sound = SoundList.ClickC,
            };
            MenuButton.Click += new EventHandler(MenuButton_Click);
        }

        static void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library != null)
            {
                double Percent = MapObject.User.CurrentBagWeight / (double)MapObject.User.MaxBagWeight;
                if (Percent > 1) Percent = 1;
                if (Percent > 0)
                {
                    Rectangle Section = new Rectangle();
                    Section.Size = new Size((int)((WeightBar.Size.Width - 2) * Percent), WeightBar.Size.Height);

                    WeightBar.Library.Draw(WeightBar.Index, Section, WeightBar.DisplayLocation, Color.White);
                }
            }
        }

        static void Window_BeforeDraw(object sender, EventArgs e)
        {
            HealthLabel.Text = string.Format("HP {0}/{1}", MapObject.User.CurrentHP, MapObject.User.MaxHP);
            ManaLabel.Text = string.Format("MP {0}/{1}", MapObject.User.CurrentMP, MapObject.User.MaxMP);
            LevelLabel.Text = MapObject.User.Level.ToString();
            ExperienceLabel.Text = string.Format("{0:#0.##%}", MapObject.User.CurrentExperience / (double)MapObject.User.MaxExperience);
            ExperienceLabel.Location = new Point(390 - (MainPanel.ExperienceLabel.Size.Width / 2), -6);
            GoldLabel.Text = MapObject.User.Gold.ToString("###,###,##0");
            CharacterName.Text = MapObject.User.Name;
            SpaceLabel.Text = MapObject.User.Inventory.Count(Ob => Ob == null).ToString();
        }

        static void CharacterButton_Click(object sender, EventArgs e)
        {
            if (CharacterDialog.Window.Visible && CharacterDialog.CharacterPage.Visible)
                CharacterDialog.Hide();
            else
            {
                CharacterDialog.Show();
                CharacterDialog.CharacterButton_Click(null, EventArgs.Empty);
            }
        }
        static void SkillButton_Click(object sender, EventArgs e)
        {
            if (CharacterDialog.Window.Visible && CharacterDialog.SkillPage.Visible)
                CharacterDialog.Hide();
            else
            {
                CharacterDialog.Show();
                CharacterDialog.SkillButton_Click(null, EventArgs.Empty);
            }
        }

        static void MenuButton_Click(object sender, EventArgs e)
        {
            if (!MenuPanel.Window.Visible)
                MenuPanel.Show();
            else
                MenuPanel.Hide();
        }
        static void QuestButton_Click(object sender, EventArgs e)
        {
            /*if (!QuestDialog.Window.Visible)
                QuestDialog.Show();
            else
                QuestDialog.Hide();*/
        }
        static void OptionButton_Click(object sender, EventArgs e)
        {
            /*if (!OptionDialog.Window.Visible)
                OptionDialog.Show();
            else
                OptionDialog.Hide();*/
        }

        static void ExperienceBar_BeforeDraw(object sender, EventArgs e)
        {
            if (ExperienceBar.Library != null)
            {
                double Percent = MapObject.User.CurrentExperience / (double)MapObject.User.MaxExperience;
                if (Percent > 1) Percent = 1;
                if (Percent > 0)
                {
                    Rectangle Section = new Rectangle();
                    Section.Size = new Size((int)((ExperienceBar.Size.Width - 3) * Percent), ExperienceBar.Size.Height);

                    ExperienceBar.Library.Draw(ExperienceBar.Index, Section, ExperienceBar.DisplayLocation, Color.White);
                }
            }
        }

        static void InventoryButton_Click(object sender, EventArgs e)
        {
            if (InventoryDialog.Window.Visible)
                InventoryDialog.Hide();
            else
                InventoryDialog.Show();
        }

        static void Label_SizeChanged(object sender, EventArgs e)
        {
            MirLabel L = sender as MirLabel;

            if (L == null) return;

            L.Location = new Point(50 - (L.Size.Width / 2), L.Location.Y);
        }
        static void HealthOrb_BeforeDraw(object sender, EventArgs e)
        {
            if (Libraries.Prguse != null)
            {
                int Height;
                if (MapObject.User.CurrentHP != MapObject.User.MaxHP)
                    Height = (int)(80 * MapObject.User.CurrentHP / (float)MapObject.User.MaxHP);
                else
                    Height = 80;

                if (Height < 0) Height = 0;
                if (Height > 80) Height = 80;
                Rectangle R = new Rectangle(0, 80 - Height, 50, Height);
                Libraries.Prguse.Draw(4, R, new Point(0, HealthOrb.DisplayLocation.Y + 80 - Height), Color.White);

                if (MapObject.User.CurrentMP != MapObject.User.MaxMP)
                    Height = (int)(80 * MapObject.User.CurrentMP / (float)MapObject.User.MaxMP);
                else
                    Height = 80;

                if (Height < 0) Height = 0;
                if (Height > 80) Height = 80;
                R = new Rectangle(51, 80 - Height, 50, Height);

                Libraries.Prguse.Draw(4, R, new Point(51, HealthOrb.DisplayLocation.Y + 80 - Height), Color.White);
            }
        }
    }
}
