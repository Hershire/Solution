using System;
using System.Drawing;
using System.Windows.Forms;
using Client.MirGraphics;
using System.Linq;


namespace Client.MirControls
{
    class MirTextBox : MirControl
    {
        #region Back Color

        protected override void OnBackColorChanged()
        {
            base.OnBackColorChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                TextBox.BackColor = BackColor;
        }

        #endregion

        #region Enabled

        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                TextBox.Enabled = Enabled;
        }

        #endregion

        #region Fore Color

        protected override void OnForeColorChanged()
        {
            base.OnForeColorChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                TextBox.ForeColor = ForeColor;
        }

        #endregion

        #region Location

        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                TextBox.Location = DisplayLocation;
        }

        #endregion

        #region Max Length

        public int MaxLength
        {
            get
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    return TextBox.MaxLength;
                return -1;
            }
            set
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    TextBox.MaxLength = value;
            }
        }

        #endregion

        #region Parent

        protected override void OnParentChanged()
        {
            base.OnParentChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                OnVisibleChanged();
        }

        #endregion

        #region Password

        public bool Password
        {
            get
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    return TextBox.UseSystemPasswordChar;
                return false;
            }
            set
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    TextBox.UseSystemPasswordChar = value;
            }
        }

        #endregion

        #region Size

        protected override void OnSizeChanged()
        {
            TextBox.Size = Size;
            _Size = TextBox.Size; 
            
            if (TextBox != null && !TextBox.IsDisposed)
                base.OnSizeChanged();
        }

        #endregion

        #region TextBox

        public TextBox TextBox { get; private set; }

        #endregion

        #region Text

        public string Text
        {
            get
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    return TextBox.Text;
                return null;
            }
            set
            {
                if (TextBox != null && !TextBox.IsDisposed)
                    TextBox.Text = value;
            }
        }

        #endregion

        #region Visible

        protected override void OnVisibleChanged()
        {
            base.OnVisibleChanged();
            if (TextBox != null && !TextBox.IsDisposed)
                TextBox.Visible = Visible;
        }
        private void TextBox_VisibleChanged(object sender, EventArgs e)
        {
            
            DialogChanged();

            if (TextBox.Visible && TextBox.CanFocus)
                if (Main.This.ActiveControl == null || Main.This.ActiveControl == Main.This)
                    Main.This.ActiveControl = TextBox;

            if (!TextBox.Visible)
                if (Main.This.ActiveControl == TextBox)
                    Main.This.Focus();
        }
        private void SetFocus(object sender, EventArgs e)
        {
            if (TextBox.Visible)
                TextBox.VisibleChanged -= SetFocus;
            if (TextBox.Parent != null)
                TextBox.ParentChanged -= SetFocus;

            if (TextBox.CanFocus) TextBox.Focus();
            else if (TextBox.Visible && TextBox.Parent != null)
                Main.This.ActiveControl = TextBox;

            
        }

        #endregion

        public MirTextBox()
        {
            BackColor = Color.Black;
            TextBox = new TextBox
            {
                BackColor = BackColor,
                BorderStyle = BorderStyle.None,
                Font = new Font("Microsoft Sans Serif", 10F),
                ForeColor = ForeColor,
                Location = DisplayLocation,
                Size = Size,
                Visible = Visible,
                Tag = this,
            };
            TextBox.VisibleChanged += TextBox_VisibleChanged;
            TextBox.ParentChanged += TextBox_VisibleChanged;
            Shown += new EventHandler(MirTextBox_Shown);
        }

        void MirTextBox_Shown(object sender, EventArgs e)
        {
            TextBox.Parent = Main.This;
        }

        public void SetFocus()
        {
            if (!TextBox.Visible)
                TextBox.VisibleChanged += SetFocus;
            else if (TextBox.Parent == null)
                TextBox.ParentChanged += SetFocus;
            else
                TextBox.Focus();
        }

        public void DialogChanged()
        {
            MirMessageBox MI = null; MirInputBox MI2 = null;
            if (MirScene.ActiveScene != null && MirScene.ActiveScene.Controls.Count > 0)
            {
                MI = (MirMessageBox)MirScene.ActiveScene.Controls.FirstOrDefault(O => O is MirMessageBox);
                MI2 = (MirInputBox)MirScene.ActiveScene.Controls.FirstOrDefault(O => O is MirInputBox);
                //MirScene.ActiveScene.Controls[MirScene.ActiveScene.Controls.Count - 1] as MirInputBox;
            }
            if (TextBox.InvokeRequired)
            {
                TextBox.Invoke((MethodInvoker)(() =>
                {
                    if ((MI != null && MI.Window != Parent) || (MI2 != null && MI2.Window != Parent))
                        TextBox.Visible = false;
                    else
                        TextBox.Visible = Visible && TextBox.Parent != null;
                }));
            }
            else
            {
                if ((MI != null && MI.Window != Parent) || (MI2 != null && MI2.Window != Parent))
                    TextBox.Visible = false;
                else
                    TextBox.Visible = Visible && TextBox.Parent != null;
            }
        }

        /*
        protected override void DrawBorder()
        {
            if (TextBox.Visible)
                base.DrawBorder();
        }*/

        public override void OnMouseClick(MouseEventArgs e)
        {
            return;
        }
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            return;
        }
        public override void OnMouseEnter()
        {
            return;
        }
        public override void OnMouseLeave()
        {
            return;
        }
        public override void OnMouseMove(MouseEventArgs e)
        {
            return;
        }
        public override void OnMouseDown(MouseEventArgs e)
        {
            return;
        }
        public override void OnMouseUp(MouseEventArgs e)
        {
            return;
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (Main.This.InvokeRequired)
                    Main.This.Invoke(new Action(DisposeTextBox));
                else
                    DisposeTextBox();
            }

            base.Dispose(Disposing);
        }

        private void DisposeTextBox()
        {
            if (!TextBox.IsDisposed)
                TextBox.Dispose();
        }

        #endregion
    }
}
