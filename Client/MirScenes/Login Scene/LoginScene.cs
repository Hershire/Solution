using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.MirControls;
using Client.MirSound;
using System.Drawing;
using Client.MirGraphics;
using System.Windows.Forms;

namespace Client.MirScenes.Login_Scene
{
    static class LoginScene
    {
        public static MirScene Scene;
        public static MirAnimatedControl Background;
        private static MirLabel VersionLabel;

        static LoginScene()
        {
            Scene = new MirScene
            {
                Size = Settings.ScreenSize,
            };
            Scene.Shown += Scene_Shown;
            Scene.VisibleChanged += Scene_VisibleChanged;
            Scene.KeyPress += Scene_KeyPress;

            Background = new MirAnimatedControl
            {
                Animated = false,
                AnimationCount = 19,
                AnimationDelay = 100,
                Index = 0,
                Library = Libraries.ChrSel,
                Loop = false,
                Parent = Scene,
            };
            Background.AfterAnimation += new EventHandler(Background_AfterAnimation);

            VersionLabel = new MirLabel
            {
                AutoSize = true,
                BackColor = Color.FromArgb(200, 50, 50, 50),
                Border = true,
                BorderColor = Color.Black,
                Location = new Point(5, 580),
                Parent = Background,
                Text = "Version: " + Application.ProductVersion
            };
        }

        static void Background_AfterAnimation(object sender, EventArgs e)
        {
            Scene.Visible = false;
            Select_Scene.SelectScene.Scene.Visible = true;
            MirScene.ActiveScene = Select_Scene.SelectScene.Scene;
        }

        static void Scene_Shown(object sender, EventArgs e)
        {
            SoundManager.PlaySound(SoundList.LoginMusic, true);
        }
        static void Scene_VisibleChanged(object sender, EventArgs e)
        {
            if (!Background.Visible)
                SoundManager.StopSound(SoundList.LoginMusic);
        }

        static void Scene_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                SceneFunctions.QuitGame();
            }
        }

        public static void Connected()
        {
            LoginDialog.Show();
            LoginDialog.NewAccountButton.Visible = true;
            LoginDialog.ChangePasswordButton.Visible = true;
        }

        public static void ConnectionFailed()
        {
            MirMessageBox MMBox = new MirMessageBox("Failed to connect to the server...", MessageBoxButtons.RetryCancel);

            MMBox.CancelButton.Click += (o, e) => SceneFunctions.QuitGame();
            MMBox.Show();
        }
    }
}
