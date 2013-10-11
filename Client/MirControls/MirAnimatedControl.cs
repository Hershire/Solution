using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.MirControls
{
    class MirAnimatedControl : MirImageControl
    {
        public static List<MirAnimatedControl> Animations = new List<MirAnimatedControl>();

        #region Animated

        private bool _Animated;
        public event EventHandler AnimatedChanged;
        public bool Animated
        {
            get { return _Animated; }
            set
            {
                if (_Animated == value) return;
                _Animated = value;
                NextOffSet = Main.Time + _FadeInDelay;
                OnAnimatedChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnAnimatedChanged(EventArgs e)
        {
            if (AnimatedChanged != null)
                AnimatedChanged.Invoke(this, e);
        }

        #endregion

        #region Animation Count

        private int _AnimationCount;
        public event EventHandler AnimationCountChanged;
        public virtual int AnimationCount
        {
            get { return _AnimationCount; }
            set
            {
                if (_AnimationCount == value) return;
                _AnimationCount = value;
                OnAnimationCountChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnAnimationCountChanged(EventArgs e)
        {
            if (AnimationCountChanged != null)
                AnimationCountChanged.Invoke(this, e);
        }

        #endregion

        #region Animation Delay

        private long _AnimationDelay;
        public event EventHandler AnimationDelayChanged;
        public long AnimationDelay
        {
            get { return _AnimationDelay; }
            set
            {
                if (_AnimationDelay == value) return;
                _AnimationDelay = value;
                OnAnimationDelayChanged();
            }
        }
        protected virtual void OnAnimationDelayChanged()
        {
            if (AnimationDelayChanged != null)
                AnimationDelayChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region FadeIn

        private long NextFadeTime;
        private bool _FadeIn;
        public event EventHandler FadeInChanged;
        public bool FadeIn
        {
            get { return _FadeIn; }
            set
            {
                if (_FadeIn == value) return;
                NextFadeTime = Main.Time + _FadeInDelay;
                _FadeIn = value;
                OnFadeInChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnFadeInChanged(EventArgs e)
        {
            if (FadeInChanged != null)
                FadeInChanged.Invoke(this, e);
        }

        #endregion

        #region FadeIn Rate

        private float _FadeInRate;
        public event EventHandler FadeInRateChanged;
        public virtual float FadeInRate
        {
            get { return _FadeInRate; }
            set
            {
                if (_FadeInRate == value) return;
                _FadeInRate = value;
                OnFadeInRateChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnFadeInRateChanged(EventArgs e)
        {
            if (FadeInRateChanged != null)
                FadeInRateChanged.Invoke(this, e);
        }

        #endregion

        #region FadeIn Delay

        private long _FadeInDelay;
        public event EventHandler FadeInDelayChanged;
        public long FadeInDelay
        {
            get { return _FadeInDelay; }
            set
            {
                if (_FadeInDelay == value) return;
                _FadeInDelay = value;
                OnFadeInDelayChanged();
            }
        }
        protected virtual void OnFadeInDelayChanged()
        {
            if (FadeInDelayChanged != null)
                FadeInDelayChanged.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Events

        public event EventHandler AfterAnimation;

        #endregion

        #region Loop

        private bool _Loop;
        public event EventHandler LoopChanged;
        public bool Loop
        {
            get { return _Loop; }
            set
            {
                if (_Loop == value) return;
                _Loop = value;
                OnLoopChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnLoopChanged(EventArgs e)
        {
            if (LoopChanged != null)
                LoopChanged.Invoke(this, e);
        }

        #endregion

        #region OffSet

        private int _OffSet;
        public event EventHandler OffSetChanged;
        public virtual int OffSet
        {
            protected get { return _OffSet; }
            set
            {
                if (_OffSet == value) return;
                _OffSet = value;
                OnOffSetChanged(EventArgs.Empty);
            }
        }
        protected virtual void OnOffSetChanged(EventArgs e)
        {
            OnIndexChanged();
            if (OffSetChanged != null)
                OffSetChanged.Invoke(this, e);
        }
        private long NextOffSet;

        #endregion

        public override int Index
        {
            get { return base.Index + OffSet; }
            set { base.Index = value; }
        }

        public MirAnimatedControl()
        {
            _AnimationCount = 0;
            _Loop = true;
            _AnimationDelay = 0;
            _FadeIn = false;
            _FadeInRate = 0;
            _FadeInDelay = 0;
            NextFadeTime = Main.Time;
            NextOffSet = Main.Time;
            Animations.Add(this);
        }

        public void UpdateOffSet()
        {
            if (_FadeIn && Main.Time > NextFadeTime)
            {
                if ((Opacity += _FadeInRate) > 1F)
                    _FadeIn = false;

                NextFadeTime += _FadeInDelay;
            }

            if (!Visible || !_Animated || _AnimationDelay == 0 || _AnimationCount == 0) return;

            if (Main.Time < NextOffSet) return;

            NextOffSet += _AnimationDelay;

            
            if (++OffSet < _AnimationCount) return;

            EventHandler Temp = AfterAnimation;
            AfterAnimation = null;

            if (!Loop)
                Animated = false;
            else
                OffSet = 0;

            if (Temp != null)
                Temp.Invoke(this, EventArgs.Empty);
        }

        #region Disposable

        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                AnimatedChanged = null;
                _Animated = false;

                AnimationCountChanged = null;
                _AnimationCount = 0;

                AnimationDelayChanged = null;
                _AnimationDelay = 0;

                AfterAnimation = null;

                LoopChanged = null;
                _Loop = false;

                OffSetChanged = null;
                _OffSet = 0;

                NextOffSet = 0;

                Animations.Remove(this);
            }

            base.Dispose(Disposing);
        }

        #endregion
    }
}
