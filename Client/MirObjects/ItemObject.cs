using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Library;
using Client.MirGraphics;

namespace Client.MirObjects
{
    class ItemObject : MapObject
    {
        public bool Added;
        public static List<ItemObject> ItemList = new List<ItemObject>();
        public override bool Blocking
        {
            get
            {
                return false;
            }
        }



        public ItemObject(ItemDetails D)
        {
            ObjectID = D.ObjectID;

            PrimaryLibrary = Libraries.DNItems;
                        
            if (D.Gold > 0)
            {
                Name = string.Format("Gold ({0:###,###,###})", D.Gold);

                if (D.Gold < 100)
                    ImageIndex = 112;
                else if (D.Gold < 200)
                    ImageIndex = 113;
                else if (D.Gold < 500)
                    ImageIndex = 114;
                else if (D.Gold < 1000)
                    ImageIndex = 115;
                else
                    ImageIndex = 116;
            }
            else
            {
                Name = D.ItemName;
                ImageIndex = D.ImageIndex;
                Added = D.Added;
            }

            Location = D.Location;
            ActualLocation = Location;
            DrawLocation = Location;
            MovePoint = Location;
            UseOffSet = false;
            ItemList.Add(this);
        }

        public override void DoAction(MirAction Action) { }

        public override void FrameProcess() { } //Sparkle?
        public override void LocationProcess()
        {
            if (PrimaryLibrary != null)
            {
                #region Display Rectangle

                DisplayRectangle.Size = PrimaryLibrary.GetSize(ImageIndex);

                DisplayRectangle.Location = new Point(
                    (Location.X - MapObject.User.MovePoint.X + Settings.PlayerOffSet.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X,
                    (Location.Y - MapObject.User.MovePoint.Y + Settings.PlayerOffSet.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y);
                #endregion

                DisplayRectangle.X += (Globals.CellWidth - DisplayRectangle.Width) / 2;
                DisplayRectangle.Y += (Globals.CellHeight - DisplayRectangle.Height) / 2;

                FinalDisplayLocation = DisplayRectangle.Location;
            }
            else
            {
                DisplayRectangle = Rectangle.Empty;
                FinalDisplayLocation = Point.Empty;
            }
        }
        public override void DrawName()
        {
            if (NameLabel == null || NameLabel.IsDisposed)
                CreateLabel();

            NameLabel.BackColor = Color.Transparent;
            NameLabel.Border = false;
            NameLabel.OutLine = true;

            if (NameLabel != null)
            {
                Size S = PrimaryLibrary.GetSize(ImageIndex);
                NameLabel.Location = new Point(
                    DisplayRectangle.X + (DisplayRectangle.Width - NameLabel.Size.Width) / 2,
                    DisplayRectangle.Y + (DisplayRectangle.Height - NameLabel.Size.Height) / 2 - 20);
                NameLabel.Draw();
            }
        }
        public void DrawName(Point OffSet)
        {
            if (NameLabel == null || NameLabel.IsDisposed)
                CreateLabel();

            NameLabel.BackColor = Color.FromArgb(100, 0, 24, 48);
            NameLabel.Border = true;
            //NameLabel.OutLine = false;

            if (NameLabel != null)
            {
                Size S = PrimaryLibrary.GetSize(ImageIndex);
                NameLabel.Location = new Point(
                    DisplayRectangle.X + OffSet.X + (DisplayRectangle.Width - NameLabel.Size.Width) / 2,
                    DisplayRectangle.Y + OffSet.Y + (DisplayRectangle.Height - NameLabel.Size.Height) / 2 - 20);
                NameLabel.Draw();
            }
        }
        public override bool MouseOver(Point P)
        {
            return DisplayRectangle.Contains(P);
        }
        public override void CreateLabel()
        {
            NameLabel = new MirControls.MirLabel
            {
                AutoSize = true,
                BorderColor = Color.Black,
                BackColor = Color.FromArgb(100, 0, 24, 48),
                ForeColor = Added ? Color.Cyan : Color.White,
                OutLine = false,
                Text = Name,
            };
        }

        public override void Remove()
        {
            base.Remove();
            ItemList.Remove(this);
        }
    }
}
