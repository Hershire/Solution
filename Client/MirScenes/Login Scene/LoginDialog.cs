using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.MirControls;
using System.Text.RegularExpressions;
using Client.MirGraphics;
using System.Drawing;
using Library;
using System.Windows.Forms;
using Client.MirSound;
using Client.MirNetwork;

namespace Client.MirScenes.Login_Scene
{
    static class LoginDialog
    {
        static MirImageControl LoginWindow, Title;
        public static MirTextBox AccountIDTextBox, PasswordTextBox;
        public static MirButton LoginButton;
        public static MirButton NewAccountButton, ChangePasswordButton, CloseButton;
        static bool AccountIDValid, PasswordValid;
        static Regex Reg;

        static LoginDialog()
        {
            LoginWindow = new MirImageControl
            {
                Index = 1084,
                Library = Libraries.Prguse,
                Parent = LoginScene.Background,
            };
            LoginWindow.Location = new Point((Settings.ScreenSize.Width - LoginWindow.Size.Width) / 2,
                                             (Settings.ScreenSize.Height - LoginWindow.Size.Height) / 2);

            Title = new MirImageControl
            {
                Index = 30,
                Library = Libraries.Title,
                Parent = LoginWindow,
            };
            Title.Location = new Point((LoginWindow.Size.Width - Title.Size.Width) / 2, 5);

            NewAccountButton = new MirButton
            {
                HoverIndex = 324,
                Index = 323,
                Library = Libraries.Title,
                Location = new Point(60, 163),
                Parent = LoginWindow,
                PressedIndex = 325
            };
            NewAccountButton.Click += NewAccountButton_Click;

            LoginButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 321,
                Index = 320,
                Library = Libraries.Title,
                Location = new Point(227, 83),
                Parent = LoginWindow,
                PressedIndex = 322,
            };
            LoginButton.Click += new EventHandler(LoginButton_Click);

            ChangePasswordButton = new MirButton
            {
                HoverIndex = 327,
                Index = 326,
                Library = Libraries.Title,
                Location = new Point(166, 163),
                Parent = LoginWindow,
                PressedIndex = 328
            };
            ChangePasswordButton.Click += ChangePasswordButton_Click;

            CloseButton = new MirButton
            {
                HoverIndex = 330,
                Index = 329,
                Library = Libraries.Title,
                Location = new Point(166, 189),
                Parent = LoginWindow,
                PressedIndex = 331,
                Size = new Size(100, 24)
            };
            CloseButton.Click += (o, e) => SceneFunctions.QuitGame();

            AccountIDTextBox = new MirTextBox
            {
                Location = new Point(85, 85),
                Parent = LoginWindow,
                Size = new Size(136, 15)
            };
            AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;
            AccountIDTextBox.TextBox.KeyPress += TextBox_KeyPress;
            AccountIDTextBox.SetFocus();

            PasswordTextBox = new MirTextBox
            {
                Location = new Point(85, 108),
                Parent = LoginWindow,
                Password = true,
                Size = new Size(136, 15)
            };
            PasswordTextBox.TextBox.TextChanged += PasswordTextBox_TextChanged;
            PasswordTextBox.TextBox.KeyPress += TextBox_KeyPress;
        }

        static void LoginButton_Click(object sender, EventArgs e)
        {
            LoginButton.Enabled = false;
            OutBound.Login();
        }

        static void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            Hide();
            ChangePasswordDialog.Show();
        }
        static void NewAccountButton_Click(object sender, EventArgs e)
        {
            Hide();
            NewAccountDialog.Show();
        }

        internal static void Clear()
        {
            AccountIDTextBox.Text = string.Empty;
            AccountIDTextBox.Border = false;
            AccountIDTextBox.BorderColor = Color.Gray;
            PasswordTextBox.Text = string.Empty;
            PasswordTextBox.Border = false;
            PasswordTextBox.BorderColor = Color.Gray;
        }
        public static void Show()
        {
            LoginWindow.Visible = true;
            AccountIDTextBox.SetFocus();
        }
        public static void Hide()
        {
            LoginWindow.Visible = false;
        }

        private static void RefreshLoginButton()
        {
            LoginButton.Enabled = AccountIDValid && PasswordValid;
        }
        private static void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sender == null) return;
            if (e.KeyChar != (char)Keys.Enter) return;
            e.Handled = true;
            if (!AccountIDValid)
            {
                AccountIDTextBox.SetFocus();
                return;
            }
            if (!PasswordValid)
            {
                PasswordTextBox.SetFocus();
                return;
            }
            if (LoginButton.Enabled)
                LoginButton.InvokeMouseClick(EventArgs.Empty);
        }
        private static void AccountIDTextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");

            if (string.IsNullOrEmpty(AccountIDTextBox.TextBox.Text) || !Reg.IsMatch(AccountIDTextBox.TextBox.Text))
            {
                AccountIDValid = false;
                AccountIDTextBox.Border = !string.IsNullOrEmpty(AccountIDTextBox.TextBox.Text);
                AccountIDTextBox.BorderColor = Color.Red;
            }
            else
            {
                AccountIDValid = true;
                AccountIDTextBox.Border = true;
                AccountIDTextBox.BorderColor = Color.Green;
            }

            RefreshLoginButton();
        }
        private static void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

            if (string.IsNullOrEmpty(PasswordTextBox.TextBox.Text) || !Reg.IsMatch(PasswordTextBox.TextBox.Text))
            {
                PasswordValid = false;
                PasswordTextBox.Border = !string.IsNullOrEmpty(PasswordTextBox.TextBox.Text);
                PasswordTextBox.BorderColor = Color.Red;
            }
            else
            {
                PasswordValid = true;
                PasswordTextBox.Border = true;
                PasswordTextBox.BorderColor = Color.Green;
            }

            RefreshLoginButton();
        }
    }
}
