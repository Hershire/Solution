using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using Microsoft.DirectX.Direct3D;

namespace Client.MirControls
{
    class MirMessageBox : MirControl
    {
        public MirImageControl Window;
        public MirButton OKButton, CancelButton, NoButton, YesButton;
        public MessageBoxButtons Buttons;

        public override Size Size
        {
            get
            {
                if (Window != null && !Window.IsDisposed)
                    return Window.Size;
                return base.Size;
            }
            set
            {
                if (Window != null && !Window.IsDisposed)
                    Window.Size = value;
                else base.Size = value;
            }
        }


        public MirMessageBox(string Message, MessageBoxButtons B = MessageBoxButtons.OK)
        {
            Buttons = B;
            Modal = true;
            Movable = false;

            Window = new MirImageControl
            {
                Index = 360,
                Library = Libraries.Prguse,
                Parent = this
            };
            Location = new Point((Settings.ScreenSize.Width - Window.Size.Width) / 2,
                                 (Settings.ScreenSize.Height - Window.Size.Height) / 2);

            new MirLabel
            {
                AutoSize = false,
                DrawFormat = DrawTextFormat.WordBreak,
                Location = new Point(35, 35),
                Size = new Size(390, 110),
                Parent = Window,
                Text = Message
            };

            switch (Buttons)
            {
                case MessageBoxButtons.OK:
                    OKButton = new MirButton
                    {
                        HoverIndex = 201,
                        Index = 200,
                        Library = Libraries.Title,
                        Location = new Point(360, 157),
                        Parent = Window,
                        PressedIndex = 202,
                    };
                    OKButton.Click += new EventHandler(DisposeDialog);
                    break;
                case MessageBoxButtons.OKCancel:
                    OKButton = new MirButton
                    {
                        HoverIndex = 201,
                        Index = 200,
                        Library = Libraries.Title,
                        Location = new Point(260, 157),
                        Parent = Window,
                        PressedIndex = 202,
                    };
                    OKButton.Click += new EventHandler(DisposeDialog);
                    CancelButton = new MirButton
                    {
                        HoverIndex = 204,
                        Index = 203,
                        Library = Libraries.Title,
                        Location = new Point(360, 157),
                        Parent = Window,
                        PressedIndex = 205,
                    };
                    CancelButton.Click += new EventHandler(DisposeDialog);
                    break;
                case MessageBoxButtons.YesNo:
                    YesButton = new MirButton
                    {
                        HoverIndex = 207,
                        Index = 206,
                        Library = Libraries.Title,
                        Location = new Point(260, 157),
                        Parent = Window,
                        PressedIndex = 208,
                    };
                    YesButton.Click += new EventHandler(DisposeDialog);
                    NoButton = new MirButton
                    {
                        HoverIndex = 211,
                        Index = 210,
                        Library = Libraries.Title,
                        Location = new Point(360, 157),
                        Parent = Window,
                        PressedIndex = 212,
                    };
                    NoButton.Click += new EventHandler(DisposeDialog);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    YesButton = new MirButton
                    {
                        HoverIndex = 207,
                        Index = 206,
                        Library = Libraries.Title,
                        Location = new Point(160, 157),
                        Parent = Window,
                        PressedIndex = 208,
                    };
                    YesButton.Click += new EventHandler(DisposeDialog);
                    NoButton = new MirButton
                    {
                        HoverIndex = 211,
                        Index = 210,
                        Library = Libraries.Title,
                        Location = new Point(260, 157),
                        Parent = Window,
                        PressedIndex = 212,
                    };
                    NoButton.Click += new EventHandler(DisposeDialog);
                    CancelButton = new MirButton
                    {
                        HoverIndex = 204,
                        Index = 203,
                        Library = Libraries.Title,
                        Location = new Point(360, 157),
                        Parent = Window,
                        PressedIndex = 205,
                    };
                    CancelButton.Click += new EventHandler(DisposeDialog);
                    break;

            }
        }

        void DisposeDialog(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Show()
        {
            Parent = MirScene.ActiveScene;
            Activate();
            Highlight();

            TextBox T;
            for (int I = 0; I < Main.This.Controls.Count; I++)
            {
                T = (TextBox)Main.This.Controls[I];
                if (T != null && T.Tag != null && (MirTextBox)T.Tag != null)
                    ((MirTextBox)T.Tag).DialogChanged();
            }
        }


        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            e.Handled = true;
        }
        public override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            e.Handled = true;
        }
        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (e.KeyChar == (char)Keys.Escape)
            {
                switch (Buttons)
                {
                    case MessageBoxButtons.OK:
                        if (OKButton != null & !OKButton.IsDisposed) OKButton.InvokeMouseClick(EventArgs.Empty);
                        break;
                    case MessageBoxButtons.OKCancel:
                    case MessageBoxButtons.YesNoCancel:
                        if (CancelButton != null & !CancelButton.IsDisposed) CancelButton.InvokeMouseClick(EventArgs.Empty);
                        break;
                    case MessageBoxButtons.YesNo:
                        if (NoButton != null & !NoButton.IsDisposed) NoButton.InvokeMouseClick(EventArgs.Empty);
                        break;
                }
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                switch (Buttons)
                {
                    case MessageBoxButtons.OK:
                    case MessageBoxButtons.OKCancel:
                        if (OKButton != null & !OKButton.IsDisposed) OKButton.InvokeMouseClick(EventArgs.Empty);
                        break;
                    case MessageBoxButtons.YesNoCancel:
                    case MessageBoxButtons.YesNo:
                        if (YesButton != null & !YesButton.IsDisposed) YesButton.InvokeMouseClick(EventArgs.Empty);
                        break;

                }
            }
            e.Handled = true;
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            base.Dispose(Disposing);

            if (Disposing)
            {
                TextBox T;
                for (int I = 0; I < Main.This.Controls.Count; I++)
                {
                    T = (TextBox)Main.This.Controls[I];
                    if (T != null && T.Tag != null && (MirTextBox)T.Tag != null)
                        ((MirTextBox)T.Tag).DialogChanged();
                }
            }
        }

        #endregion
    }
}
