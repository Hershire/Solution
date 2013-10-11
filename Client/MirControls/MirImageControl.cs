using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.MirControls;
using Client.MirGraphics;
using System.Drawing;
using Library;

namespace Client.MirControls
{
    class MirImageControl : MirControl
    {
        public override Point DisplayLocation
        {
            get
            {
                if (UseOffSet)
                    return Functions.PointA(base.DisplayLocation, Library.GetOffSet(Index));
                else return base.DisplayLocation;
            }
        }
        public Point DisplayLocationWithoutOffSet
        {
            get
            {
                return base.DisplayLocation;
            }
        }

        #region DrawImage
        private bool _DrawImage;
        public event EventHandler DrawImageChanged;
        public bool DrawImage
        {
            private get { return _DrawImage; }
            set
            {
                if (_DrawImage == value) return;
                _DrawImage = value;
                OnDrawImageChanged();
            }
        }
        private void OnDrawImageChanged()
        {
            
            if (DrawImageChanged != null)
                DrawImageChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Index

        private int _Index;
        public event EventHandler IndexChanged;
        public virtual int Index
        {
            get { return _Index; }
            set
            {
                if (_Index == value) return;
                _Index = value;
                OnIndexChanged();
            }
        }
        protected virtual void OnIndexChanged()
        {
            OnSizeChanged();
            if (IndexChanged != null)
                IndexChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Library

        private ImageLibrary _Library;
        public event EventHandler LibraryChanged;
        public ImageLibrary Library
        {
            get { return _Library; }
            set
            {
                if (_Library == value) return;
                _Library = value;
                OnLibraryChanged();
            }
        }
        protected virtual void OnLibraryChanged()
        {
            
            OnSizeChanged();
            if (LibraryChanged != null)
                LibraryChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region PixelDetect

        private bool _PixelDetect;
        public event EventHandler PixelDetectChanged;
        public bool PixelDetect
        {
            private get { return _PixelDetect; }
            set
            {
                if (_PixelDetect == value) return;
                _PixelDetect = value;
                OnPixelDetectChanged();
            }
        }
        private void OnPixelDetectChanged()
        {
            if (PixelDetectChanged != null)
                PixelDetectChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region UseOffset

        private bool _UseOffSet;
        public event EventHandler UseOffSetChanged;
        public bool UseOffSet
        {
            protected get { return _UseOffSet; }
            set
            {
                if (_UseOffSet == value) return;
                _UseOffSet = value;
                OnUseOffSetChanged();
            }
        }
        protected virtual void OnUseOffSetChanged()
        {
            OnLocationChanged();
            if (UseOffSetChanged != null)
                UseOffSetChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Size

        public override Size Size
        {
            set
            {
                base.Size = value;
            }
            get
            {
                if (Library != null && Index >= 0)
                    return Library.GetSize(Index);
                else return base.Size;
            }
        }

        #endregion

        public MirImageControl()
        {
            DrawImage = true;
            Index = -1;
        }

        protected override void DrawControl()
        {

            base.DrawControl();

            if (DrawImage && Library != null)
                Library.Draw(Index, DisplayLocation, _ForeColor, _Opacity);
        }

        public override bool IsMouseOver(Point P)
        {
            return base.IsMouseOver(P) && (!_PixelDetect || Library.VisiblePixel(Index, Functions.PointS(P, DisplayLocation)) || Moving);
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                DrawImageChanged = null;
                _DrawImage = false;

                IndexChanged = null;
                _Index = 0;

                LibraryChanged = null;
                Library = null;

                PixelDetectChanged = null;
                _PixelDetect = false;

                UseOffSetChanged = null;
                _UseOffSet = false;

            }

            base.Dispose(Disposing);
        }

        #endregion
    }
}
