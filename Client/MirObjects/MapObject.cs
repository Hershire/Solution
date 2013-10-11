using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Drawing;
using Client.MirGraphics;
using Client.MirControls;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Client.MirScenes.Game_Scene;
using System.Windows.Forms;
using Client.MirSound;

namespace Client.MirObjects
{
    class MapObject
    {
        public static MapObject MouseObject, TargetObject;
        public static PlayerObject User = new PlayerObject();

        public Rectangle DisplayRectangle;

        public long ObjectID, AttackerID;
        public string Name;
        public Color NameColour = Color.White, DrawColour = Color.White;
        public MirLabel NameLabel;

        public bool Dead;
        public MirDirection Direction;
        public Point Location, ActualLocation, MovingOffSet, FinalDisplayLocation, MovePoint, DrawLocation;

        public int ImageIndex, Light, WeaponShape, BaseIndex, Effect;
        
        public ImageLibrary PrimaryLibrary;

        public Queue<QueuedAction> ActionList = new Queue<QueuedAction>();
        public MirAction CurrentAction;
        public long NextMotion;
        public FrameSet Frames;
        public Frame CurrentFrame;
        public int Frame;
        public bool FreezeFrame, CanDoAction, Struck, UseOffSet = true, OverWeightRun;


        protected SoundList MoveSound, AppearSound, AttackSound, StruckSound, FlinchSound, DieSound, PopupSound;

        //Stats
        public int CurrentHP, MaxHP;
        public int CurrentMP, MaxMP;
        
        public virtual bool Blocking
        {
            get { return !Dead; }
        }

        public MapObject()
        {

        }
        


        public virtual void LocationProcess()
        {
            #region Moving Off Set
            if (CurrentAction == MirAction.Walking || CurrentAction == MirAction.Running)
            {
                int M = CurrentAction == MirAction.Running ? 2 : 1;
                MovePoint = Functions.PointMove(Location, Direction, -M);

                switch (Direction)
                {
                    case MirDirection.Up:
                        MovingOffSet = new Point(0, (int)((Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                    case MirDirection.UpRight:
                        MovingOffSet = new Point((int)((-Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), (int)((Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                    case MirDirection.Right:
                        MovingOffSet = new Point((int)((-Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), 0);
                        break;
                    case MirDirection.DownRight:
                        MovingOffSet = new Point((int)((-Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), (int)((-Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                    case MirDirection.Down:
                        MovingOffSet = new Point(0, (int)((-Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                    case MirDirection.DownLeft:
                        MovingOffSet = new Point((int)((Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), (int)((-Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                    case MirDirection.Left:
                        MovingOffSet = new Point((int)((Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), 0);
                        break;
                    case MirDirection.UpLeft:
                        MovingOffSet = new Point((int)((Globals.CellWidth * M / (float)(CurrentFrame.Count)) * (Frame + 1)), (int)((Globals.CellHeight * M / (float)(CurrentFrame.Count)) * (Frame + 1)));
                        break;
                }

                MovingOffSet = new Point(MovingOffSet.X % 2 + MovingOffSet.X, MovingOffSet.Y % 2 + MovingOffSet.Y);

                if (CurrentAction == MirAction.Walking)
                {
                    if (Frame >= 3)
                        DrawLocation = Location;
                    else
                        DrawLocation = MovePoint;
                }
                else if (CurrentAction == MirAction.Running)
                {
                    if (Frame >= 4)
                        DrawLocation = Location;
                    else if (Frame >= 2)
                        DrawLocation = Functions.PointMove(Location, Direction, -1);
                    else
                        DrawLocation = MovePoint;
                }
            }
            else
            {
                DrawLocation = Location;
                MovePoint = Location;
                MovingOffSet = Point.Empty;
            }
            #endregion

            if (PrimaryLibrary != null)
            {
                #region Display Rectangle

                DisplayRectangle.Size = PrimaryLibrary.GetSize(ImageIndex);

                DisplayRectangle.Location = new Point(
                    (MovePoint.X - MapObject.User.MovePoint.X + Settings.PlayerOffSet.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X - MovingOffSet.X,
                    (MovePoint.Y - MapObject.User.MovePoint.Y + Settings.PlayerOffSet.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y - MovingOffSet.Y);


                #endregion

                FinalDisplayLocation = Functions.PointA(DisplayRectangle.Location, PrimaryLibrary.GetOffSet(ImageIndex));
            }
            else
            {
                DisplayRectangle = Rectangle.Empty;
                FinalDisplayLocation = Point.Empty;
            }
        }
        public virtual void FrameProcess()
        {
            if (Frames == null) return;

            switch (CurrentAction)
            {
                case MirAction.Standing:
                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (ActionList.Count >= 2) Frame++;

                        NextMotion += CurrentFrame.Interval;
                        CanDoAction = true;

                        if (++Frame >= CurrentFrame.Count) Frame = 0;
                    }
                    break;
                case MirAction.Walking:
                    if (Main.CanMove)
                    {
                        if (this == MapObject.User)
                        {
                            if ((User.CurrentBagWeight > User.MaxBagWeight ||
                                User.CurrentBodyWeight > User.MaxBodyWeight) && OverWeightRun)
                            {
                                OverWeightRun = false;
                                return;
                            }
                            else OverWeightRun = true;

                            if (Frame == 1 || Frame == 4)
                                PlayStepSound();

                            if (Frame == CurrentFrame.Count - 1 && Main.NextMoveTime > Main.Time)
                                break;
                        }

                        if (ActionList.Count >= 2) Frame++;


                        if (++Frame >= CurrentFrame.Count)
                        {
                            if (FreezeFrame)
                            {
                                Frame--;
                                CanDoAction = true;
                                FreezeFrame = false;
                            }
                            else
                                DoAction(MirAction.Standing);
                        }
                    }
                    break;
                case MirAction.Running:
                    if (Main.CanMove)
                    {
                        if (this == MapObject.User)
                        {
                            if ((User.CurrentBagWeight > User.MaxBagWeight ||
                                User.CurrentBodyWeight > User.MaxBodyWeight) && OverWeightRun)
                            {
                                OverWeightRun = false;
                                return;
                            }
                            else OverWeightRun = true;

                            if (Frame == 1 || Frame == 4)
                                PlayStepSound();

                            if (Frame == CurrentFrame.Count - 1 && Main.NextMoveTime > Main.Time)
                                break;
                        }

                        if (ActionList.Count >= 2) Frame++;

                        if (++Frame >= CurrentFrame.Count)
                        {
                            if (FreezeFrame)
                            {
                                Frame--;
                                CanDoAction = true;
                                FreezeFrame = false;
                            }
                            else
                                DoAction(MirAction.Standing);
                        }
                    }
                    break;
                case MirAction.Stance:
                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (ActionList.Count >= 2) Frame++;

                        NextMotion += CurrentFrame.Interval;
                        CanDoAction = true;

                        if (++Frame >= CurrentFrame.Count)
                            DoAction(MirAction.Standing);
                    }
                    break;
                case MirAction.Attack1:
                case MirAction.Attack2:
                case MirAction.Attack3:
                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (Frame == 1) PlayAttackSound();

                        if (ActionList.Count >= 2) Frame++;

                        NextMotion += CurrentFrame.Interval;

                        if (++Frame >= CurrentFrame.Count)
                        {
                            DoAction(MirAction.Stance);
                            CanDoAction = true;
                        }
                    }
                    break;
                case MirAction.Harvest:
                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (ActionList.Count >= 2) Frame++;

                        NextMotion += CurrentFrame.Interval;

                        if (++Frame >= CurrentFrame.Count)
                        {
                            DoAction(MirAction.Standing);
                            CanDoAction = true;
                        }
                    }
                    break;
                case MirAction.Struck:
                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (Frame == 0)
                        {
                            if (this == MapObject.User)
                            {
                                Main.AllowRun = false;
                                Main.StruckRunTime = Main.Time + 2000;
                            }

                            MapObject Temp = MapLayer.ObjectList.FirstOrDefault(O => O.ObjectID == AttackerID);
                            PlayStruckSound(Temp == null ? -1 : Temp.WeaponShape);

                        }

                        if (Frame == 1) PlayFlinchSound();

                        NextMotion += CurrentFrame.Interval;

                        if (++Frame >= CurrentFrame.Count)
                        {
                            DoAction(MirAction.Stance);
                            CanDoAction = true;
                        }
                    }
                    break;
                case MirAction.Die:
                    if (this == MapObject.User) Main.AllowRun = false;

                    if (Main.Timer.ElapsedMilliseconds >= NextMotion)
                    {
                        if (Frame == 0)
                            PlayDieSound();

                        NextMotion += CurrentFrame.Interval;

                        if (++Frame >= CurrentFrame.Count)
                            DoAction(MirAction.Dead);
                    }
                    break;
                case MirAction.Dead:
                    //no idea
                    break;
                case MirAction.Skeleton:

                    break;
            }
            if (Dead && ActionList.Count > 0)
            {
                QueuedAction Q = ActionList.Dequeue();
                if (Q.Action == MirAction.Skeleton)
                    DoAction(Q.Action);
            }
            if (CanDoAction && (ActionList.Count > 0 || Struck))
            {
                if (Struck)
                {
                    DoAction(MirAction.Struck);
                    Struck = false;
                }
                else if ((ActionList.Peek().Action != MirAction.Walking && ActionList.Peek().Action != MirAction.Running) || Main.CanMove)
                {
                    QueuedAction Q = ActionList.Dequeue();
                    if (Q.Action == MirAction.Walking || Q.Action == MirAction.Running)
                    {
                        Direction = Functions.DirectionFromPoint(Location, Q.Location);
                        if (Location != Q.Location)
                        {
                            DoAction(Q.Action);
                            Location = Functions.PointMove(Location, Direction, Q.Action == MirAction.Walking ? 1 : 2);
                        }
                    }
                    else
                    {
                        if (Q.Action != MirAction.Die && Q.Action != MirAction.Dead && Q.Action != MirAction.Skeleton)
                            Direction = Q.Direction;
                        DoAction(Q.Action);
                    }
                }
            }

            ImageIndex = CurrentFrame.Start + (CurrentFrame.OffSet * (byte)Direction) + Frame + BaseIndex;

        }

        public virtual void DoAction(MirAction Action)
        {
            if (Frames == null) return;

            CurrentAction = Action;
            Frame = 0;
            CanDoAction = false;
            FreezeFrame = true;

            switch (Action)
            {
                case MirAction.Turn:
                    PlayAppearSound();
                    CurrentAction = MirAction.Standing;
                    CurrentFrame = Frames.Stand;
                    CanDoAction = true;
                    break;
                case MirAction.Standing:
                    CurrentFrame = Frames.Stand;
                    CanDoAction = true;
                    break;
                case MirAction.Harvest:
                    CurrentFrame = Frames.Harvest ?? Frames.Stand;
                    break;
                case MirAction.Walking:
                    CurrentFrame = Frames.Walk;
                    //if (this == User) Main.NextMoveTime = Main.Timer.ElapsedMilliseconds + 500;
                    break;
                case MirAction.Stance:
                    CurrentFrame = Frames.Stance ?? Frames.Stand;
                    break;
                case MirAction.Attack1:
                    CurrentFrame = Frames.Attack1;
                    if (this == User) Main.AttackTime = Main.Timer.ElapsedMilliseconds + User.AttackSpeed;
                    break;
                case MirAction.Attack2:
                    CurrentFrame = Frames.Attack2;
                    if (this == User) Main.AttackTime = Main.Timer.ElapsedMilliseconds + User.AttackSpeed;
                    break;
                case MirAction.Attack3:
                    CurrentFrame = Frames.Attack3;
                    if (this == User) Main.AttackTime = Main.Timer.ElapsedMilliseconds + User.AttackSpeed;
                    break;
                case MirAction.Struck:
                    CurrentFrame = Frames.Struck;
                    break;
                case MirAction.Die:
                    CurrentFrame = Frames.Die;
                    Dead = true;
                    break;
                case MirAction.Dead:
                    CurrentFrame = Frames.Dead;
                    break;
                case MirAction.Skeleton:
                    CurrentFrame = Frames.Skeleton ?? Frames.Dead;
                    break;
                case MirAction.Running:
                    CurrentFrame = Frames.Run ?? Frames.Walk;
                    //if (this == User) Main.NextMoveTime = Main.Timer.ElapsedMilliseconds + 500;
                    break;
            }

            NextMotion = Main.Timer.ElapsedMilliseconds + CurrentFrame.Interval;
            ImageIndex = CurrentFrame.Start + (CurrentFrame.OffSet * (byte)Direction) + Frame + BaseIndex;
            LocationProcess();
        }
        public void QueueAction(MirAction Action, MirDirection D)
        {
            ActionList.Enqueue(new QueuedAction
            {
                Action = Action,
                Direction = D
            });
        }
        public void QueueAction(MirAction Action, Point L)
        {
            ActionList.Enqueue(new QueuedAction
            {
                Action = Action,
                Location = L
            });
        }

        public virtual void DrawName()
        {
            if (NameLabel == null || NameLabel.IsDisposed)
                CreateLabel();

            if (NameLabel != null)
            {
                NameLabel.Text = Name;
                Size S = PrimaryLibrary.GetSize(ImageIndex);
                NameLabel.Location = new Point(DisplayRectangle.X + (48 - NameLabel.Size.Width) / 2, DisplayRectangle.Y - (32 - NameLabel.Size.Height / 2) + (Dead ? 35 : 0));
                NameLabel.Draw();
            }
        }
        public virtual void CreateLabel()
        {
            NameLabel = new MirControls.MirLabel
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = NameColour,
                OutLine = true,
                OutLineColor = Color.Black,
                Text = Name,
            };
        }
        public virtual bool MouseOver(Point P)
        {
            return PrimaryLibrary.VisiblePixel(ImageIndex, Functions.PointS(P, FinalDisplayLocation));
        }

        public virtual void PlayStepSound()
        {
            SoundManager.PlaySound(MoveSound, false);
        }
        public void PlayAppearSound()
        {
            SoundManager.PlaySound(AppearSound, false);
        }
        public void PlayAttackSound()
        {
            SoundManager.PlaySound(AttackSound, false);
        }
        public virtual void PlayStruckSound(int WeaponType)
        {
            if (WeaponType == -1) return;
            if (WeaponType == 1 || WeaponType == 2 || (WeaponType >= 4 && WeaponType <= 6) ||
                WeaponType == 9 || WeaponType == 10 || (WeaponType >= 13 && WeaponType <= 17) ||
                (WeaponType >= 22 && WeaponType <= 31) || (WeaponType >= 33 && WeaponType <= 37))
                StruckSound = SoundList.StruckBodySword;
            else if (WeaponType == 3 || WeaponType == 7 || WeaponType == 1)
                StruckSound = SoundList.StruckBodyAxe;
            else if (WeaponType == 8 || WeaponType == 12 || WeaponType == 18 || WeaponType == 21 || WeaponType == 32)
                StruckSound = SoundList.StruckBodyLongStick;
            else StruckSound = SoundList.StruckBodyFist;
            SoundManager.PlaySound(StruckSound, false);
        }

        public void PlayFlinchSound()
        {
            SoundManager.PlaySound(FlinchSound, false);
        }
        public void PlayDieSound()
        {
            SoundManager.PlaySound(DieSound, false);
        }


        public virtual void Remove()
        {
            if (MouseObject == this) MouseObject = null;
            if (TargetObject == this) TargetObject = null;
            if (NameLabel != null && !NameLabel.IsDisposed) NameLabel.Dispose();
            NameLabel = null;
            MapLayer.ObjectList.Remove(this);
        }

        public void DrawBlend()
        {
            DrawColour = Color.FromArgb(255, 200, 200, 200);
            DXManager.SetBlend(true);
            Draw();
            DXManager.SetBlend(false);
            DrawColour = Color.White;
        }
        public virtual void Draw()
        {
            if (FinalDisplayLocation.X >= Settings.ScreenSize.Width ||
                FinalDisplayLocation.Y >= Settings.ScreenSize.Height) return;
            if (PrimaryLibrary != null)
                PrimaryLibrary.Draw(ImageIndex, FinalDisplayLocation, DrawColour);

            if (Effect > 0)
            {
                DrawEffect();
            }
        }
        public virtual void DrawEffect()
        {

        }

    }



    class QueuedAction
    {
        public MirAction Action;
        public MirDirection Direction;
        public Point Location;
    }
}
