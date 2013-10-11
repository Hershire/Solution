using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using Library.MirNetwork.ClientPackets;
using Library;
using System.Windows.Forms;
using Client.MirScenes.Login_Scene;
using Client.MirScenes.Select_Scene;
using Client.MirObjects;
using Client.MirScenes.Game_Scene;

namespace Client.MirNetwork
{
    static class OutBound
    {
        public static void ClientVersion()
        {
            ClientVersion P = new ClientVersion();
            try
            {
                using (FileStream FStream = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read))
                using (MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider())
                    P.VersionHash = MD5.ComputeHash(FStream);
                Network.Enqueue(P);
            }
            catch (Exception Ex)
            {
                if (Settings.LogErrors) Main.SaveError(Ex.ToString());
            }
        }
        public static void KeepAlive()
        {
            Network.Enqueue(new KeepAlive());
        }
        public static void NewAccount()
        {
            NewAccount P = new NewAccount
            {
                AccountID = NewAccountDialog.AccountIDTextBox.Text,
                Password = NewAccountDialog.Password1TextBox.Text,
                EMailAddress = NewAccountDialog.EMailTextBox.Text ?? string.Empty,
                BirthDate =
                    !string.IsNullOrEmpty(NewAccountDialog.BirthDateTextBox.Text)
                        ? DateTime.Parse(NewAccountDialog.BirthDateTextBox.Text)
                        : DateTime.MinValue,
                UserName = NewAccountDialog.UserNameTextBox.Text ?? string.Empty,
                SecretQuestion = NewAccountDialog.QuestionTextBox.Text ?? string.Empty,
                SecretAnswer = NewAccountDialog.AnswerTextBox.Text ?? string.Empty
            };

            Network.Enqueue(P);
        }
        public static void ChangePassword()
        {
            ChangePassword P = new ChangePassword
            {
                AccountID = ChangePasswordDialog.AccountIDTextBox.Text,
                CurrentPassword = ChangePasswordDialog.CurrentPasswordTextBox.Text,
                NewPassword = ChangePasswordDialog.NewPassword1TextBox.Text
            };
            Network.Enqueue(P);
        }
        public static void Login()
        {
            Login P = new Login
            {
                AccountID = LoginDialog.AccountIDTextBox.Text,
                Password = LoginDialog.PasswordTextBox.Text
            };

            Network.Enqueue(P);
        }
        public static void NewCharacter()
        {
            NewCharacter P = new NewCharacter
            {
                CharacterName = NewCharacterDialog.CharacterNameTextBox.Text,
                Class = NewCharacterDialog.NewClass,
                Gender = NewCharacterDialog.NewGender
            };

            Network.Enqueue(P);
        }
        public static void DeleteCharacter()
        {
            DeleteCharacter P = new DeleteCharacter
            {
                CharacterIndex = MirScenes.SceneFunctions.DeleteIndex,
            };

            Network.Enqueue(P);
        }
        public static void StartGame()
        {
            StartGame P = new StartGame
            {
                CharacterIndex = SelectScene.CharacterList[SelectScene.SelectedIndex].Index
            };
            Network.Enqueue(P);
        }

        public static void LogOut()
        {
            Network.Enqueue(new LogOut());
        }

        public static void Chat()
        {
            Network.Enqueue(new Chat { Message = ChatPanel.ChatTextBox.Text });
        }

        public static void Turn()
        {
            Network.Enqueue(new Turn { Direction = MapObject.User.Direction });
            Main.NextMoveTime = Main.Time + 5000;
        }
        public static void Walk()
        {
            Network.Enqueue(new Walk { Direction = MapObject.User.Direction });
            Main.NextMoveTime = Main.Time + 5000;
        }
        public static void Run()
        {
            Network.Enqueue(new Run { Direction = MapObject.User.Direction });
            Main.NextMoveTime = Main.Time + 5000;
        }
        public static void Attack(byte T)
        {
            Network.Enqueue(new Attack { Direction = MapObject.User.Direction, Type = T });
           // Main.CanAct = false;
        }
        public static void Harvest()
        {
            Network.Enqueue(new Harvest { Direction = MapObject.User.Direction });
            //Main.CanAct = false;
        }

        public static void MoveItem(MirGridType G, int F, int T)
        {
            Network.Enqueue(new MoveItem { Grid = G, From = F, To = T });
        }
        public static void EquipItem(MirGridType G, int U, int T)
        {
            Network.Enqueue(new EquipItem { Grid = G, UniqueID = U, To = T });
        }
        public static void RemoveItem(MirGridType G, int U, int T)
        {
            Network.Enqueue(new RemoveItem { Grid = G, UniqueID = U, To = T });
        }
        public static void UseItem(MirGridType G, int U)
        {
            Network.Enqueue(new UseItem { Grid = G, UniqueID = U});
        }

        public static void PickUp()
        {
            Network.Enqueue(new PickUp());
        }

        public static void DropItem(MirGridType G, int U)
        {
            Network.Enqueue(new DropItem { Grid = G, UniqueID = U });
        }
        public static void DropGold(int Amount)
        {
            Network.Enqueue(new DropGold { Amount = Amount });
        }
    

    }
}
