using System;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Client.MirSound;
using System.Windows.Forms;

namespace Client.MirControls
{
    class MirButton : MirImageControl
    {
        #region Hover Index

        private int _HoverIndex;
        public event EventHandler HoverIndexChanged;
        public int HoverIndex
        {
            get { return _HoverIndex; }
            set
            {
                if (_HoverIndex == value) return;
                _HoverIndex = value;
                OnHoverIndexChanged();
            }
        }
        private void OnHoverIndexChanged()
        {
            if (HoverIndexChanged != null)
                HoverIndexChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Label

        private MirLabel Label;

        #endregion

        #region Index
        private int PreviousIndex;
        public override int Index
        {
            get
            {
                if (!Enabled)
                {
                    if (PreviousIndex != base.Index) 
                    PreviousIndex = base.Index;
                    return PreviousIndex;
                }

                if (_PressedIndex >= 0 && ActiveControl == this && MouseControl == this)
                {
                    if (PreviousIndex != _PressedIndex) 
                    PreviousIndex = _PressedIndex;
                    return PreviousIndex;
                }

                if (_HoverIndex >= 0 && MouseControl == this)
                {
                    if (PreviousIndex != _HoverIndex) 
                    PreviousIndex = _HoverIndex;
                    return PreviousIndex;
                }

                if (PreviousIndex != base.Index) 
                PreviousIndex = base.Index;
                return PreviousIndex;
            }
            set { base.Index = value; }
        }
        #endregion

        #region Pressed Index

        private int _PressedIndex;
        public event EventHandler PressedIndexChanged;
        public int PressedIndex
        {
            get { return _PressedIndex; }
            set
            {
                if (_PressedIndex == value) return;
                _PressedIndex = value;
                OnPressedIndexChanged();
            }
        }
        private void OnPressedIndexChanged()
        {
            if (PressedIndexChanged != null)
                PressedIndexChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Size
        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            if (Label != null && !Label.IsDisposed)
                Label.Size = Size;
        }
        #endregion

        #region Text

        public string Text
        {
            get
            {
                if (Label != null && !Label.IsDisposed)
                    return Label.Text;
                return null;
            }
            set
            {
                if (Label != null && !Label.IsDisposed)
                    Label.Text = value;
            }
        }

        #endregion

        #region Font Colour

        public Color FontColor
        {
            get
            {
                if (Label != null && !Label.IsDisposed)
                    return Label.ForeColor;
                return Color.Empty;
            }
            set
            {
                if (Label != null && !Label.IsDisposed)
                    Label.ForeColor = value;
            }
        }

        #endregion

        public MirButton()
        {
            _HoverIndex = -1;
            _PressedIndex = -1;
            _Sound = SoundList.ClickB;

            Label = new MirLabel
            {
                DrawFormat = DrawTextFormat.Center | DrawTextFormat.VerticalCenter,
                NotControl = true,
                Parent = this,
            };
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                HoverIndexChanged = null;
                _HoverIndex = 0;

                if (Label != null && !Label.IsDisposed)
                    Label.Dispose();
                Label = null;

                PressedIndexChanged = null;
                _PressedIndex = 0;
            }

            base.Dispose(Disposing);
        }

        #endregion
    }
}
