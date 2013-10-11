using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using Microsoft.DirectX.Direct3D;
using Client.MirScenes.Login_Scene;
using Client.MirObjects;

namespace Client.MirControls
{
    class MirScene : MirControl
    {
        public static MirScene ActiveScene = LoginScene.Scene;
        private static MouseButtons Buttons;
        private static long LastClickTime;
        private static MirControl ClickedControl;
        private static Point OldLocation;

        private Texture SceneTexture;

        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();
            if (!Visible && ActiveScene == this) ActiveScene = null;
        }

        public MirScene()
        {
        }

        internal override void Draw()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);


            DXManager.Device.BeginScene();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);
            
            base.Draw();
            
            DXManager.Sprite.End();
            DXManager.Device.EndScene();
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (!Enabled) return;

            if (MouseControl != null && MouseControl != this)
                MouseControl.OnMouseDown(e);
            else
                base.OnMouseDown(e);
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            if (!Enabled) return;
            if (MouseControl != null && MouseControl != this)
                MouseControl.OnMouseUp(e);
            else
                base.OnMouseUp(e);
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            if (!Enabled) return;
            if (Buttons == e.Button)
            {
                if (LastClickTime + SystemInformation.DoubleClickTime >= Main.Time)
                {
                    OnMouseDoubleClick(e);
                    return;
                }
            }
            else
                LastClickTime = 0;

            if (ActiveControl != null && ActiveControl.IsMouseOver(e.Location) && ActiveControl != this)
                ActiveControl.OnMouseClick(e);
            else
                base.OnMouseClick(e);

            ClickedControl = ActiveControl;

            LastClickTime = Main.Time;
            Buttons = e.Button;
        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!Enabled) return;
            LastClickTime = 0;
            Buttons = MouseButtons.None;

            if (ActiveControl != null && ActiveControl.IsMouseOver(e.Location) && ActiveControl != this)
            {
                if (ActiveControl == ClickedControl)
                    ActiveControl.OnMouseDoubleClick(e);
                else
                    ActiveControl.OnMouseClick(e);
            }
            else
            {
                if (ActiveControl == ClickedControl)
                    base.OnMouseDoubleClick(e);
                else
                    base.OnMouseClick(e);
            }
        }


        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (SceneTexture != null && !SceneTexture.Disposed)                
                    SceneTexture.Dispose();
                SceneTexture = null;
                                
                if (this == ActiveScene)
                    ActiveScene = null;
            }

            base.Dispose(Disposing);
        }

        #endregion
    }
}
