using System;
using System.Drawing;
using System.Text.RegularExpressions;
using Client.MirGraphics;
using Client.MirControls;
using Client.MirNetwork;
using Library;


namespace Client.MirScenes.Login_Scene
{
    static class ChangePasswordDialog
    {
        public static MirImageControl ChangePasswordWindow;
        public static MirButton ConfirmButton;
        static MirButton CancelButton;
        public static MirTextBox AccountIDTextBox, CurrentPasswordTextBox, NewPassword1TextBox;
        static MirTextBox NewPassword2TextBox;
        static bool AccountIDValid, CurrentPasswordValid, NewPassword1Valid, NewPassword2Valid;
        static Regex Reg;

        static ChangePasswordDialog()
        {
            ChangePasswordWindow = new MirImageControl
            {
                Index = 50,
                Library = Libraries.Prguse,
                Parent = LoginScene.Background,
                Visible = false
            };
            ChangePasswordWindow.Location = new Point((Settings.ScreenSize.Width - ChangePasswordWindow.Size.Width) / 2,
                                             (Settings.ScreenSize.Height - ChangePasswordWindow.Size.Height) / 2);

            CancelButton = new MirButton
            {
                HoverIndex = 111,
                Index = 110,
                Library = Libraries.Title,
                Location = new Point(222, 236),
                Parent = ChangePasswordWindow,
                PressedIndex = 112
            };
            CancelButton.Click += CancelButton_Click;

            ConfirmButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 108,
                Index = 107,
                Library = Libraries.Title,
                Location = new Point(80, 236),
                Parent = ChangePasswordWindow,
                PressedIndex = 109,
            };
            ConfirmButton.Click += ConfirmButton_Click;


            AccountIDTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(178, 76),
                MaxLength = Globals.MaxAccountIDLength,
                Parent = ChangePasswordWindow,
                Size = new Size(136, 18),
            };
            AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;

            CurrentPasswordTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(178, 114),
                MaxLength = Globals.MaxPasswordLength,
                Parent = ChangePasswordWindow,
                Password = true,
                Size = new Size(136, 18),
            };
            CurrentPasswordTextBox.TextBox.TextChanged += CurrentPasswordTextBox_TextChanged;

            NewPassword1TextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(178, 152),
                MaxLength = Globals.MaxPasswordLength,
                Parent = ChangePasswordWindow,
                Password = true,
                Size = new Size(136, 18),
            };
            NewPassword1TextBox.TextBox.TextChanged += NewPassword1TextBox_TextChanged;

            NewPassword2TextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(178, 189),
                MaxLength = Globals.MaxPasswordLength,
                Parent = ChangePasswordWindow,
                Password = true,
                Size = new Size(136, 18),
            };
            NewPassword2TextBox.TextBox.TextChanged += NewPassword2TextBox_TextChanged;
        }

        internal static void Hide()
        {
            ChangePasswordWindow.Visible = false;
        }

        internal static void Clear()
        {
            AccountIDTextBox.Text = string.Empty;
            AccountIDTextBox.BorderColor = Color.Gray;
            CurrentPasswordTextBox.Text = string.Empty;
            CurrentPasswordTextBox.BorderColor = Color.Gray;
            NewPassword1TextBox.Text = string.Empty;
            NewPassword1TextBox.BorderColor = Color.Gray;
            NewPassword2TextBox.Text = string.Empty;
            NewPassword2TextBox.BorderColor = Color.Gray;
        }

        public static void Show()
        {
            ChangePasswordWindow.Visible = true;
            AccountIDTextBox.SetFocus();
        }

        private static void RefreshConfirmButton()
        {
            ConfirmButton.Enabled = AccountIDValid && CurrentPasswordValid && NewPassword1Valid && NewPassword2Valid;
        }

        private static void ConfirmButton_Click(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = false;
            OutBound.ChangePassword();
        }
        private static void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
            LoginDialog.Show();
        }

        private static void AccountIDTextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");

            if (string.IsNullOrEmpty(AccountIDTextBox.Text) || !Reg.IsMatch(AccountIDTextBox.Text))
            {
                AccountIDValid = false;
                AccountIDTextBox.BorderColor = Color.Red;
            }
            else
            {
                AccountIDValid = true;
                AccountIDTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        private static void CurrentPasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

            if (string.IsNullOrEmpty(CurrentPasswordTextBox.Text) || !Reg.IsMatch(CurrentPasswordTextBox.Text))
            {
                CurrentPasswordValid = false;
                CurrentPasswordTextBox.BorderColor = Color.Red;
            }
            else
            {
                CurrentPasswordValid = true;
                CurrentPasswordTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        private static void NewPassword1TextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

            if (string.IsNullOrEmpty(NewPassword1TextBox.Text) || !Reg.IsMatch(NewPassword1TextBox.Text))
            {
                NewPassword1Valid = false;
                NewPassword1TextBox.BorderColor = Color.Red;
            }
            else
            {
                NewPassword1Valid = true;
                NewPassword1TextBox.BorderColor = Color.Green;
            }
            NewPassword2TextBox_TextChanged(sender, e);
        }
        private static void NewPassword2TextBox_TextChanged(object sender, EventArgs e)
        {
            if (NewPassword1TextBox.Text == NewPassword2TextBox.Text)
            {
                NewPassword2Valid = NewPassword1Valid;
                NewPassword2TextBox.BorderColor = NewPassword1TextBox.BorderColor;
            }
            else
            {
                NewPassword2Valid = false;
                NewPassword2TextBox.BorderColor = Color.Red;
            }
            RefreshConfirmButton();
        }
    }
}