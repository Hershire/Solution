using Client.MirGraphics;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.MirControls
{
    class MirInputBox : MirControl
    {
        public MirImageControl Window;
        public MirButton OKButton, CancelButton;
        public MirTextBox InputTextBox;

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

        public MirInputBox(string Message)
        {
            Modal = true;
            Movable = false;
            Window = new MirImageControl
            {
                Index = 660,
                Library = Libraries.Prguse,
                Parent = this
            };

            Location = new Point((Settings.ScreenSize.Width - Window.Size.Width) / 2,
                                 (Settings.ScreenSize.Height - Window.Size.Height) / 2);

            new MirLabel
            {
                DrawFormat = DrawTextFormat.WordBreak,
                Location = new Point(25, 25),
                Size = new Size(235, 40),
                Parent = Window,
                Text = Message,
            };

            InputTextBox = new MirTextBox
            {
                Parent = Window,
                Border = true,
                BorderColor = Color.Lime,
                Location = new Point(23, 86),
                Size = new Size(240, 19),
            };
            InputTextBox.SetFocus();
            InputTextBox.TextBox.KeyPress += new KeyPressEventHandler(MirInputBox_KeyPress);

            OKButton = new MirButton
            {
                HoverIndex = 201,
                Index = 200,
                Library = Libraries.Title,
                Location = new Point(60, 123),
                Parent = Window,
                PressedIndex = 202,
            };

            CancelButton = new MirButton
            {
                HoverIndex = 204,
                Index = 203,
                Library = Libraries.Title,
                Location = new Point(160, 123),
                Parent = Window,
                PressedIndex = 205,
            };
            CancelButton.Click += new EventHandler(DisposeDialog);
        }
        void MirInputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (OKButton != null & !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyChar == (char)Keys.Escape)
            {
                if (CancelButton != null & !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(EventArgs.Empty);
                e.Handled = true;
            }
        }
        void DisposeDialog(object sender, EventArgs e)
        {
            Dispose();
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
                if (CancelButton != null & !CancelButton.IsDisposed)
                    CancelButton.InvokeMouseClick(EventArgs.Empty);
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                if (OKButton != null & !OKButton.IsDisposed)
                    OKButton.InvokeMouseClick(EventArgs.Empty);

            }
            e.Handled = true;
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
