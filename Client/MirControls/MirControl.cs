using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Library;
using Client.MirGraphics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Client.MirSound;

namespace Client.MirControls
{
    class MirControl : IDisposable
    {
        public static MirControl ActiveControl        { get; private set; }
        public static MirControl MouseControl        { get; private set; }

        public virtual Point DisplayLocation
        {
            get { return Parent == null ? Location : Functions.PointA(Parent.DisplayLocation, Location); }
        }
        public Rectangle DisplayRectangle
        {
            get { return new Rectangle(DisplayLocation, Size); }
        }

        #region Back Colour

        protected Color _BackColor;
        public event EventHandler BackColorChanged;
        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                if (_BackColor == value) return;
                _BackColor = value;
                OnBackColorChanged();
            }
        }
        protected virtual void OnBackColorChanged()
        {
            TextureValid = false;
            if (BackColorChanged != null)
                BackColorChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Border

        private bool _Border;
        public event EventHandler BorderChanged;
        private Rectangle BorderRectangle;
        private Vector2[] _BorderInfo;

        protected Vector2[] BorderInfo
        {
            get
            {
                if (Size == Size.Empty) return null;

                if (BorderRectangle != DisplayRectangle)
                {

                    _BorderInfo = new Vector2[]
                    {
                        new Vector2(DisplayRectangle.Left -1, DisplayRectangle.Top - 1),
                        new Vector2(DisplayRectangle.Right, DisplayRectangle.Top - 1),

                        new Vector2(DisplayRectangle.Left -1, DisplayRectangle.Top - 1),
                        new Vector2(DisplayRectangle.Left -1, DisplayRectangle.Bottom),

                        new Vector2(DisplayRectangle.Left -1, DisplayRectangle.Bottom),
                        new Vector2(DisplayRectangle.Right, DisplayRectangle.Bottom),

                        new Vector2(DisplayRectangle.Right, DisplayRectangle.Top - 1),
                        new Vector2(DisplayRectangle.Right, DisplayRectangle.Bottom)
                    };

                    BorderRectangle = DisplayRectangle;
                }
                return _BorderInfo;
            }
        }
        public virtual bool Border
        {
            get { return _Border; }
            set
            {
                if (_Border == value) return;
                _Border = value;
                OnBorderChanged();
            }
        }
        private void OnBorderChanged()
        {
            
            if (BorderChanged != null)
                BorderChanged.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Border Colour

        private Color _BorderColor;
        public event EventHandler BorderColorChanged;
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                if (_BorderColor == value) return;
                _BorderColor = value;
                OnBorderColorChanged();
            }
        }
        private void OnBorderColorChanged()
        {
            
            if (BorderColorChanged != null)
                BorderColorChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Control Texture

        private bool _TextureValid, _DrawControlTexutre;
        protected bool TextureValid
        {
            get { return _TextureValid; }
            set
            {
                if (_TextureValid == value) return;
                _TextureValid = value;
                
            }
        }
        public bool DrawControlTexture
        {
            get { return _DrawControlTexutre; }
            set
            {
                if (_DrawControlTexutre == value) return;
                _DrawControlTexutre = value;
                
            }
        }
        protected Texture ControlTexture;
        protected virtual bool CreateTexture()
        {
            if (Size == Size.Empty) return false;

            if (ControlTexture != null && !ControlTexture.Disposed)
                ControlTexture.Dispose();

            ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
            ControlTexture.Disposing += ControlTexture_Disposing;

            Surface OldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(ControlTexture.GetSurfaceLevel(0));
            DXManager.Device.Clear(ClearFlags.Target, BackColor, 0, 0);
            DXManager.SetSurface(OldSurface);

            TextureValid = true;
            return true;
        }
        protected void ControlTexture_Disposing(object sender, EventArgs e)
        {
            ControlTexture = null;
            TextureValid = false;
        }

        #endregion

        #region Controls

        public List<MirControl> Controls { get; private set; }
        public event EventHandler ControlAdded, ControlRemoved;
        private void AddControl(MirControl Control)
        {
            if (Controls == null) Controls = new List<MirControl>();

            Controls.Add(Control);
            OnControlAdded();
        }
        public void InsertControl(int Index, MirControl Control)
        {
            if (Controls == null) Controls = new List<MirControl>();

            if (Control.Parent != this)
            {
                Control.Parent = null;
                Control._Parent = this;
            }

            if (Index >= Controls.Count)
                Controls.Add(Control);
            else
            {
                Controls.Insert(Index, Control);
                OnControlAdded();
            }


        }
        private void RemoveControl(MirControl Control)
        {
            Controls.Remove(Control);
            OnControlRemoved();

            if (Controls.Count == 0) Controls = null;
        }
        private void OnControlAdded()
        {
            
            if (ControlAdded != null)
                ControlAdded.Invoke(this, EventArgs.Empty);
        }
        private void OnControlRemoved()
        {
            
            if (ControlRemoved != null)
                ControlRemoved.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Enabled

        protected bool _Enabled;
        public event EventHandler EnabledChanged;
        public bool Enabled
        {
            internal get { return Parent == null ? _Enabled : Parent.Enabled && _Enabled; }
            set
            {
                if (_Enabled == value) return;
                _Enabled = value;
                OnEnabledChanged();
            }
        }
        protected virtual void OnEnabledChanged()
        {
            if (EnabledChanged != null)
                EnabledChanged.Invoke(this, EventArgs.Empty);

            if (!Enabled && ActiveControl == this)
                ActiveControl.Deactivate();

            if (Controls != null)
                for (int I = 0; I < Controls.Count; I++)
                    Controls[I].OnEnabledChanged();
        }


        #endregion

        #region Events

        protected bool HasShown;
        public event EventHandler BeforeDraw, AfterDraw, MouseEnter, MouseLeave, Shown;
        public event EventHandler Click, DoubleClick, MouseMove, MouseDown, MouseUp;
        public event KeyEventHandler KeyDown, KeyUp;
        public event KeyPressEventHandler KeyPress;

        #endregion

        #region Fore Colour

        protected Color _ForeColor;
        public event EventHandler ForeColorChanged;
        public Color ForeColor
        {
            get { return _ForeColor; }
            set
            {
                if (_ForeColor == value) return;
                _ForeColor = value;
                OnForeColorChanged();
            }
        }
        protected virtual void OnForeColorChanged()
        {
            TextureValid = false;
            if (ForeColorChanged != null)
                ForeColorChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Location

        private Point _Location;
        public event EventHandler LocationChanged;
        public Point Location
        {
            get { return _Location; }
            set
            {
                if (_Location == value) return;
                _Location = value;
                OnLocationChanged();
            }
        }
        protected virtual void OnLocationChanged()
        {
            
            if (LocationChanged != null)
                LocationChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Modal

        private bool _Modal;
        public event EventHandler ModalChanged;
        public bool Modal
        {
            get { return _Modal; }
            set
            {
                if (_Modal == value) return;
                _Modal = value;
                OnModalChanged();
            }
        }
        private void OnModalChanged()
        {
            if (ModalChanged != null)
                ModalChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Movable

        private bool _Movable;
        protected bool Moving;
        private Point MovePoint;
        public event EventHandler MovableChanged;
        public bool Movable
        {
            get { return _Movable; }
            set
            {
                if (_Movable == value) return;
                _Movable = value;
                OnMovableChanged();
            }
        }
        private void OnMovableChanged()
        {
            if (MovableChanged != null)
                MovableChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Not Control

        private bool _NotControl;
        public event EventHandler NotControlChanged;
        public bool NotControl
        {
            private get { return _NotControl; }
            set
            {
                if (_NotControl == value) return;
                _NotControl = value;
                OnNotControlChanged();
            }
        }
        private void OnNotControlChanged()
        {
            if (NotControlChanged != null)
                NotControlChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Opacity

        protected float _Opacity;
        public event EventHandler OpacityChanged;
        public float Opacity
        {
            get { return _Opacity; }
            set
            {
                if (_Opacity == value) return;

                if (value > 1F) value = 1F;
                if (value < 0F) value = 0;

                _Opacity = value;
                OnOpacityChanged();
            }
        }
        private void OnOpacityChanged()
        {
            
            if (OpacityChanged != null)
                OpacityChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Parent

        private MirControl _Parent;
        public event EventHandler ParentChanged;
        public MirControl Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent == value) return;
                if (_Parent != null) _Parent.RemoveControl(this);
                _Parent = value;
                if (_Parent != null) _Parent.AddControl(this);
                OnParentChanged();
            }
        }
        protected virtual void OnParentChanged()
        {
            
            OnLocationChanged();
            if (ParentChanged != null)
                ParentChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Size

        protected Size _Size;
        public event EventHandler SizeChanged;
        public virtual Size Size
        {
            get { return _Size; }
            set
            {
                if (_Size == value) return;
                _Size = value;
                OnSizeChanged();
            }
        }
        protected virtual void OnSizeChanged()
        {
            TextureValid = false;

            if (SizeChanged != null)
                SizeChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Sound

        protected SoundList _Sound;
        public event EventHandler SoundChanged;
        public SoundList Sound
        {
            private get { return _Sound; }
            set
            {
                if (_Sound == value) return;
                _Sound = value;
                OnSoundChanged();
            }
        }
        private void OnSoundChanged()
        {
            if (SoundChanged != null)
                SoundChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Sort

        private bool _Sort;
        public event EventHandler SortChanged;
        public bool Sort
        {
            get { return _Sort; }
            set
            {
                if (_Sort == value) return;
                _Sort = value;
                OnSortChanged();
            }
        }
        private void OnSortChanged()
        {
            
            if (SortChanged != null)
                SortChanged.Invoke(this, EventArgs.Empty);
        }
        private void TrySort()
        {
            if (Parent == null) return;

            if (Sort)
            {
                Parent.Controls.Remove(this);
                Parent.Controls.Add(this);
            }

            Parent.TrySort();
        }

        #endregion

        #region Visible
        protected bool _Visible;
        public event EventHandler VisibleChanged;
        public virtual bool Visible
        {
            get { return Parent == null ? _Visible : Parent.Visible && _Visible; }
            set
            {
                if (_Visible == value) return;
                _Visible = value;
                OnVisibleChanged();
            }
        }
        protected virtual void OnVisibleChanged()
        {
            
            if (VisibleChanged != null)
                VisibleChanged.Invoke(this, EventArgs.Empty);

            Moving = false;
            MovePoint = Point.Empty;

            if (Sort && Parent != null)
            {
                Parent.Controls.Remove(this);
                Parent.Controls.Add(this);
            }

            if (!Visible)
            {
                Dehighlight();
                if (ActiveControl == this)
                    ActiveControl = null;
            }
            else
                if (IsMouseOver(Main.PointToC(Cursor.Position)))
                    Highlight();


            if (Controls != null)
                for (int I = 0; I < Controls.Count; I++)
                    Controls[I].OnVisibleChanged();
        }
        #endregion


        public MirControl()
        {
            BackColor = Color.Transparent;
            Enabled = true;
            ForeColor = Color.White;
            Opacity = 1F;
            Visible = true;
        } 

        internal virtual void Draw()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!Visible) return;

            BeforeDrawControl();
            DrawControl();
            DrawChildControls();
            DrawBorder();
            AfterDrawControl();

            if (!HasShown)
            {
                if (Shown != null)
                    Shown.Invoke(this, EventArgs.Empty);
                HasShown = true;
            }
        }

        protected virtual void Deactivate()
        {
            if (ActiveControl == this) ActiveControl = null;
        }
        protected virtual void Activate()
        {
            if (ActiveControl == this) return;

            if (ActiveControl != null) ActiveControl.Deactivate();

            ActiveControl = this;
        }

        protected virtual void Dehighlight()
        {
            if (MouseControl == this)
            {
                MouseControl.OnMouseLeave();
                MouseControl = null;
            }
        }
        protected virtual void Highlight()
        {
            if (MouseControl == this) return;

            if (MouseControl != null) MouseControl.Dehighlight();
            OnMouseEnter();
            MouseControl = this;
        }

        protected virtual void BeforeDrawControl()
        {
            if (BeforeDraw != null)
                BeforeDraw.Invoke(this, EventArgs.Empty);
        }
        protected virtual void DrawControl()
        {
            if (!DrawControlTexture) return;

            if (!TextureValid || ControlTexture == null || ControlTexture.Disposed)
                if (!CreateTexture()) return;

            float OldOpacity = DXManager.Opacity;
            
            DXManager.SetOpacity(Opacity);
            DXManager.Sprite.Draw2D(ControlTexture, Point.Empty, 0F, DisplayLocation, Color.White);
            DXManager.SetOpacity(OldOpacity);
        }
        protected virtual void DrawChildControls()
        {
            if (Controls != null)
                for (int I = 0; I < Controls.Count; I++)
                    if (Controls[I] != null) Controls[I].Draw();
        }
        protected virtual void DrawBorder()
        {
            if (!Border || BorderInfo == null) return;
            DXManager.Sprite.Flush();
            DXManager.Line.Draw(BorderInfo, _BorderColor);
        }
        protected virtual void AfterDrawControl()
        {
            if (AfterDraw != null)
                AfterDraw.Invoke(this, EventArgs.Empty);
        }

        public virtual bool IsMouseOver(Point P)
        {
            return (Visible && (DisplayRectangle.Contains(P) || Moving || Modal)) && !NotControl;
        }

        public virtual void OnMouseEnter()
        {
            if (!_Enabled) return;

            if (MouseEnter != null)
                MouseEnter.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseLeave()
        {
            if (!_Enabled) return;                       

            if (MouseLeave != null)
                MouseLeave.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnMouseClick(MouseEventArgs e)
        {
            if (!Enabled) return;


            if (Sound != SoundList.None)
                SoundManager.PlaySound(Sound, false);

            if (Click != null)
                InvokeMouseClick(e);
        }
        public void InvokeMouseClick(EventArgs e)
        {
            Click.Invoke(this, e);
        }
        public virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!Enabled) return;

            if (DoubleClick != null)
            {
                if (Sound != SoundList.None)
                    SoundManager.PlaySound(Sound, false);
                InvokeMouseDoubleClick(e);
            }
            else
                OnMouseClick(e);
        }
        public void InvokeMouseDoubleClick(EventArgs e)
        {
            DoubleClick.Invoke(this, e);
        }
        public virtual void OnMouseMove(MouseEventArgs e)
        {
            if (!_Enabled) return;


            if (Moving)
            {
                Point TempPoint = e.Location;
                if (TempPoint.Y >= Main.This.ClientSize.Height)
                    TempPoint.Y = Main.This.ClientSize.Height - 1;
                if (TempPoint.X >= Main.This.ClientSize.Width)
                    TempPoint.X = Main.This.ClientSize.Width - 1;
                if (TempPoint.X < 0) TempPoint.X = 0;
                if (TempPoint.Y < 0) TempPoint.Y = 0;
                Location = Functions.PointS(TempPoint, MovePoint);
                return;
            }

            if (Controls != null)
                for (int I = Controls.Count - 1; I >= 0; I--)
                    if (Controls[I].IsMouseOver(e.Location))
                    {
                        Controls[I].OnMouseMove(e);
                        return;
                    }
            
          /*  if (MouseControl != this)
            {
                if (MouseControl != null)
                    MouseControl.OnMouseLeave();
                OnMouseEnter();
            }

            MouseControl = this;*/

            Highlight();

            if (MouseMove != null)
                MouseMove.Invoke(this, e);
        }
        public virtual void OnMouseDown(MouseEventArgs e)
        {
            if (!_Enabled) return;

            ActiveControl = this;
            
            TrySort();

            if (_Movable)
            {
                Moving = true;
                MovePoint = Functions.PointS(e.Location, Location);
            }

            if (MouseDown != null)
                MouseDown.Invoke(this, e);
        }
        public virtual void OnMouseUp(MouseEventArgs e)
        {
            if (!_Enabled) return;

            if (Moving)
            {
                Moving = false;
                MovePoint = Point.Empty;
            }

            ActiveControl = null;

            if (MouseUp != null)
                MouseUp.Invoke(this, e);
        }

        public virtual void OnKeyPress(KeyPressEventArgs e)
        {
            if (!_Enabled) return;

            if (Controls != null)
                for (int I = Controls.Count - 1; I >= 0; I--)
                    if (e.Handled) return;
                    else Controls[I].OnKeyPress(e);

            if (KeyPress != null)
            {
                KeyPress.Invoke(this, e);
                return;
            }
        }
        public virtual void OnKeyDown(KeyEventArgs e)
        {
            if (!_Enabled) return;

            if (Controls != null)
                for (int I = Controls.Count - 1; I >= 0; I--)
                    if (e.Handled) return;
                    else Controls[I].OnKeyDown(e);

            if (KeyDown != null)
            {
                KeyDown.Invoke(this, e);
                return;
            }
        }
        public virtual void OnKeyUp(KeyEventArgs e)
        {
            if (!_Enabled) return;

            if (Controls != null)
                for (int I = Controls.Count - 1; I >= 0; I--)
                    if (e.Handled) return;
                    else Controls[I].OnKeyUp(e);

            if (KeyUp != null)
            {
                KeyUp.Invoke(this, e);
                return;
            }
        }
        
        #region Disposable

        public bool IsDisposed { get; private set; }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {

                BackColorChanged = null;
                _BackColor = Color.Empty;

                BorderChanged = null;
                _Border = false;
                BorderRectangle = Rectangle.Empty;
                _BorderInfo = null;

                BorderColorChanged = null;
                _BorderColor = Color.Empty;

                DrawControlTexture = false;
                if (ControlTexture != null && !ControlTexture.Disposed)
                    ControlTexture.Dispose();
                ControlTexture = null;
                TextureValid = false;

                ControlAdded = null;
                ControlRemoved = null;

                if (Controls != null)
                {
                    //Reverse because control will remove it's self from list changing list count.
                    for (int I = Controls.Count - 1; I >= 0; I--)
                    {
                        if (Controls[I] != null && !Controls[I].IsDisposed)
                            Controls[I].Dispose();
                    }

                    Controls = null;
                }
                _Enabled = false;
                EnabledChanged = null;

                HasShown = false;

                BeforeDraw = null;
                AfterDraw = null;
                Shown = null;

                Click = null;
                DoubleClick = null;
                MouseEnter = null;
                MouseLeave = null;
                MouseMove = null;
                MouseDown = null;
                MouseUp = null;

                KeyPress = null;
                KeyUp = null;
                KeyDown = null;

                ForeColorChanged = null;
                _ForeColor = Color.Empty;

                LocationChanged = null;
                _Location = Point.Empty;

                ModalChanged = null;
                _Modal = false;

                MovableChanged = null;
                MovePoint = Point.Empty;
                Moving = false;
                _Movable = false;

                NotControlChanged = null;
                _NotControl = false;

                OpacityChanged = null;
                _Opacity = 0F;

                if (Parent != null && Parent.Controls != null)
                    Parent.Controls.Remove(this);
                ParentChanged = null;
                _Parent = null;

                SizeChanged = null;
                _Size = Size.Empty;

                SoundChanged = null;
                _Sound = SoundList.None;

                VisibleChanged = null;
                _Visible = false;
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            Dispose(true);
        }

        #endregion
    }
}
