using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library;
using Client.MirControls;
using Client.MirGraphics;
using System.Drawing;
using System.Windows.Forms;
using Client.MirNetwork;

namespace Client.MirScenes.Game_Scene
{
    class ChatPanel
    {
        public static MirImageControl Window, ControlBar;
        public static MirButton SizeButton, SettingsButton;
        public static MirButton NormalButton, ShoutButton, WhisperButton, LoverButton, MentorButton, GroupButton, GuildButton;
        public static MirTextBox ChatTextBox;
        public static List<ChatInfo> History = new List<ChatInfo>();
        public static int ChatSize, LineCount = 4, TransparencyOffSet;
        public static MirLabel[] ChatLines = new MirLabel[12];
        public static int StartIndex;
        public static string LastPM = string.Empty, PreChat = string.Empty;


        public static void Create()
        {
            Window = new MirImageControl
            {
                Index = 2201,
                Library = Libraries.Prguse,
                Parent = GameScene.Scene,
                Location = new Point(230, 503),
                Visible = true
            };


            ChatTextBox = new MirTextBox
            {
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                Parent = Window,
                Size = new Size(403, 13),
                Location = new Point(1, 54),
                MaxLength = Globals.MaxChatLength,
                Visible = false,
            };
            ChatTextBox.TextBox.Font = new Font("Microsoft Sans Serif", 8F);
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;

            ControlBar = new MirImageControl
            {
                Index = 2035,
                Library = Libraries.Prguse,
                Parent = GameScene.Scene,
                Location = new Point(230, 488),
                Visible = true
            };

            SizeButton = new MirButton
            {
                Index = 2057,
                HoverIndex = 2058,
                PressedIndex = 2059,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(350, 1),
                Visible = true
            };
            SizeButton.Click += SizeButton_Click;

            SettingsButton = new MirButton
            {
                Index = 2060,
                HoverIndex = 2061,
                PressedIndex = 2062,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(372, 1),
                Visible = true
            };
            SettingsButton.Click += SettingsButton_Click;

            NormalButton = new MirButton
            {
                Index = 2037,
                HoverIndex = 2037,
                PressedIndex = 2038,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(12, 1),
                Visible = true
            };
            NormalButton.Click += NormalButton_Click;


            ShoutButton = new MirButton
            {
                Index = 2039,
                HoverIndex = 2040,
                PressedIndex = 2041,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(34, 1),
                Visible = true
            };
            ShoutButton.Click += ShoutButton_Click;

            WhisperButton = new MirButton
            {
                Index = 2042,
                HoverIndex = 2043,
                PressedIndex = 2044,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(56, 1),
                Visible = true
            };
            WhisperButton.Click += WhisperButton_Click;


            LoverButton = new MirButton
            {
                Index = 2045,
                HoverIndex = 2046,
                PressedIndex = 2047,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(78, 1),
                Visible = true
            };
            LoverButton.Click += LoverButton_Click;
            
            MentorButton = new MirButton
            {
                Index = 2048,
                HoverIndex = 2049,
                PressedIndex = 2050,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(100, 1),
                Visible = true
            };
            MentorButton.Click += MentorButton_Click;

            GroupButton = new MirButton
            {
                Index = 2051,
                HoverIndex = 2052,
                PressedIndex = 2053,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(122, 1),
                Visible = true
            };
            GroupButton.Click += GroupButton_Click;

            GuildButton = new MirButton
            {
                Index = 2054,
                HoverIndex = 2055,
                PressedIndex = 2056,
                Library = Libraries.Prguse,
                Parent = ControlBar,
                Location = new Point(144, 1),
                Visible = true
            };
            GuildButton.Click += GuildButton_Click;


            for (int I = 0; I < ChatLines.Length; I++)
                ChatLines[I] = new MirLabel
                {
                    AutoSize = true,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    OutLine = false,
                    // Size = new Size(403, 13),
                    Parent = Window,
                    Location = new Point(1, 2 + I * 12),
                    Font = new Font("Microsoft Sans Serif", 8F),
                    Visible = I < LineCount,
                };
        }

        static void GuildButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex ;
            PreChat = "!~";
        }

        static void GroupButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = "!!";
        }

        static void MentorButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = @"\";
        }

        static void LoverButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = ":)";
        }

        static void WhisperButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = "/";
        }

        static void ShoutButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex -1;
            ShoutButton.Index = ShoutButton.HoverIndex ;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = "!";
        }

        static void NormalButton_Click(object sender, EventArgs e)
        {
            NormalButton.Index = NormalButton.HoverIndex;
            ShoutButton.Index = ShoutButton.HoverIndex -1;
            WhisperButton.Index = WhisperButton.HoverIndex -1;
            LoverButton.Index = LoverButton.HoverIndex -1;
            MentorButton.Index = MentorButton.HoverIndex -1;
            GroupButton.Index = GroupButton.HoverIndex -1;
            GuildButton.Index = GuildButton.HoverIndex -1;
            PreChat = string.Empty;
        }

        static void SizeButton_Click(object sender, EventArgs e)
        {
            if (++ChatSize >= 3) ChatSize = 0;

            int Bot = Window.DisplayRectangle.Bottom;
            switch (ChatSize)
            {
                case 0:
                    LineCount = 4;
                    Window.Index = 2201;
                    StartIndex += 8;
                    break;
                case 1:
                    LineCount = 8;
                    Window.Index = 2204;
                    StartIndex -= 4;
                    break;
                case 2:
                    LineCount = 12;
                    Window.Index = 2207;
                    StartIndex -= 4;
                    break;
            }
            Window.Location = new Point(Window.Location.X, Bot - Window.Size.Height);
            ControlBar.Location = new Point(ControlBar.Location.X, Window.DisplayRectangle.Top - ControlBar.Size.Height);
            //Belt Location

            for (int I = 0; I < ChatLines.Length; I++)
                ChatLines[I].Visible = I < LineCount;

            UpdateChatList();
        }

        static void SettingsButton_Click(object sender, EventArgs e)
        {
            if (++ChatSize >= 3) ChatSize = 0;
            int Bot = Window.DisplayRectangle.Bottom;
            switch (ChatSize)
            {
                case 0:
                    LineCount = 4;
                    Window.Index = 2200;
                    StartIndex += 8;
                    break;
                case 1:
                    LineCount = 8;
                    Window.Index = 2203;
                    StartIndex -= 4;
                    break;
                case 2:
                    LineCount = 12;
                    Window.Index = 2206;
                    StartIndex -= 4;
                    break;
            }
            Window.Location = new Point(Window.Location.X, Bot - Window.Size.Height);
            ControlBar.Location = new Point(ControlBar.Location.X, Window.DisplayRectangle.Top - ControlBar.Size.Height);
            //Belt Location

            for (int I = 0; I < ChatLines.Length; I++)
                ChatLines[I].Visible = I < LineCount;

            UpdateChatList();
        }

        static void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (!string.IsNullOrEmpty(ChatTextBox.Text))
                {
                    OutBound.Chat();

                    if (ChatTextBox.Text[0] == '/')
                    {
                        RecieveChat(new ChatInfo { Message = ChatTextBox.TextBox.Text, Type = MirChatType.OutBoundWhisper });
                        LastPM = ChatTextBox.Text.Substring(1);
                        int TempI = LastPM.IndexOf(' ');
                        if (TempI != -1)
                            LastPM = LastPM.Substring(0, TempI);
                    }
                }
                ChatTextBox.Visible = false;
                ChatTextBox.Text = string.Empty;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                e.Handled = true;
                ChatTextBox.Visible = false;
                ChatTextBox.Text = string.Empty;
            }
            else if (e.KeyChar == (char)Keys.Tab)
                e.Handled = false;
        }



        public static void RecieveChat(ChatInfo Chat)
        {
            History.Add(Chat);
            if (StartIndex == History.Count - LineCount - 1)
                StartIndex = History.Count - LineCount;
            UpdateChatList();
        }
        public static void UpdateChatList()
        {
            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;
            for (int I = 0; I < ChatLines.Length; I++)
            {
                if (I + StartIndex >= History.Count || I >= LineCount)
                {
                    ChatLines[I].Visible = false;
                    ChatLines[I].Text = string.Empty;
                }
                else
                {
                    ChatLines[I].Visible = true;
                    switch (History[I + StartIndex].Type)
                    {
                        case MirChatType.Normal:
                            ChatLines[I].Text = string.Format("{0}:{1}", History[I + StartIndex].User, History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.White;
                            ChatLines[I].ForeColor = Color.Black;
                            break;
                        case MirChatType.Shout:
                            ChatLines[I].Text = string.Format("(!){0}:{1}", History[I + StartIndex].User, History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.Yellow;
                            ChatLines[I].ForeColor = Color.Black;
                            break;
                        case MirChatType.Group:
                            ChatLines[I].Text = string.Format("-{0}:{1}", History[I + StartIndex].User, History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.White;
                            ChatLines[I].ForeColor = Color.Gray;
                            break;
                        case MirChatType.Guild:
                            ChatLines[I].Text = string.Format("{0}:{1}", History[I + StartIndex].User, History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.White;
                            ChatLines[I].ForeColor = Color.Green;
                            break;
                        case MirChatType.BlueSystem:
                            ChatLines[I].Text = string.Format("{0}", History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.Blue;
                            ChatLines[I].ForeColor = Color.White;
                            break;
                        case MirChatType.RedSystem:
                            ChatLines[I].Text = string.Format("{0}", History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.Red;
                            ChatLines[I].ForeColor = Color.White;
                            break;
                        case MirChatType.Experience:
                            ChatLines[I].Text = string.Format("{0}", History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.Red;
                            ChatLines[I].ForeColor = Color.White;
                            break;
                        case MirChatType.Whisper:
                            ChatLines[I].Text = string.Format("{0}=>{1}", History[I + StartIndex].User, History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.White;
                            ChatLines[I].ForeColor = Color.FromArgb(0, 0, 255);
                            break;
                        case MirChatType.OutBoundWhisper:
                            ChatLines[I].Text = string.Format("{0}", History[I + StartIndex].Message);
                            ChatLines[I].BackColor = Color.White;
                            ChatLines[I].ForeColor = Color.FromArgb(200, 16,75, 200);
                            break;
                    }

                }
            }

        }
    }


    public class ChatInfo
    {
        public string User;
        public string Message;
        public MirChatType Type;
    }
}
