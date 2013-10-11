using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using Client.MirGraphics;
using Client.MirControls;
using Client.MirNetwork;
using Library;

namespace Client.MirScenes.Login_Scene
{
    internal static class NewAccountDialog
    {
        private static MirImageControl NewAccountWindow;
        internal static MirButton ConfirmButton;
        private static MirButton CancelButton;

        public static MirTextBox AccountIDTextBox,
                                 Password1TextBox,
                                 EMailTextBox,
                                 UserNameTextBox,
                                 BirthDateTextBox,
                                 QuestionTextBox,
                                 AnswerTextBox;

        private static MirTextBox Password2TextBox;

        private static MirLabel Description;

        private static bool AccountIDValid,
                            Password1Valid,
                            Password2Valid,
                            EMailValid = true,
                            UserNameValid = true,
                            BirthDateValid = true,
                            QuestionValid = true,
                            AnswerValid = true;

        private static Regex Reg;

        static NewAccountDialog()
        {
            NewAccountWindow = new MirImageControl
            {
                Index = 63,
                Library = Libraries.Prguse,
                Parent = LoginScene.Background,
                Visible = false
            };
            NewAccountWindow.Location = new Point((Settings.ScreenSize.Width - NewAccountWindow.Size.Width) / 2,
                                             (Settings.ScreenSize.Height - NewAccountWindow.Size.Height) / 2);

            CancelButton = new MirButton
            {
                HoverIndex = 204,
                Index = 203,
                Library = Libraries.Title,
                Location = new Point(409, 425),
                Parent = NewAccountWindow,
                PressedIndex = 205
            };
            CancelButton.Click += CancelButton_Click;

            ConfirmButton = new MirButton
            {
                Enabled = false,
                HoverIndex = 201,
                Index = 200,
                Library = Libraries.Title,
                Location = new Point(135, 425),
                Parent = NewAccountWindow,
                PressedIndex = 202,
            };
            ConfirmButton.Click += ConfirmButton_Click;

            AccountIDTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 104),
                MaxLength = Globals.MaxAccountIDLength,
                Parent = NewAccountWindow,
                Size = new Size(136, 18),
            };
            AccountIDTextBox.TextBox.TextChanged += AccountIDTextBox_TextChanged;
            AccountIDTextBox.TextBox.GotFocus += AccountIDTextBox_GotFocus;

            Password1TextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 130),
                MaxLength = Globals.MaxPasswordLength,
                Parent = NewAccountWindow,
                Password = true,
                Size = new Size(136, 18),
            };
            Password1TextBox.TextBox.TextChanged += Password1TextBox_TextChanged;
            Password1TextBox.TextBox.GotFocus += PasswordTextBox_GotFocus;

            Password2TextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 156),
                MaxLength = Globals.MaxPasswordLength,
                Parent = NewAccountWindow,
                Password = true,
                Size = new Size(136, 18),
            };
            Password2TextBox.TextBox.TextChanged += Password2TextBox_TextChanged;
            Password2TextBox.TextBox.GotFocus += PasswordTextBox_GotFocus;

            UserNameTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 190),
                MaxLength = 20,
                Parent = NewAccountWindow,
                Size = new Size(136, 18),
            };
            UserNameTextBox.TextBox.TextChanged += UserNameTextBox_TextChanged;
            UserNameTextBox.TextBox.GotFocus += UserNameTextBox_GotFocus;


            BirthDateTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 216),
                MaxLength = 10,
                Parent = NewAccountWindow,
                Size = new Size(136, 18),
            };
            BirthDateTextBox.TextBox.TextChanged += BirthDateTextBox_TextChanged;
            BirthDateTextBox.TextBox.GotFocus += BirthDateTextBox_GotFocus;

            QuestionTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 251),
                MaxLength = 30,
                Parent = NewAccountWindow,
                Size = new Size(190, 18),
            };
            QuestionTextBox.TextBox.TextChanged += QuestionTextBox_TextChanged;
            QuestionTextBox.TextBox.GotFocus += QuestionTextBox_GotFocus;

            AnswerTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 277),
                MaxLength = 30,
                Parent = NewAccountWindow,
                Size = new Size(190, 18),
            };
            AnswerTextBox.TextBox.TextChanged += AnswerTextBox_TextChanged;
            AnswerTextBox.TextBox.GotFocus += AnswerTextBox_GotFocus;

            EMailTextBox = new MirTextBox
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(226, 312),
                MaxLength = 50,
                Parent = NewAccountWindow,
                Size = new Size(136, 18),
            };
            EMailTextBox.TextBox.TextChanged += EMailTextBox_TextChanged;
            EMailTextBox.TextBox.GotFocus += EMailTextBox_GotFocus;


            Description = new MirLabel
            {
                Border = true,
                BorderColor = Color.Gray,
                Location = new Point(15, 340),
                Parent = NewAccountWindow,
                Size = new Size(300, 70),
                Visible = false
            };
        }

        internal static void Hide()
        {
            NewAccountWindow.Visible = false;
        }

        internal static void Clear()
        {
            AccountIDTextBox.Text = string.Empty;
            AccountIDTextBox.BorderColor = Color.Gray;
            Password1TextBox.Text = string.Empty;
            Password1TextBox.BorderColor = Color.Gray;
            Password2TextBox.Text = string.Empty;
            Password2TextBox.BorderColor = Color.Gray;
            EMailTextBox.Text = string.Empty;
            EMailTextBox.BorderColor = Color.Gray;
            UserNameTextBox.Text = string.Empty;
            UserNameTextBox.BorderColor = Color.Gray;
        }

        public static void Show()
        {
            NewAccountWindow.Visible = true;
            AccountIDTextBox.SetFocus();
        }

        static void RefreshConfirmButton()
        {
            ConfirmButton.Enabled = AccountIDValid && Password1Valid && Password2Valid && EMailValid &&
                                    UserNameValid && BirthDateValid && QuestionValid && AnswerValid;
        }

        static void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
            LoginDialog.Show();
        }
        static void ConfirmButton_Click(object sender, EventArgs e)
        {
            ConfirmButton.Enabled = false;
            OutBound.NewAccount();
        }
        static void AccountIDTextBox_TextChanged(object sender, EventArgs e)
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
        static void Password1TextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

            if (string.IsNullOrEmpty(Password1TextBox.Text) || !Reg.IsMatch(Password1TextBox.Text))
            {
                Password1Valid = false;
                Password1TextBox.BorderColor = Color.Red;
            }
            else
            {
                Password1Valid = true;
                Password1TextBox.BorderColor = Color.Green;
            }
            Password2TextBox_TextChanged(sender, e);
        }
        static void Password2TextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");

            if (string.IsNullOrEmpty(Password2TextBox.Text) || !Reg.IsMatch(Password2TextBox.Text) ||
                Password1TextBox.Text != Password2TextBox.Text)
            {
                Password2Valid = false;
                Password2TextBox.BorderColor = Color.Red;
            }
            else
            {
                Password2Valid = true;
                Password2TextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void EMailTextBox_TextChanged(object sender, EventArgs e)
        {
            Reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (string.IsNullOrEmpty(EMailTextBox.Text))
            {
                EMailValid = true;
                EMailTextBox.BorderColor = Color.Gray;
            }
            else if (!Reg.IsMatch(EMailTextBox.Text))
            {
                EMailValid = false;
                EMailTextBox.BorderColor = Color.Red;
            }
            else
            {
                EMailValid = true;
                EMailTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserNameTextBox.Text))
            {
                UserNameValid = true;
                UserNameTextBox.BorderColor = Color.Gray;
            }
            else if (UserNameTextBox.Text.Length > 20)
            {
                UserNameValid = false;
                UserNameTextBox.BorderColor = Color.Red;
            }
            else
            {
                UserNameValid = true;
                UserNameTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void BirthDateTextBox_TextChanged(object sender, EventArgs e)
        {
            DateTime Temp;
            if (string.IsNullOrEmpty(BirthDateTextBox.Text))
            {
                BirthDateValid = true;
                BirthDateTextBox.BorderColor = Color.Gray;
            }
            else if (!DateTime.TryParse(BirthDateTextBox.Text, out Temp))
            {
                BirthDateValid = false;
                BirthDateTextBox.BorderColor = Color.Red;
            }
            else
            {
                BirthDateValid = true;
                BirthDateTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void QuestionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(QuestionTextBox.Text))
            {
                QuestionValid = true;
                QuestionTextBox.BorderColor = Color.Gray;
            }
            else if (QuestionTextBox.Text.Length > 30)
            {
                QuestionValid = false;
                QuestionTextBox.BorderColor = Color.Red;
            }
            else
            {
                QuestionValid = true;
                QuestionTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void AnswerTextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(AnswerTextBox.Text))
            {
                AnswerValid = true;
                AnswerTextBox.BorderColor = Color.Gray;
            }
            else if (AnswerTextBox.Text.Length > 30)
            {
                AnswerValid = false;
                AnswerTextBox.BorderColor = Color.Red;
            }
            else
            {
                AnswerValid = true;
                AnswerTextBox.BorderColor = Color.Green;
            }
            RefreshConfirmButton();
        }
        static void AccountIDTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text = " Description: Account ID.\n Accepted characters: a-z A-Z 0-9.\n Length: between " +
                               Globals.MinAccountIDLength + " and " + Globals.MaxAccountIDLength + " letters.";
        }
        static void PasswordTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text = " Description: Password.\n Accepted characters: a-z A-Z 0-9.\n Length: between " +
                               Globals.MinPasswordLength + " and " + Globals.MaxPasswordLength + " letters.";
        }
        static void EMailTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text =
                " Description: E-Mail Address.\n Format: Example@Example.Com.\n Max Length: 50 letters.\n Optional Field.";
        }
        static void UserNameTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text =
                " Description: User Name.\n Accepted characters:All.\n Length: between 0 and 20 letters.\n Optional Field.";
        }
        static void BirthDateTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text =
                string.Format(" Description: Birth Date.\n Format: {0}.\n Length: 10 letters.\n Optional Field.",
                              Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpper());
        }
        static void QuestionTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text =
                " Description: Secret Question.\n Accepted characters: All.\n Length: between 0 and 30 letters.\n Optional Field.";
        }
        static void AnswerTextBox_GotFocus(object sender, EventArgs e)
        {
            Description.Visible = true;
            Description.Text =
                " Description: Secret Answer.\n Accepted characters: All.\n Length: between 0 and 30 letters.\n Optional Field.";
        }
    }
}