using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirControls;
using System.Windows.Forms;
using Client.MirGraphics;
using Client.MirSound;
using System.Drawing;
using Library;
using Microsoft.DirectX.Direct3D;
using Client.MirNetwork;

namespace Client.MirScenes.Select_Scene
{
    static class SelectScene
    {
        public static List<SelectCharacterInfo> CharacterList = new List<SelectCharacterInfo>();
        public static MirScene Scene;
        static MirAnimatedControl CharacterDisplay;
        public static MirButton StartGameButton;
        static MirButton NewCharacterButton, DeleteCharacterButton, /*CreditsButton,*/ ExitGame;
        public static MirImageControl Background;
        static MirLabel ServerLabel;
        public static int SelectedIndex;
        static MirImageControl Character1Button, Character2Button, Character3Button, Character4Button;
        static MirLabel Character1Name, Character1Level, Character1Class;
        static MirLabel Character2Name, Character2Level, Character2Class;
        static MirLabel Character3Name, Character3Level, Character3Class;
        static MirLabel Character4Name, Character4Level, Character4Class;
        static MirLabel LastAccessLabel, LastAccessLabelLabel;
        
        static SelectScene()
        {
            SelectedIndex = -1;

            Scene = new MirScene
            {
                Size = Settings.ScreenSize,
                Visible = false,
            };
            Scene.KeyPress += Scene_KeyPress;
            Scene.Shown += Scene_Shown;
            Scene.VisibleChanged += Scene_VisibleChanged;

            Background = new MirImageControl
            {
                Index = 64,
                Library = Libraries.Prguse,
                Parent = Scene,
            };

            ServerLabel = new MirLabel()
            {
                Location = new Point(322, 44),
                DrawFormat = DrawTextFormat.Center | DrawTextFormat.VerticalCenter,
                Parent = Background,
                Size = new Size(155, 17),
                Text = "Legend of Mir 2",
            };

            StartGameButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 341,
                Index = 340,
                Library = Libraries.Title,
                Location = new Point(110, 568),
                Parent = Background,
                PressedIndex = 342
            };
            StartGameButton.Click += new EventHandler(StartGameButton_Click);

            NewCharacterButton = new MirButton
            {
                HoverIndex = 344,
                Index = 343,
                Library = Libraries.Title,
                Location = new Point(230, 568),
                Parent = Background,
                PressedIndex = 345
            };
            NewCharacterButton.Click += NewCharacterButton_Click;

            DeleteCharacterButton = new MirButton
            {
                HoverIndex = 347,
                Index = 346,
                Library = Libraries.Title,
                Location = new Point(350, 568),
                Parent = Background,
                PressedIndex = 348
            };
            DeleteCharacterButton.Click += new EventHandler(DeleteCharacterButton_Click);
            /*
            CreditsButton = new MirButton
            {
                HoverIndex = 350,
                Index = 349,
                Library = Libraries.Title,
                Location = new Point(470, 568),
                Parent = Background,
                PressedIndex = 351
            };*/

            ExitGame = new MirButton
            {
                HoverIndex = 353,
                Index = 352,
                Library = Libraries.Title,
                Location = new Point(590, 568),
                Parent = Background,
                PressedIndex = 354
            };
            ExitGame.Click += (o, e) => SceneFunctions.QuitGame();


            CharacterDisplay = new MirAnimatedControl
            {
                Animated = true,
                AnimationCount = 16,
                AnimationDelay = 250,
                FadeIn = true,
                FadeInDelay = 75,
                FadeInRate = 0.1F,
                Index = 20,
                Library = Libraries.ChrSel,
                Location = new Point(200, 300),
                Parent = Background,
                UseOffSet = true,
                // Visible = false
            };
            CharacterDisplay.BeforeDraw += new EventHandler(CharacterDisplay_BeforeDraw);
            CharacterDisplay.AfterDraw += new EventHandler(CharacterDisplay_GlowEffect);

            Character1Button = new MirImageControl
            {
                Index = 45,
                Library = Libraries.Prguse,
                Location = new Point(447, 122),
                Parent = Background,
                Sound = SoundList.ClickA,
            };
            Character1Button.Click += Character1Button_Click;

            Character2Button = new MirImageControl
            {
                Index = 45,
                Library = Libraries.Prguse,
                Location = new Point(447, 226),
                Parent = Background,
                Sound = SoundList.ClickA,
            };
            Character2Button.Click += new EventHandler(Character2Button_Click);

            Character3Button = new MirImageControl
            {
                Index = 45,
                Library = Libraries.Prguse,
                Location = new Point(447, 330),
                Parent = Background,
                Sound = SoundList.ClickA,
            };
            Character3Button.Click += new EventHandler(Character3Button_Click);

            Character4Button = new MirImageControl
            {
                Index = 45,
                Library = Libraries.Prguse,
                Location = new Point(447, 434),
                Parent = Background,
                Sound = SoundList.ClickA,
            };
            Character4Button.Click += new EventHandler(Character4Button_Click);

            Character1Name = new MirLabel
            {
                Location = new Point(107, 9),
                Parent = Character1Button,
                NotControl = true,
                Size = new Size(170, 18),
                Visible = false
            };

            Character1Level = new MirLabel
            {
                Location = new Point(107, 28),
                Parent = Character1Button,
                NotControl = true,
                Size = new Size(30, 18),
                Visible = false
            };

            Character1Class = new MirLabel
            {
                Location = new Point(178, 28),
                Parent = Character1Button,
                NotControl = true,
                Size = new Size(100, 18),
                Visible = false
            };

            Character2Name = new MirLabel
            {
                Location = new Point(107, 9),
                Parent = Character2Button,
                NotControl = true,
                Size = new Size(170, 18),
                Visible = false
            };

            Character2Level = new MirLabel
            {
                Location = new Point(107, 28),
                Parent = Character2Button,
                NotControl = true,
                Size = new Size(30, 18),
                Visible = false
            };

            Character2Class = new MirLabel
            {
                Location = new Point(178, 28),
                Parent = Character2Button,
                NotControl = true,
                Size = new Size(100, 18),
                Visible = false
            };
            Character3Name = new MirLabel
            {
                Location = new Point(107, 9),
                Parent = Character3Button,
                NotControl = true,
                Size = new Size(170, 18),
                Visible = false
            };

            Character3Level = new MirLabel
            {
                Location = new Point(107, 28),
                Parent = Character3Button,
                NotControl = true,
                Size = new Size(30, 18),
                Visible = false
            };

            Character3Class = new MirLabel
            {
                Location = new Point(178, 28),
                Parent = Character3Button,
                NotControl = true,
                Size = new Size(100, 18),
                Visible = false
            };
            Character4Name = new MirLabel
            {
                Location = new Point(107, 9),
                Parent = Character4Button,
                NotControl = true,
                Size = new Size(170, 18),
                Visible = false
            };

            Character4Level = new MirLabel
            {
                Location = new Point(107, 28),
                Parent = Character4Button,
                NotControl = true,
                Size = new Size(30, 18),
                Visible = false
            };

            Character4Class = new MirLabel
            {
                Location = new Point(178, 28),
                Parent = Character4Button,
                NotControl = true,
                Size = new Size(100, 18),
                Visible = false
            };
            LastAccessLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.VerticalCenter,
                Location = new Point(140, 510),
                Parent = Background,
                Size = new Size(189, 21),
            };
            LastAccessLabelLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.VerticalCenter,
                Location = new Point(-80, 0),
                Parent = LastAccessLabel,
                Text = "Last Online:",
                Size = new Size(100, 21),
            };
        }

        static void Scene_VisibleChanged(object sender, EventArgs e)
        {
            if (Scene.Visible)
                SoundManager.PlaySound(SoundList.SelectMusic, true);
            else
                SoundManager.StopSound(SoundList.SelectMusic);
        }

        static void CharacterDisplay_BeforeDraw(object sender, EventArgs e)
        {

        }
        static void StartGameButton_Click(object sender, EventArgs e)
        {
            StartGameButton.Enabled = false;
            OutBound.StartGame();
        }
        static void DeleteCharacterButton_Click(object sender, EventArgs e)
        {
            if (SelectedIndex >= CharacterList.Count) return;
            SceneFunctions.DeleteCharacter(CharacterList[SelectedIndex].CharacterName, CharacterList[SelectedIndex].Index);
        }

        static void Scene_Shown(object sender, EventArgs e)
        {
            if (Login_Scene.LoginScene.Scene != null && !Login_Scene.LoginScene.Scene.IsDisposed)
                Login_Scene.LoginScene.Scene.Dispose();
        }
        static void Scene_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Enter, Arrorws 1 2 3 4 Delete Back
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (StartGameButton.Enabled)
                    StartGameButton.InvokeMouseClick(EventArgs.Empty);
                e.Handled = true;
            }
        }

        public static void CharacterDisplay_GlowEffect(object sender, EventArgs e)
        {
            MirAnimatedControl C = null;
            if ((C = sender as MirAnimatedControl) == null || Libraries.ChrSel == null) return;

            Libraries.ChrSel.DrawBlend(C.Index + 560, C.DisplayLocationWithoutOffSet, Color.White, true);
        }
        static void NewCharacterButton_Click(object sender, EventArgs e)
        {
            NewCharacterDialog.Show();
        }

        static void Character4Button_Click(object sender, EventArgs e)
        {
            if (CharacterList.Count >= 4 && SelectedIndex != 3)
            {
                SelectedIndex = 3;
                UpdateSelectButtons();
            }
        }
        static void Character3Button_Click(object sender, EventArgs e)
        {
            if (CharacterList.Count >= 3 && SelectedIndex != 2)
            {
                SelectedIndex = 2;
                UpdateSelectButtons();
            }
        }
        static void Character2Button_Click(object sender, EventArgs e)
        {
            if (CharacterList.Count >= 2 && SelectedIndex != 1)
            {
                SelectedIndex = 1;
                UpdateSelectButtons();
            }
        }
        static void Character1Button_Click(object sender, EventArgs e)
        {
            if (CharacterList.Count >= 1 && SelectedIndex != 0)
            {
                SelectedIndex = 0;
                UpdateSelectButtons();
            }
        }

        public static void UpdateButton(int ButtonNumber, int Index)
        {
            SelectCharacterInfo CI = Index >= CharacterList.Count ? null : CharacterList[Index];

            MirImageControl Button;

            switch (ButtonNumber)
            {
                case 1:
                    Button = Character1Button;
                    break;
                case 2:
                    Button = Character2Button;
                    break;
                case 3:
                    Button = Character3Button;
                    break;
                case 4:
                    Button = Character4Button;
                    break;
                default:
                    return;
            }

            if (CI == null)
            {
                for (int I = Button.Controls.Count - 1; I >= 0; I--)
                    Button.Controls[I].Visible = false;

                Button.Index = 45;
                Button.Sound = SoundList.None;
                return;
            }
            Button.Sound = SoundList.ClickA;

            ((MirLabel)Button.Controls[0]).Text = CI.CharacterName;
            ((MirLabel)Button.Controls[1]).Text = CI.Level.ToString();
            ((MirLabel)Button.Controls[2]).Text = CI.Class.ToString();

            for (int I = Button.Controls.Count - 1; I >= 0; I--)
                Button.Controls[I].Visible = true;

            Button.Index = 90 + (byte)CI.Class + (Index == SelectedIndex ? 4 : 0);

            if (Index == SelectedIndex)
            {
                if (DateTime.MinValue == CI.LastAccess)
                    LastAccessLabel.Text = "Never";
                else
                    LastAccessLabel.Text = CI.LastAccess.ToString();
                LastAccessLabelLabel.Visible = true;
            }

        }
        public static void UpdateSelectButtons()
        {
            LastAccessLabel.Text = null;
            LastAccessLabelLabel.Visible = false;
            if (SelectedIndex == -1)
            {
                SelectedIndex = CharacterList.Count > 0 ? 0 : -1;

                for (int I = 1; I < CharacterList.Count; I++)
                    if (CharacterList[I].LastAccess > CharacterList[SelectedIndex].LastAccess)
                        SelectedIndex = I;
            }

            UpdateButton(1, 0);
            UpdateButton(2, 1);
            UpdateButton(3, 2);
            UpdateButton(4, 3);

            if (SelectedIndex == -1 || SelectedIndex >= CharacterList.Count)
            {
                StartGameButton.Enabled = false;
                CharacterDisplay.Visible = false;
                return;
            }
            else
            {
                CharacterDisplay.Index = ((byte)CharacterList[SelectedIndex].Class + 1) * 20 + ((byte)CharacterList[SelectedIndex].Gender * 280);
                CharacterDisplay.FadeIn = true;
                CharacterDisplay.Opacity = 0;
                StartGameButton.Enabled = true;
                CharacterDisplay.Visible = true;
            }
        }

    }
}
