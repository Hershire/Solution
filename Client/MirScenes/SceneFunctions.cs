using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.MirControls;
using System.Drawing;
using System.Windows.Forms;
using Client.MirNetwork;
using Client.MirSound;
using Client.MirScenes.Login_Scene;

namespace Client.MirScenes
{
    static class SceneFunctions
    {
        public static int DeleteIndex;
        public static bool ConClosed;

        public static void ShowMessage(string Message)
        {
            MirMessageBox MMBox = new MirMessageBox(Message);
            MMBox.Show();
        }

        public static void QuitGame()
        {
            Network.Disconnect();
            if (Main.This.InvokeRequired)
                Main.This.Invoke(new Action(Main.This.Dispose));
            else
                Main.This.Dispose();
        }
        public static void ConnectionClosed()
        {
            if (ConClosed) return;

            ConClosed = true;
            MirMessageBox MMBox = new MirMessageBox("Failed to connect to the server.", MessageBoxButtons.OK);
            MMBox.OKButton.Click += (o, e) => SceneFunctions.QuitGame();
            MMBox.Show();
        }
        public static void WrongVersion()
        {
            MirMessageBox MMBox = new MirMessageBox("Wrong Version, Please update your game.\n Game will now close.", MessageBoxButtons.OK);
            MMBox.OKButton.Click += (o, e) => SceneFunctions.QuitGame();
            MMBox.Show();
        }
        public static void ConnectionLost()
        {
            if (ConClosed) return;

            ConClosed = true;
            MirMessageBox MMBox = new MirMessageBox("Lost connection with the server.");
            MMBox.OKButton.Click += (o, e) => SceneFunctions.QuitGame();
            MMBox.Show();
        }

        public static void RelocateLabel(object sender, EventArgs e)
        {
            if (sender != null)
                ((MirControl)sender).Location = new Point(-10 - (sender as MirControl).Size.Width, 0);
        }
        public static void CenterLabel(object sender, EventArgs e)
        {
            if (sender != null && ((MirControl)sender).Parent != null)
                ((MirControl)sender).Location = new Point((((MirControl)sender).Parent.Size.Width - ((MirControl)sender).Size.Width) / 2, 10);
        }


        public static void LoginSuccess()
        {
            LoginScene.Scene.Enabled = false;
            LoginDialog.Clear();
            LoginDialog.Hide();
            SoundManager.PlaySound(SoundList.LoginEffect, false);
            LoginScene.Background.Animated = true;

        }

        public static void LogOutSuccess()
        {
            Select_Scene.SelectScene.Scene.Visible = false;
            Game_Scene.GameScene.Scene.Enabled = true;

            Select_Scene.SelectScene.Scene.Enabled = true;
            Select_Scene.SelectScene.Scene.Visible = true;

            MirScene.ActiveScene = Select_Scene.SelectScene.Scene;

            Game_Scene.MapLayer.ObjectList.Clear();
            Game_Scene.MapLayer.LoadMap(null);

            Game_Scene.CharacterDialog.Hide();
            Game_Scene.InventoryDialog.Hide();
            Game_Scene.MenuPanel.Hide();
            
            //Handle rest

            //Kill All Game Sounds;
        }
        
        public static void DeleteCharacter(string Name, int Index)
        {
            MirMessageBox MMBox = new MirMessageBox(string.Format("Are you sure you want to Delete the character {0}?", Name), MessageBoxButtons.YesNo);
            DeleteIndex = Index;
            MMBox.YesButton.Click += DeleteCharacter;
            MMBox.Show();
        }
        static void DeleteCharacter(object sender, EventArgs e)
        {
            OutBound.DeleteCharacter();
        }

        public static void LogOut()
        {
            OutBound.LogOut();
            Game_Scene.GameScene.Scene.Enabled = false;

        }
    }
}
