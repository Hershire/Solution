using Client.MirControls;
using Client.MirNetwork;
using Client.MirObjects;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.MirScenes.Game_Scene
{
    static class GameScene
    {
        public static MirScene Scene;
        public static Point MouseLocation;
        public static MouseButtons MouseB;
        public static bool Shift, Alt, Ctrl;

        static int OutputCount = 10;
        static long OutputDuration = 5000;
        public static MirLabel[] OutputLines = new MirLabel[OutputCount];
        public static List<string> OutputMessages = new List<string>();
        public static List<long> OutputTimes = new List<long>();   

        static GameScene()
        {
            Scene = new MirScene
            {
                Size = Settings.ScreenSize
            };

            Scene.BeforeDraw += Scene_BeforeDraw;
            Scene.MouseDown += Scene_MouseDown;
            Scene.MouseUp += Scene_MouseUp;
            Scene.MouseMove += Scene_MouseMove;
            Scene.KeyDown += Scene_KeyDown;
            Scene.KeyPress += Scene_KeyPress;
            Scene.AfterDraw += Scene_AfterDraw;

            for (int I = 0; I < OutputLines.Length; I++)
                OutputLines[I] = new MirLabel
                {
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    ForeColor = Color.DarkGreen,
                    OutLine = true,
                    Location = new Point(20, 50 + I * 13),
                    Font = new Font("Microsoft Sans Serif", 8F)
                };

            MainPanel.Create();
            MiniMap.Create();
            ChatPanel.Create();
            BeltPanel.Create();
        }

        static void OperateObjects()
        {
            MapLayer.ObjectList.Sort(delegate(MapObject M1, MapObject M2)
            {
                if (M1 is ItemObject && !(M2 is ItemObject))
                    return -1;
                if (M2 is ItemObject && !(M1 is ItemObject))
                    return 1;

                int I = M2.Dead.CompareTo(M1.Dead);
                if (I == 0)
                    return M2.ObjectID.CompareTo(M1.ObjectID);
                return I;
            });

            MapObject.User.FrameProcess();

            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if (MapLayer.ObjectList[I] != MapObject.User)
                    MapLayer.ObjectList[I].FrameProcess();
            }

            MapObject.MouseObject = null;

            for (int I = MapLayer.ObjectList.Count - 1; I >= 0; I--)
                if (MapLayer.ObjectList[I] != MapObject.User && MapLayer.ObjectList[I].MouseOver(MouseLocation))
                {
                    MapObject.MouseObject = MapLayer.ObjectList[I];
                    break;
                }
        }

        static void Scene_AfterDraw(object sender, EventArgs e)
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Draw();

            if ((MirItemCell.PickedUpGold || MirItemCell.SelectedCell != null && MirItemCell.SelectedCell.Item != null) && 
                MirGraphics.Libraries.Items != null)
            {
                int Image = MirItemCell.PickedUpGold ? 116 : MirItemCell.SelectedCell.Item.Info.Image;
                Point Location = Main.This.PointToClient(Cursor.Position);
                Size ImgSize = MirGraphics.Libraries.Items.GetSize(Image);
                Location.Offset(-ImgSize.Width / 2, -ImgSize.Height / 2);

                if (Location.X + ImgSize.Width >= Settings.ScreenSize.Width)
                    Location.X = Settings.ScreenSize.Width - ImgSize.Width;

                if (Location.Y + ImgSize.Height >= Settings.ScreenSize.Height)
                    Location.Y = Settings.ScreenSize.Height - ImgSize.Height;

                MirGraphics.Libraries.Items.Draw(Image, Location, Color.White);
            }

            for (int I = 0; I < OutputLines.Length; I++)
                OutputLines[I].Draw();
        }
        static void Scene_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    ChatPanel.ChatTextBox.Visible = true;
                    ChatPanel.ChatTextBox.SetFocus();
                    if (ChatPanel.PreChat == "/")
                        ChatPanel.ChatTextBox.Text = string.Format("/{0} ", ChatPanel.LastPM);
                    else
                        ChatPanel.ChatTextBox.Text = ChatPanel.PreChat;
                    ChatPanel.ChatTextBox.TextBox.Select(ChatPanel.ChatTextBox.TextBox.TextLength, 0);
                    e.Handled = true;
                    break;
                case '/':
                    ChatPanel.ChatTextBox.Visible = true;
                    ChatPanel.ChatTextBox.SetFocus();
                    ChatPanel.ChatTextBox.Text = string.Format("/{0} ", ChatPanel.LastPM);
                    ChatPanel.ChatTextBox.TextBox.Select(ChatPanel.ChatTextBox.TextBox.TextLength, 0);
                    e.Handled = true;
                    break;
            }
        }
        static void Scene_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    ChatPanel.StartIndex--;
                    ChatPanel.UpdateChatList();
                    break;
                case Keys.Down:
                    ChatPanel.StartIndex++;
                    ChatPanel.UpdateChatList();
                    break;
                case Keys.V:
                    MiniMap.Toggle();
                    break;
                case Keys.F9:
                case Keys.I:
                    if (InventoryDialog.Window.Visible)
                        InventoryDialog.Hide();
                    else
                        InventoryDialog.Show();
                    break;
                case Keys.F10:
                case Keys.C:
                    if (CharacterDialog.Window.Visible && CharacterDialog.CharacterPage.Visible)
                        CharacterDialog.Hide();
                    else
                    {
                        CharacterDialog.Show();
                        CharacterDialog.CharacterButton_Click(null, EventArgs.Empty);
                    }
                    break;
                case Keys.F11:
                case Keys.S:
                    if (CharacterDialog.Window.Visible && CharacterDialog.SkillPage.Visible)
                        CharacterDialog.Hide();
                    else
                    {
                        CharacterDialog.Show();
                        CharacterDialog.SkillButton_Click(null, EventArgs.Empty);
                    }
                    break;
                case Keys.Escape:
                    InventoryDialog.Hide();
                    CharacterDialog.Hide();
                    break;
                case Keys.Tab:
                    if (Main.Timer.ElapsedMilliseconds > Main.PickUpTime)
                    {
                        Main.PickUpTime = Main.Timer.ElapsedMilliseconds + 200;
                        for (int I = 0; I < ItemObject.ItemList.Count; I++)
                            if (ItemObject.ItemList[I].Location == MapObject.User.Location)
                            {
                                OutBound.PickUp();
                                return;
                            }
                    }
                    break;
                case Keys.X:
                    if (!Alt) break;
                    LogOut();
                    break;
                case Keys.Q:
                    if (!Alt) break;
                    QuitGame();
                    break;
                case Keys.Z:
                    if (Ctrl) BeltPanel.Flip();
                    else
                    {
                        if (BeltPanel.Window.Visible)
                            BeltPanel.Hide();
                        else
                            BeltPanel.Show();
                    }
                    break;
                case Keys.NumPad1:
                case Keys.D1:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[0].UseItem();
                    break;
                case Keys.NumPad2:
                case Keys.D2:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[1].UseItem();
                    break;
                case Keys.NumPad3:
                case Keys.D3:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[2].UseItem();
                    break;
                case Keys.NumPad4:
                case Keys.D4:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[3].UseItem();
                    break;
                case Keys.NumPad5:
                case Keys.D5:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[4].UseItem();
                    break;
                case Keys.NumPad6:
                case Keys.D6:
                    if (Main.Time > Main.UseItemTime)
                        BeltPanel.Grid[5].UseItem();
                    break;
            }
        }        
        static void Scene_BeforeDraw(object sender, EventArgs e)
        {
            OperateObjects();
            OperateInput();
            MapObject.User.LocationProcess();
            for (int I = 0; I < MapLayer.ObjectList.Count; I++)
            {
                if (MapLayer.ObjectList[I] != MapObject.User)
                    MapLayer.ObjectList[I].LocationProcess();
            }
            UpdateOutput();
            MapLayer.Render();
        }

        static void Scene_MouseMove(object sender, EventArgs e)
        {
            MouseEventArgs ME = e as MouseEventArgs;
            if (ME == null) return;

            MouseLocation = ME.Location;
        }
        static void Scene_MouseUp(object sender, EventArgs e)
        {
            MouseEventArgs ME = e as MouseEventArgs;
            if (ME == null) return;
        }
        static void Scene_MouseDown(object sender, EventArgs e)
        {

            MouseEventArgs ME = e as MouseEventArgs;
            if (ME == null) return;

            if (ME.Button == MouseButtons.Left )
            {
                if (MirItemCell.SelectedCell != null)
                {
                    if (MirItemCell.SelectedCell.GridType == MirGridType.Equipment || MirItemCell.SelectedCell.GridType == MirGridType.Trade)
                    {
                        MirItemCell.SelectedCell = null;
                        return;
                    }

                    MirMessageBox MMBox = new MirMessageBox(
                        string.Format("Are you sure you want to drop {0}?", MirItemCell.SelectedCell.Item.Info.ItemName), 
                        MessageBoxButtons.YesNo);

                    MirItemCell Cell = MirItemCell.SelectedCell;

                    MMBox.YesButton.Click += (O, A) =>
                    {
                        OutBound.DropItem(Cell.GridType, Cell.Item.UniqueID);
                        Cell.Item = null;
                        MapObject.User.CalculateWeight();
                    };
                    MMBox.Show();

                    MirItemCell.SelectedCell = null;

                    return;
                }
                else if (MirItemCell.PickedUpGold)
                {
                    MirInputBox MIBox = new MirInputBox("How much do you want to drop?");

                    MIBox.OKButton.Click += (O, A) =>
                    {
                        int Amount;
                        if (int.TryParse(MIBox.InputTextBox.Text, out Amount) && Amount > 0 && Amount <= MapObject.User.Gold)
                        {
                            OutBound.DropGold(Amount);
                            MapObject.User.Gold -= Amount;
                        }
                        MIBox.Dispose();
                    };
                    MIBox.Show();
                    MirItemCell.PickedUpGold = false;

                    return;
                }
            }

            MouseB |= ME.Button;

            if (ME.Button == MouseButtons.Left)
            {
                if (MapObject.MouseObject != MapObject.User && MapObject.MouseObject != null && !MapObject.MouseObject.Dead)
                    MapObject.TargetObject = MapObject.MouseObject;
                else
                    MapObject.TargetObject = null;
            }
        }

        public static void OperateInput()
        {
            MirDirection Dir;
            PlayerObject User = MapObject.User;
            
            if (User.CanDoAction && MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (MapObject.TargetObject as PlayerObject != null)
                {
                    if (Shift && Functions.InRange(MapObject.TargetObject.Location, MapObject.User.Location, 1) && Main.CanAttack)
                    {
                        Dir = Functions.DirectionFromPoint(MapObject.User.Location, MapObject.TargetObject.Location);
                        MapObject.User.Direction = Dir;
                        MapObject.User.DoAction(MirAction.Attack1);
                        OutBound.Attack(0);
                        return;
                    }
                }
                else if (MapObject.TargetObject as MonsterObject != null)
                {
                    if (Functions.InRange(MapObject.TargetObject.Location, MapObject.User.Location, 1) && Main.CanAttack)
                    {
                        Dir = Functions.DirectionFromPoint(MapObject.User.Location, MapObject.TargetObject.Location);
                        MapObject.User.Direction = Dir;
                        MapObject.User.DoAction(MirAction.Attack1);
                        OutBound.Attack(0);
                        return;
                    }
                }
            }



            if (Scene == MirControl.MouseControl && (MapObject.MouseObject as PlayerObject == null))
            {
                Point TPoint = MousePoint(MouseLocation);
                Dir = MouseDirection(MouseLocation);
                switch (MouseB)
                {
                    case MouseButtons.Right:
                        if (MapObject.User.CanDoAction)
                        {
                            if (Math.Abs(TPoint.X - Settings.PlayerOffSet.X) <= 2 && Math.Abs(TPoint.Y - Settings.PlayerOffSet.Y) <= 2)
                            {
                                if (MapObject.User.Frame >1 )
                                {
                                    MapObject.User.Direction = Dir;
                                    MapObject.User.DoAction(MirAction.Standing);
                                    OutBound.Turn();
                                    return;
                                }
                            }
                            else if (Main.CanMove)// && Main.UserCanMove)
                            {
                                if (Main.AllowRun && MapLayer.CanRun(Dir))
                                {
                                    MapObject.User.Location = Functions.PointMove(MapObject.User.Location, Dir, 2);
                                    MapObject.User.Direction = Dir;
                                    MapObject.User.DoAction(MirAction.Running);
                                    OutBound.Run();
                                    return;
                                }
                                else if (MapLayer.CanWalk(Dir))
                                {
                                    MapObject.User.Location = Functions.PointMove(MapObject.User.Location, Dir, 1);
                                    MapObject.User.Direction = Dir;
                                    MapObject.User.DoAction(MirAction.Walking);
                                    OutBound.Walk();
                                    Main.AllowRun = true;
                                    return;
                                }
                                else
                                {
                                    MapObject.User.Direction = Dir;
                                    MapObject.User.DoAction(MirAction.Standing);
                                    OutBound.Turn();
                                    return;
                                }
                            }
                        }
                        break;
                    case MouseButtons.Left:
                        if (MapObject.User.CanDoAction && Shift)
                        {
                            if (Main.CanAttack)
                            {
                                MapObject.User.Direction = Dir;
                                switch (Main.Rand.Next(3))
                                {
                                    case 2:
                                        MapObject.User.DoAction(MirAction.Attack1);
                                        OutBound.Attack(2);
                                        break;
                                    case 1:
                                        MapObject.User.DoAction(MirAction.Attack2);
                                        OutBound.Attack(1);
                                        break;
                                    default:
                                        MapObject.User.DoAction(MirAction.Attack3);
                                        OutBound.Attack(0);
                                        break;

                                }
                            }
                            return;
                        }
                        else if (MapObject.User.CanDoAction && Alt)
                        {
                            MapObject.User.Direction = Dir;
                            MapObject.User.DoAction(MirAction.Harvest);
                            OutBound.Harvest();
                        }
                        else if (TPoint == Settings.PlayerOffSet)
                        {
                            if (Main.Timer.ElapsedMilliseconds > Main.PickUpTime)
                            {
                                Main.PickUpTime = Main.Timer.ElapsedMilliseconds + 200;
                                for (int I = 0; I < ItemObject.ItemList.Count; I++)
                                    if (ItemObject.ItemList[I].Location == MapObject.User.Location)
                                    {
                                        OutBound.PickUp();
                                        return;
                                    }
                            }
                        }
                        else if (MapObject.User.CanDoAction && Main.CanMove)// && Main.UserCanMove)
                        {
                            if (MapLayer.CanWalk(Dir))
                            {
                                MapObject.User.Location = Functions.PointMove(MapObject.User.Location, Dir, 1);
                                MapObject.User.Direction = Dir;
                                MapObject.User.DoAction(MirAction.Walking);
                                OutBound.Walk();
                                return;
                            }
                            else
                            {
                                MapObject.User.Direction = Dir;
                                MapObject.User.DoAction(MirAction.Standing);
                                OutBound.Turn(); 
                                return;
                            }
                        }
                        break;
                }
            }
            if (MapObject.User.CanDoAction && MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (MapObject.TargetObject as PlayerObject != null)
                {
                    if (Shift && !Functions.InRange(MapObject.TargetObject.Location, MapObject.User.Location, 1) && Main.CanMove)
                    {
                        Dir = Functions.DirectionFromPoint(MapObject.User.Location, MapObject.TargetObject.Location);

                        if (MapLayer.CanWalk(Dir))
                        {
                            MapObject.User.Location = Functions.PointMove(MapObject.User.Location, Dir, 1);
                            MapObject.User.Direction = Dir;
                            MapObject.User.DoAction(MirAction.Walking);
                            OutBound.Walk();
                            return;
                        }
                    }
                }
                else if (MapObject.TargetObject as MonsterObject != null)
                {
                    if (!Functions.InRange(MapObject.TargetObject.Location, MapObject.User.Location, 1) && Main.CanMove)
                    {
                        Dir = Functions.DirectionFromPoint(MapObject.User.Location, MapObject.TargetObject.Location);

                        if (MapLayer.CanWalk(Dir))
                        {
                            MapObject.User.Location = Functions.PointMove(MapObject.User.Location, Dir, 1);
                            MapObject.User.Direction = Dir;
                            MapObject.User.DoAction(MirAction.Walking);
                            OutBound.Walk();
                            return;
                        }
                    }
                }
            }
        }

        public static void OutputMessage(string Message)
        {
            OutputMessages.Add(Message);
            OutputTimes.Add(Main.Time);
        }

        public static void UpdateOutput()
        {
            int StartIndex = -1;

            for (int I = 0; I < OutputCount; I++)
            {
                if (OutputTimes.Count - 1 - I < 0) break;
                if (Main.Time - OutputTimes[OutputTimes.Count - 1 - I] > OutputDuration) break;

                StartIndex = OutputTimes.Count - 1 - I;
            }
            for (int I = 0; I < OutputCount; I++)
            {
                if (I + StartIndex < OutputMessages.Count && StartIndex != -1)
                {
                    OutputLines[I].Text = OutputMessages[I + StartIndex];
                    OutputLines[I].Visible = true;
                }
                else
                {
                    OutputLines[I].Text = string.Empty;
                    OutputLines[I].Visible = false;
                }


            }

        }
        private static Point MousePoint(Point MousePoint)
        {
            return new Point(MousePoint.X / Globals.CellWidth, MousePoint.Y / Globals.CellHeight);
        }
        private static double Distance(PointF P1, PointF P2)
        {
            double X, Y;
            X = P2.X - P1.X;
            Y = P2.Y - P1.Y;
            return Math.Sqrt(X * X + Y * Y);
        }
        private static MirDirection MouseDirection(Point MousePoint)
        {
            Point P = new Point(MousePoint.X / Globals.CellWidth, MousePoint.Y / Globals.CellHeight);
            if (Functions.InRange(Settings.PlayerOffSet, P, 2))
                return Functions.DirectionFromPoint(Settings.PlayerOffSet, P);

            PointF A, B, C;
            float AC, BC, AB;
            C = new PointF(Settings.ScreenSize.Width / 2, (Settings.ScreenSize.Height - 150) / 2);
            A = new PointF(C.X, 0);
            B = MousePoint;
            BC = (float)Distance(C, B);
            AC = BC;
            B.Y -= C.Y;
            C.Y += BC;
            B.Y += BC;
            AB = (float)Distance(B, A);
            Double X = (AC * AC + BC * BC - AB * AB) / (2 * AC * BC);
            Double Angle = Math.Acos(X);

            Angle *= 180 / Math.PI;

            if (MousePoint.X < C.X) Angle = 360 - Angle;
            Angle += 22.5F;
            if (Angle > 360) Angle -= 360;

            return (MirDirection)(Angle / 45);
        }

        public static void UseItem(MirItemCell Cell)
        {

        }

        public static void QuitGame()
        {
            //If Last Combat < 10 CANCEL
            MirMessageBox MMBox = new MirMessageBox("Do you want to quit Legened of Mir?", MessageBoxButtons.YesNo);
            MMBox.YesButton.Click += (o, e) => SceneFunctions.QuitGame();
            MMBox.Show();
        }
        public static void LogOut()
        {
            //If Last Combat < 10 CANCEL
            MirMessageBox MMBox = new MirMessageBox("Do you want to log out of Legened of Mir?", MessageBoxButtons.YesNo);
            MMBox.YesButton.Click += (o, e) => SceneFunctions.LogOut();
            MMBox.Show();
        }

        #region Item Label

        internal static MirControl ItemLabel;
        internal static UserItem MouseItem;

        internal static void DisposeItemLabel()
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Dispose();

            ItemLabel = null;

        }
        internal static void CreateItemLabel(UserItem I, Point Location)
        {
            DisposeItemLabel();
            if (ItemLabel != null || I == null) return;

            ItemLabel = new MirControl
            {
                BackColor = Color.FromArgb(255, 0, 24, 48),
                Border = true,
                BorderColor = Color.FromArgb(144, 148, 48),
                DrawControlTexture = true,
                NotControl = true,
                Opacity = 0.7F,
            };

            MirLabel NameLabel = new MirLabel
            {
                AutoSize = true,
                ForeColor = Color.Yellow,
                Location = new Point(5, ItemLabel.Size.Height + 3),
                OutLine = false,
                Parent = ItemLabel,
                Text = I.Info.ItemName
            };
            ItemLabel.Size = NameLabel.Size;

            GeneralItemInfo(I);

            switch (I.Info.ItemType)
            {
                case MirItemType.Potion:
                    PotionItemInfo(I);
                    break;
                default:
                    EquipmentItemInfo(I);
                    break;

            }

            if (I.Info.RequiredClass != MirRequiredClass.None)
            {
                Color LabelColour = Color.White;
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Warrior))
                            LabelColour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Wizard))
                            LabelColour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Taoist))
                            LabelColour = Color.Red;
                        break;
                    case MirClass.Assassin:
                        if (!I.Info.RequiredClass.HasFlag(MirRequiredClass.Assassin))
                            LabelColour = Color.Red;
                        break;
                }

                MirLabel ClassLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = LabelColour,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Class Required: {0}", I.Info.RequiredClass),
                };
                if (I.Info.RequiredClass == MirRequiredClass.WarWizTao)
                    ClassLabel.Text = string.Format("Class Required: {0}, {1}, {2}", MirRequiredClass.Warrior, MirRequiredClass.Wizard, MirRequiredClass.Taoist);

                ItemLabel.Size = new Size(ClassLabel.Size.Width > ItemLabel.Size.Width ? ClassLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + ClassLabel.Size.Height);
            }

            if (I.Info.RequiredAmount > 0)
            {
                string Text;
                Color Colour = Color.White;
                switch (I.Info.RequiredType)
                {
                    case MirRequiredType.Level:
                        Text = string.Format("Level Required: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.Level < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    case MirRequiredType.AC:
                        Text = string.Format("Level AC: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.MaxAC < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    case MirRequiredType.MAC:
                        Text = string.Format("Level MAC: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.MaxMAC < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    case MirRequiredType.DC:
                        Text = string.Format("Level DC: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.MaxDC < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    case MirRequiredType.MC:
                        Text = string.Format("Level MC: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.MaxMC < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    case MirRequiredType.SC:
                        Text = string.Format("Level SC: {0}", I.Info.RequiredAmount);
                        if (MapObject.User.MaxSC < I.Info.RequiredAmount)
                            Colour = Color.Red;
                        break;
                    default:
                        Text = "Unknown Type Required";
                        break;
                }
                MirLabel LevelLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Colour,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = Text,
                };


                ItemLabel.Size = new Size(LevelLabel.Size.Width > ItemLabel.Size.Width ? LevelLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + LevelLabel.Size.Height);
            }
            ItemLabel.Size = new Size(ItemLabel.Size.Width + 10, ItemLabel.Size.Height + 3);

            int X = Location.X, Y = Location.Y;
            if (Location.X + ItemLabel.Size.Width > Settings.ScreenSize.Width)
                X = Settings.ScreenSize.Width - ItemLabel.Size.Width;

            if (Location.Y + ItemLabel.Size.Height > Settings.ScreenSize.Height)
                Y = Settings.ScreenSize.Height - ItemLabel.Size.Height;
            ItemLabel.Location = new Point(X, Y);
        }
        internal static void GeneralItemInfo(UserItem I)
        {
            if (I.Info.Weight > 0)
            {
                MirLabel WeightLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Weight: {0}", I.Info.Weight)
                };
                ItemLabel.Size = new Size(WeightLabel.Size.Width > ItemLabel.Size.Width ? WeightLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + WeightLabel.Size.Height);
            }

            if (I.Info.StackSize > 1)
            {
                MirLabel CountLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Count: {0}/{1}", I.Amount, I.Info.StackSize)
                };

                ItemLabel.Size = new Size(CountLabel.Size.Width > ItemLabel.Size.Width ? CountLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + CountLabel.Size.Height);
            }

        }
        internal static void PotionItemInfo(UserItem I)
        {
            if (I.Info.Health > 0)
            {
                MirLabel HPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Health Restoration: +{0}HP", I.Info.Health)
                };
                ItemLabel.Size = new Size(HPLabel.Size.Width > ItemLabel.Size.Width ? HPLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + HPLabel.Size.Height);
            }
            if (I.Info.Mana > 0)
            {
                MirLabel MPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Mana Restoration: +{0}MP", I.Info.Mana)
                };
                ItemLabel.Size = new Size(MPLabel.Size.Width > ItemLabel.Size.Width ? MPLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + MPLabel.Size.Height);
            }
        }
        internal static void EquipmentItemInfo(UserItem I)
        {
            if (I.Info.Durability > 0)
            {
                MirLabel DurabilityLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.CurrentDurability <= 0 ? Color.Red : Color.Cyan,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                };

                if (I.Info.ItemType == MirItemType.Amulet)
                    DurabilityLabel.Text = string.Format("Useage: {0}/{1}", I.CurrentDurability, I.MaxDurability);
                else
                    DurabilityLabel.Text = string.Format("Durability: {0}/{1}", I.CurrentDurability / 1000, I.MaxDurability / 1000);

                ItemLabel.Size = new Size(DurabilityLabel.Size.Width > ItemLabel.Size.Width ? DurabilityLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + DurabilityLabel.Size.Height);
            }

            if (I.Info.MinAC > 0 || I.Info.MaxAC > 0 || I.AddedAC > 0)
            {
                MirLabel ACLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedAC > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedAC > 0 ? "AC: {0}-{1} (+{2})" : "AC: {0}-{1}", I.Info.MinAC, I.Info.MaxAC + I.AddedAC, I.AddedAC)
                };
                ItemLabel.Size = new Size(ACLabel.Size.Width > ItemLabel.Size.Width ? ACLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + ACLabel.Size.Height);
            }

            if (I.Info.MinMAC > 0 || I.Info.MaxMAC > 0 || I.AddedMAC > 0)
            {
                MirLabel MACLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedMAC > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedMAC > 0 ? "MAC: {0}-{1} (+{2})" : "MAC: {0}-{1}", I.Info.MinMAC, I.Info.MaxMAC + I.AddedMAC, I.AddedMAC)
                };
                ItemLabel.Size = new Size(MACLabel.Size.Width > ItemLabel.Size.Width ? MACLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + MACLabel.Size.Height);
            }
            if (I.Info.MinDC > 0 || I.Info.MaxDC > 0 || I.AddedDC > 0)
            {
                MirLabel DCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedDC > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedDC > 0 ? "DC: {0}-{1} (+{2})" : "DC: {0}-{1}", I.Info.MinDC, I.Info.MaxDC + I.AddedDC, I.AddedDC)
                };
                ItemLabel.Size = new Size(DCLabel.Size.Width > ItemLabel.Size.Width ? DCLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + DCLabel.Size.Height);
            }
            if (I.Info.MinMC > 0 || I.Info.MaxMC > 0 || I.AddedMC > 0)
            {
                MirLabel MCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedMC > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedMC > 0 ? "MC: {0}-{1} (+{2})" : "MC: {0}-{1}", I.Info.MinMC, I.Info.MaxMC + I.AddedMC, I.AddedMC)
                };
                ItemLabel.Size = new Size(MCLabel.Size.Width > ItemLabel.Size.Width ? MCLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + MCLabel.Size.Height);
            }
            if (I.Info.MinSC > 0 || I.Info.MaxSC > 0 || I.AddedSC > 0)
            {
                MirLabel SCLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedSC > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedSC > 0 ? "SC: {0}-{1} (+{2})" : "SC: {0}-{1}", I.Info.MinSC, I.Info.MaxSC + I.AddedSC, I.AddedSC)
                };
                ItemLabel.Size = new Size(SCLabel.Size.Width > ItemLabel.Size.Width ? SCLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + SCLabel.Size.Height);
            }



            if (I.Info.Accuracy > 0 || I.AddedAccuracy > 0)
            {
                MirLabel AccuracyLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedAccuracy > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedAccuracy > 0 ? "Accuracy: +{0} (+{1})" : "Accuracy: +{0}", I.Info.Accuracy + I.AddedAccuracy, I.AddedAccuracy)
                };
                ItemLabel.Size = new Size(AccuracyLabel.Size.Width > ItemLabel.Size.Width ? AccuracyLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + AccuracyLabel.Size.Height);
            }

            if (I.Info.Agility > 0 || I.AddedAgility > 0)
            {
                MirLabel AgilityLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedAgility > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedAgility > 0 ? "Agility: +{0} (+{1})" : "Agility: +{0}", I.Info.Agility + I.AddedAgility, I.AddedAgility)
                };
                ItemLabel.Size = new Size(AgilityLabel.Size.Width > ItemLabel.Size.Width ? AgilityLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + AgilityLabel.Size.Height);
            }
            /*
            if (I.Info.MagicResist > 0 || I.AddedMagicResist > 0)
            {
                MirLabel MagicResistLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedMagicResist > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedMagicResist > 0 ? "Magic Resist: +{0}% (+{1}%)" : "Magic Resist: +{0}%", I.Info.MagicResist + I.AddedMagicResist, I.AddedMagicResist)
                };
                ItemLabel.Size = new Size(MagicResistLabel.Size.Width > ItemLabel.Size.Width ? MagicResistLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + MagicResistLabel.Size.Height);
            }
            */

            if (I.Info.Health > 0 || I.AddedHealth > 0)
            {
                MirLabel HPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedHealth > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedHealth > 0 ? "Health: +{0} (+{1})" : "Health: +{0}", I.Info.Health + I.AddedHealth, I.AddedHealth)
                };
                ItemLabel.Size = new Size(HPLabel.Size.Width > ItemLabel.Size.Width ? HPLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + HPLabel.Size.Height);
            }


            if (I.Info.Mana > 0 || I.AddedMana > 0)
            {
                MirLabel MPLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedMana > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedMana > 0 ? "Mana: +{0} (+{1})" : "Mana: +{0}", I.Info.Mana + I.AddedMana, I.AddedMana)
                };
                ItemLabel.Size = new Size(MPLabel.Size.Width > ItemLabel.Size.Width ? MPLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + MPLabel.Size.Height);
            }


            if (I.Info.BodyWeight > 0 || I.AddedBodyWeight > 0)
            {
                MirLabel BodyWeightLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedBodyWeight > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedBodyWeight > 0 ? "Body Weight: +{0} (+{1})" : "Body Weight: +{0}", I.Info.BodyWeight + I.AddedBodyWeight, I.AddedBodyWeight)
                };
                ItemLabel.Size = new Size(BodyWeightLabel.Size.Width > ItemLabel.Size.Width ? BodyWeightLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + BodyWeightLabel.Size.Height);
            }
            if (I.Info.HandWeight > 0 || I.AddedHandWeight > 0)
            {
                MirLabel HandWeightLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedHandWeight > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedHandWeight > 0 ? "Hand Weight: +{0} (+{1})" : "Hand Weight: +{0}", I.Info.HandWeight + I.AddedHandWeight, I.AddedHandWeight)
                };
                ItemLabel.Size = new Size(HandWeightLabel.Size.Width > ItemLabel.Size.Width ? HandWeightLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + HandWeightLabel.Size.Height);
            }

            if (I.Info.BagWeight > 0 || I.AddedBagWeight > 0)
            {
                MirLabel BagWeightLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedBagWeight > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedBagWeight > 0 ? "Bag Weight: +{0} (+{1})" : "Bag Weight: +{0}", I.Info.BagWeight + I.AddedBagWeight, I.AddedBagWeight)
                };
                ItemLabel.Size = new Size(BagWeightLabel.Size.Width > ItemLabel.Size.Width ? BagWeightLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + BagWeightLabel.Size.Height);
            }

            if (I.Info.AttackSpeed > 0 || I.AddedAttackSpeed > 0)
            {
                MirLabel AttackSpeedLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = I.AddedAttackSpeed > 0 ? Color.Cyan : Color.White,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedAttackSpeed > 0 ? "A.Speed: +{0} (+{1})" : "A.Speed: +{0}", I.Info.AttackSpeed + I.AddedAttackSpeed, I.AddedAttackSpeed)
                };
                ItemLabel.Size = new Size(AttackSpeedLabel.Size.Width > ItemLabel.Size.Width ? AttackSpeedLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + AttackSpeedLabel.Size.Height);
            }
            if (I.Info.Luck > 0 || I.AddedLuck > 0)
            {
                MirLabel LuckLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColor = Color.Yellow,
                    Location = new Point(5, ItemLabel.Size.Height + 3),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format(I.AddedLuck > 0 ? "Luck: +{0} (+{1})" : "Luck: +{0}", I.Info.Luck + I.AddedLuck, I.AddedLuck)
                };
                ItemLabel.Size = new Size(LuckLabel.Size.Width > ItemLabel.Size.Width ? LuckLabel.Size.Width : ItemLabel.Size.Width, ItemLabel.Size.Height + LuckLabel.Size.Height);
            }

        }
        #endregion
    }
}
