using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using Client.MirGraphics;
using System.Drawing;

namespace Client.MirControls
{
    class MirLabel : MirControl
    {
        #region Auto Size

        private bool _AutoSize;
        public event EventHandler AutoSizeChanged;
        public bool AutoSize
        {
            private get { return _AutoSize; }
            set
            {
                if (_AutoSize == value) return;
                _AutoSize = value;
                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
        private void OnAutoSizeChanged(EventArgs e)
        {
            TextureValid = false;
            if (AutoSizeChanged != null)
                AutoSizeChanged.Invoke(this, e);
        }

        #endregion

        #region Draw Format

        private DrawTextFormat _DrawFormat;
        public event EventHandler DrawFormatChanged;
        public DrawTextFormat DrawFormat
        {
            private get { return _DrawFormat; }
            set
            {
                if (_DrawFormat == value) return;
                _DrawFormat = value;
                OnDrawFormatChanged(EventArgs.Empty);
            }
        }
        private void OnDrawFormatChanged(EventArgs e)
        {
            TextureValid = false;
            if (DrawFormatChanged != null)
                DrawFormatChanged.Invoke(this, e);
        }

        #endregion

        #region Font

        private Font _Font;
        private Microsoft.DirectX.Direct3D.Font DXFont;
        public event EventHandler FontChanged;
        public Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font == value) return;
                _Font = value;
                OnFontChanged(EventArgs.Empty);
            }
        }
        private void OnFontChanged(EventArgs e)
        {
            TextureValid = false;

            if (DXFont != null && !DXFont.Disposed)
                DXFont.Dispose();

            if (Font != null)
                DXFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, Font);

            if (FontChanged != null)
                FontChanged.Invoke(this, e);
        }

        #endregion

        #region Out Line

        private bool _OutLine;
        public event EventHandler OutLineChanged;
        public bool OutLine
        {
            get { return _OutLine; }
            set
            {
                if (_OutLine == value) return;
                _OutLine = value;
                OnOutLineChanged(EventArgs.Empty);
            }
        }
        private void OnOutLineChanged(EventArgs e)
        {
            TextureValid = false;
            if (OutLineChanged != null)
                OutLineChanged.Invoke(this, e);
        }

        #endregion

        #region Out Line Colour

        private Color _OutLineColor;
        public event EventHandler OutLineColorChanged;
        public Color OutLineColor
        {
            get { return _OutLineColor; }
            set
            {
                if (_OutLineColor == value) return;
                _OutLineColor = value;
                OnOutLineColorChanged();
            }
        }

        private void OnOutLineColorChanged()
        {
            TextureValid = false;
            if (OutLineColorChanged != null)
                OutLineColorChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Text

        private string _Text;
        public event EventHandler TextChanged;
        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text == value) return;
                _Text = value;
                OnTextChanged(EventArgs.Empty);
            }
        }
        private void OnTextChanged(EventArgs e)
        {
            DrawControlTexture = !string.IsNullOrEmpty(Text);
            TextureValid = false;

            if (AutoSize)
            {
                Size = DXFont.MeasureString(null, Text, DrawFormat, ForeColor).Size;
                if (OutLine && Size != Size.Empty)
                    Size = new Size(Size.Width + 2, Size.Height + 2);
            }

            if (TextChanged != null)
                TextChanged.Invoke(this, e);
        }

        #endregion

        public MirLabel()
        {
            AutoSize = false;
            OutLine = true;
            OutLineColor = Color.Black;
            DrawControlTexture = true;
            DrawFormat = DrawTextFormat.None;
            Font = new Font("Microsoft Sans Serif", 10F);
            Text = string.Empty;
        }

        protected override bool CreateTexture()
        {
            if (string.IsNullOrEmpty(Text) || !Main.This.Created) return false;

            if (DXFont == null || DXFont.Disposed)
                if (Font != null)
                    DXFont = new Microsoft.DirectX.Direct3D.Font(DXManager.Device, Font);
                else
                    return false;

            if (AutoSize)
            {
                Size = DXFont.MeasureString(null, Text, DrawFormat, ForeColor).Size;
                if (OutLine && Size != Size.Empty)
                    Size = new Size(Size.Width + 2, Size.Height + 2);
            }

            if (Size == Size.Empty) return false;


            if (ControlTexture != null && !ControlTexture.Disposed)
                ControlTexture.Dispose();

            ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ControlTexture.Disposing += ControlTexture_Disposing;

            Surface OldSurface = DXManager.CurrentSurface;
            
            DXManager.SetSurface(ControlTexture.GetSurfaceLevel(0));
            DXManager.Device.Clear(ClearFlags.Target, BackColor, 0, 0);

            Point TempPoint = Point.Empty;

            if (OutLine)
            {
                TempPoint.X = 1;
                TempPoint.Y = 0;
                if (DrawFormat == DrawTextFormat.None)
                    DXFont.DrawText(null, Text, TempPoint, OutLineColor);
                else
                    DXFont.DrawText(null, Text, new Rectangle(TempPoint, Size), DrawFormat, OutLineColor);

                TempPoint.X = 0;
                TempPoint.Y = 1;
                if (DrawFormat == DrawTextFormat.None)
                    DXFont.DrawText(null, Text, TempPoint, OutLineColor);
                else
                    DXFont.DrawText(null, Text, new Rectangle(TempPoint, Size), DrawFormat, OutLineColor);

                TempPoint.X = 2;
                TempPoint.Y = 1;
                if (DrawFormat == DrawTextFormat.None)
                    DXFont.DrawText(null, Text, TempPoint, OutLineColor);
                else
                    DXFont.DrawText(null, Text, new Rectangle(TempPoint, Size), DrawFormat, OutLineColor);

                TempPoint.X = 1;
                TempPoint.Y = 2;
                if (DrawFormat == DrawTextFormat.None)
                    DXFont.DrawText(null, Text, TempPoint, OutLineColor);
                else
                    DXFont.DrawText(null, Text, new Rectangle(TempPoint, Size), DrawFormat, OutLineColor);

                TempPoint = new Point(1, 1);
            }

            if (DrawFormat == DrawTextFormat.None)
                DXFont.DrawText(null, Text, TempPoint, ForeColor);
            else
                DXFont.DrawText(null, Text, new Rectangle(TempPoint, Size), DrawFormat, ForeColor);

            DXManager.SetSurface(OldSurface);

            TextureValid = true;
            return true;
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                AutoSizeChanged = null;
                _AutoSize = false;

                DrawFormatChanged = null;
                _DrawFormat = DrawTextFormat.None;

                FontChanged = null;

                if (_Font != null)
                    _Font.Dispose();
                _Font = null;

                if (DXFont != null && !DXFont.Disposed)
                    DXFont.Dispose();
                DXFont = null;

                OutLineChanged = null;
                _OutLine = false;

                OutLineColorChanged = null;
                _OutLineColor = Color.Empty;

                TextChanged = null;
                Text = null;
            }

            base.Dispose(Disposing);
        }

        #endregion
    }
}
