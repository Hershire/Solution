using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Microsoft.DirectX.Direct3D;
using Client.MirNetwork;
using Client.MirScenes.Game_Scene;

namespace Client
{
    public partial class Main : Form
    {
        public static Random Rand = new Random();
        public static DateTime LoadTime = DateTime.Now;
        public static Main This;
        public static long Time, MoveTime, AnimationTime, NextMoveTime, AttackTime, StruckRunTime, PickUpTime, UseItemTime;

        public static DateTime Now
        { get { return LoadTime.AddMilliseconds(Time); } }
        public static Stopwatch Timer = Stopwatch.StartNew();
        public static bool AllowRun, CanMove, UserCanMove;
            
        public static bool CanAttack
        {
            get { return Timer.ElapsedMilliseconds >= AttackTime; }
        }


        public Main()
        {
            InitializeComponent();

            This = this;

            Application.Idle += Application_Idle;
            FormClosing += Main_FormClosing;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Selectable, true);
            FormBorderStyle = Settings.FullScreen ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            ClientSize = Settings.ScreenSize;

            MouseClick += Main_MouseClick;
            MouseDown += Main_MouseDown;
            MouseUp += Main_MouseUp;
            MouseMove += Main_MouseMove;
            MouseDoubleClick += Main_MouseDoubleClick;
            KeyPress += Main_KeyPress;
            KeyDown += Main_KeyDown;
            KeyUp += Main_KeyUp;
            Deactivate += Main_Deactivate;

            DXManager.Create();
        }


        static void UpdateEnviroment()
        {            
            Time = Timer.ElapsedMilliseconds;
            Network.Process();

            if (Time >= MoveTime)
            {
                MoveTime +=  100; //Move Speed
                CanMove = true;
            }
            else
                CanMove = false;
            
            if (Time > AnimationTime)
            {
                AnimationTime += 100;
                MapLayer.AnimationCount++;
            }

            for (int I = 0; I < MirAnimatedControl.Animations.Count; I++)            
                MirAnimatedControl.Animations[I].UpdateOffSet();            
        }
        static void RenderEnviroment()
        {

            try
            {
                if (DXManager.DeviceLost)
                {
                    DXManager.AttemptReset();
                    Thread.Sleep(1);
                    return;
                }
                                
                if (MirScene.ActiveScene != null) MirScene.ActiveScene.Draw();

                DXManager.Device.Present();
            }
            catch (DeviceLostException)
            {
                //Do Nothing, Handled Elsewhere.
            }
            catch (Exception Ex)
            {
                if (Settings.LogErrors) SaveError(Ex.ToString());
                DXManager.AttemptRecovery();
            }
        }
        private void ToggleFullScreen()
        {
            Settings.FullScreen = !Settings.FullScreen;

            FormBorderStyle = Settings.FullScreen ? FormBorderStyle.None : FormBorderStyle.FixedDialog;
            DXManager.Parameters.Windowed = !Settings.FullScreen;
            DXManager.Device.Reset(DXManager.Parameters);
        }

        public static void SaveError(string Error)
        {
            try
            {
                if (Settings.MaxErrorLogCount > 0)
                {
                    DateTime Time = DateTime.Now;

                    File.AppendAllText(@".\Error.txt",
                                       string.Format("[{0}] {1}{2}", Time, Error, Environment.NewLine));
                    Settings.MaxErrorLogCount--;
                }
            }
            catch
            {
            }
        }

        public static Point PointToC(Point P)
        {
            if (This.InvokeRequired)
            {
                This.Invoke((MethodInvoker)(() =>
                {
                    P = This.PointToClient(P);
                }));
                return P;
            }
            else
                return This.PointToClient(P);
        }

        void Main_Deactivate(object sender, EventArgs e)
        {
            GameScene.MouseB = MouseButtons.None;
            GameScene.Shift = false;
            GameScene.Alt = false;
        }

        void Main_KeyUp(object sender, KeyEventArgs e)
        {
            GameScene.Shift = e.Shift;
            GameScene.Alt = e.Alt;
            GameScene.Ctrl = e.Control;

            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnKeyUp(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_KeyDown(object sender, KeyEventArgs e)
        {
            GameScene.Shift = e.Shift;
            GameScene.Alt = e.Alt;
            GameScene.Ctrl = e.Control;
            try
            {
                if (e.Alt && e.KeyCode == Keys.Enter)
                {
                    ToggleFullScreen();
                    return;
                }

                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnKeyDown(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnKeyPress(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseClick(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_MouseUp(object sender, MouseEventArgs e)
        {
            GameScene.MouseB &= ~e.Button;
            
            if (e.Button == MouseButtons.Right)
                AllowRun = false;

            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseUp(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseMove(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (MirItemCell.SelectedCell != null || MirItemCell.PickedUpGold))
            {
                MirItemCell.SelectedCell = null;
                MirItemCell.PickedUpGold = false;
            }
            
            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseDown(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        void Main_MouseClick(object sender, MouseEventArgs e)
        {

            try
            {
                if (MirScene.ActiveScene != null)
                    MirScene.ActiveScene.OnMouseClick(e);
            }
            catch (Exception Ex)
            {
                SaveError(Ex.ToString());
            }
        }
        private static void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Network.Network.Disconnect();
        }
        private static void Application_Idle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                UpdateEnviroment();
                RenderEnviroment();
            }
        }
        #region Idle Check

        private static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin,
                                               uint messageFilterMax, uint flags);

        #endregion

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
