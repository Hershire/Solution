using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirControls;
using Client.MirGraphics;
using System.Drawing;
using System.Text.RegularExpressions;
using Library;
using Microsoft.DirectX.Direct3D;
using Client.MirNetwork;
using System.Windows.Forms;
using Client.MirSound;


namespace Client.MirScenes.Select_Scene
{
    static class NewCharacterDialog
    {
        static MirImageControl NewCharacterWindow, NewCharacterTitle;
        static MirAnimatedControl CharacterDisplay;
        public static MirButton ConfirmButton, CancelButton, WarriorButton, WizardButton, TaoistButton, AssassinButton, MaleButton, FemaleButton;
        public static MirTextBox CharacterNameTextBox;
        private static Regex Reg;
        public static MirGender NewGender;
        public static MirClass NewClass;
        public static MirLabel Description;

        static NewCharacterDialog()
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength + "}$");
            NewCharacterWindow = new MirImageControl
            {
                Index = 73,
                Library = Libraries.Prguse,
                Parent = SelectScene.Background,
                Visible = false,
            };
            NewCharacterWindow.Location = new Point((Settings.ScreenSize.Width - NewCharacterWindow.Size.Width) / 2,
                                             (Settings.ScreenSize.Height - NewCharacterWindow.Size.Height) / 2);

            NewCharacterTitle = new MirImageControl
            {
                Index = 20,
                Library = Libraries.Title,
                Location = new Point(193, 7),
                Parent = NewCharacterWindow,
            };

            CancelButton = new MirButton
            {
                HoverIndex = 364,
                Index = 363,
                Library = Libraries.Title,
                Location = new Point(425, 425),
                Parent = NewCharacterWindow,
                PressedIndex = 365
            };
            CancelButton.Click += CancelButton_Click;

            ConfirmButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 361,
                Index = 360,
                Library = Libraries.Title,
                Location = new Point(160, 425),
                Parent = NewCharacterWindow,
                PressedIndex = 362,
            };
            ConfirmButton.Click += new EventHandler(ConfirmButton_Click);

            CharacterNameTextBox = new MirTextBox
            {
                Location = new Point(357, 269),
                Parent = NewCharacterWindow,
                Size = new Size(190, 15),
                MaxLength = Globals.MaxCharacterNameLength
            };
            CharacterNameTextBox.TextBox.KeyPress += TextBox_KeyPress;
            CharacterNameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
            CharacterNameTextBox.SetFocus();

            CharacterDisplay = new MirAnimatedControl
            {
                Animated = true,
                AnimationCount = 16,
                AnimationDelay = 250,
                Index = 20,
                Library = Libraries.ChrSel,
                Location = new Point(120, 250),
                Parent = NewCharacterWindow,
                UseOffSet = true,
            };
            CharacterDisplay.AfterDraw += SelectScene.CharacterDisplay_GlowEffect;

            WarriorButton = new MirButton
            {
                HoverIndex = 2427,
                Index = 2427,
                Library = Libraries.Prguse,
                Location = new Point(355, 296),
                Parent = NewCharacterWindow,
                PressedIndex = 2428,
                Sound = SoundList.ClickA,
            };
            WarriorButton.Click += new EventHandler(WarriorButton_Click);
            WizardButton = new MirButton
            {
                HoverIndex = 2430,
                Index = 2429,
                Library = Libraries.Prguse,
                Location = new Point(405, 296),
                Parent = NewCharacterWindow,
                PressedIndex = 2431,
                Sound = SoundList.ClickA,
            };
            WizardButton.Click += new EventHandler(WizardButton_Click);
            TaoistButton = new MirButton
            {
                HoverIndex = 2433,
                Index = 2432,
                Library = Libraries.Prguse,
                Location = new Point(455, 296),
                Parent = NewCharacterWindow,
                PressedIndex = 2434,
                Sound = SoundList.ClickA,
            };
            TaoistButton.Click += new EventHandler(TaoistButton_Click);
            AssassinButton = new MirButton
            {
                HoverIndex = 2436,
                Index = 2435,
                Library = Libraries.Prguse,
                Location = new Point(505, 296),
                Parent = NewCharacterWindow,
                PressedIndex = 2437,
                Sound = SoundList.ClickA,
            };
            AssassinButton.Click += new EventHandler(AssassinButton_Click);

            MaleButton = new MirButton
            {
                HoverIndex = 2421,
                Index = 2421,
                Library = Libraries.Prguse,
                Location = new Point(355, 343),
                Parent = NewCharacterWindow,
                PressedIndex = 2422,
                Sound = SoundList.ClickA,
            };
            MaleButton.Click += new EventHandler(MaleButton_Click);
            FemaleButton = new MirButton
            {
                HoverIndex = 2424,
                Index = 2423,
                Library = Libraries.Prguse,
                Location = new Point(405, 343),
                Parent = NewCharacterWindow,
                PressedIndex = 2425,
                Sound = SoundList.ClickA,
            };
            FemaleButton.Click += new EventHandler(FemaleButton_Click);
            Description = new MirLabel
            {
                Border = true,
                BorderColor = Color.FromArgb(53, 49, 41),
                DrawFormat = DrawTextFormat.WordBreak,
                Location = new Point(279, 70),
                Parent = NewCharacterWindow,
                Size = new Size(278, 170),
                Text = Settings.WarriorDescription,
            };
        }

        private static void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender == null) return;
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;

            if (ConfirmButton.Enabled)
                ConfirmButton.InvokeMouseClick(EventArgs.Empty);
        }

        static void ConfirmButton_Click(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = false;
            CharacterNameTextBox.Enabled = false;
            OutBound.NewCharacter();
        }


        static void UpdateDisplayCharacter()
        {
            CharacterDisplay.Index = ((byte)NewClass + 1) * 20 + ((byte)NewGender * 280);
        }

        static void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public static void Show()
        {
            NewCharacterWindow.Visible = true;
        }
        public static void Hide()
        {
            NewCharacterWindow.Visible = false;
        }


        static void FemaleButton_Click(object sender, EventArgs e)
        {
            MaleButton.Index = 2420;
            FemaleButton.Index = 2424;
            NewGender = MirGender.Female;
            UpdateDisplayCharacter();
        }
        static void MaleButton_Click(object sender, EventArgs e)
        {
            MaleButton.Index = 2421;
            FemaleButton.Index = 2423;
            NewGender = MirGender.Male;
            UpdateDisplayCharacter();
        }

        static void AssassinButton_Click(object sender, EventArgs e)
        {
            WarriorButton.Index = 2426;
            WizardButton.Index = 2429;
            TaoistButton.Index = 2432;
            AssassinButton.Index = 2436;
            NewClass = MirClass.Assassin;
            Description.Text = Settings.AssassinDescription;
            UpdateDisplayCharacter();
        }
        static void TaoistButton_Click(object sender, EventArgs e)
        {
            WarriorButton.Index = 2426;
            WizardButton.Index = 2429;
            TaoistButton.Index = 2433;
            AssassinButton.Index = 2435;
            NewClass = MirClass.Taoist;
            Description.Text = Settings.TaoistDescription;
            UpdateDisplayCharacter();
        }
        static void WizardButton_Click(object sender, EventArgs e)
        {
            WarriorButton.Index = 2426;
            WizardButton.Index = 2430;
            TaoistButton.Index = 2432;
            AssassinButton.Index = 2435;
            NewClass = MirClass.Wizard;
            Description.Text = Settings.WizardDescription;
            UpdateDisplayCharacter();
        }
        static void WarriorButton_Click(object sender, EventArgs e)
        {
            WarriorButton.Index = 2427;
            WizardButton.Index = 2429;
            TaoistButton.Index = 2432;
            AssassinButton.Index = 2435;
            NewClass = MirClass.Warrior;
            Description.Text = Settings.WarriorDescription;
            UpdateDisplayCharacter();
        }

        private static void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CharacterNameTextBox.Text))
            {
                ConfirmButton.Enabled = false;
                CharacterNameTextBox.Border = false;
            }
            else if (!Reg.IsMatch(CharacterNameTextBox.Text))
            {
                ConfirmButton.Enabled = false;
                CharacterNameTextBox.Border = true;
                CharacterNameTextBox.BorderColor = Color.Red;
            }
            else
            {
                ConfirmButton.Enabled = true;
                CharacterNameTextBox.Border = true;
                CharacterNameTextBox.BorderColor = Color.Green;
            }
        }
    }
}
