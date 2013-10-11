using System;
using Library;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Client.MirControls;
using Client.MirGraphics;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Client.MirObjects;

namespace Client.MirScenes.Game_Scene
{
    class MiniMap
    {
        public static MirImageControl Window, LightSetting;
        public static MirButton ToggleButton, BigMapButton, MailButton;
        public static MirLabel LocationLabel, MapNameLabel;
        public static float Opacity = 1F;

        public static void Create()
        {
            Window = new MirImageControl
            {
                Index = 2090,
                Parent = GameScene.Scene,
                Library = Libraries.Prguse,
                Location = new System.Drawing.Point(674, 0),
                PixelDetect = true,
            };
            Window.BeforeDraw += new EventHandler(Window_BeforeDraw);

            MapNameLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.Center | DrawTextFormat.VerticalCenter,
                Parent = Window,
                Size = new Size(120, 18),
                Location = new Point(2, 2),
            };

            LocationLabel = new MirLabel
            {
                DrawFormat = DrawTextFormat.Center | DrawTextFormat.VerticalCenter,
                Parent = Window,
                Size = new Size(56, 18),
                Location = new Point(46, 131),
            };

            MailButton = new MirButton
            {
                Index = 2099,
                HoverIndex = 2100,
                PressedIndex = 2101,
                Parent = Window,
                Location = new Point(4, 131),
                Library = Libraries.Prguse,
                Sound = MirSound.SoundList.ClickA,
            };

            BigMapButton = new MirButton
            {
                Index = 2096,
                HoverIndex = 2097,
                PressedIndex = 2098,
                Parent = Window,
                Location = new Point(25, 131),
                Library = Libraries.Prguse,
                Sound = MirSound.SoundList.ClickA,
            };
            ToggleButton = new MirButton
            {
                Index = 2102,
                HoverIndex = 2103,
                PressedIndex = 2104,
                Parent = Window,
                Location = new Point(109, 3),
                Library = Libraries.Prguse,
                Sound = MirSound.SoundList.ClickA,
            };
            ToggleButton.Click += ToggleButton_Click;
        }

        static void ToggleButton_Click(object sender, EventArgs e)
        {
            Toggle();
        }

        static void Window_BeforeDraw(object sender, EventArgs e)
        {
            MiniMap.MapNameLabel.Text = MapLayer.MapInfo.MapName;
            MiniMap.LocationLabel.Text = Functions.PointToString(MapObject.User.Location);

            if (MapLayer.MapInfo.MiniMap > 0 && Window.Index == 2090 && Libraries.MMap != null)
            {
                Rectangle ViewRect = new Rectangle(0, 0, 120, 108);
                Point DrawLocation = Window.Location; DrawLocation.Offset(3, 22);

                Size MiniMapSize = Libraries.MMap.GetSize(MapLayer.MapInfo.MiniMap);
                float ScaleX = MiniMapSize.Width / (float)MapLayer.MapSize.Width;
                float ScaleY = MiniMapSize.Height / (float)MapLayer.MapSize.Height;

                ViewRect.Location = new Point(
                    (int)(ScaleX * MapObject.User.Location.X) - ViewRect.Width / 2,
                    (int)(ScaleY * MapObject.User.Location.Y) - ViewRect.Height / 2);

                if (ViewRect.Right >= MiniMapSize.Width)
                    ViewRect.X = MiniMapSize.Width - ViewRect.Width;
                if (ViewRect.Bottom >= MiniMapSize.Height)
                    ViewRect.Y = MiniMapSize.Height - ViewRect.Height;

                if (ViewRect.X < 0) ViewRect.X = 0;
                if (ViewRect.Y < 0) ViewRect.Y = 0;

                Libraries.MMap.Draw(MapLayer.MapInfo.MiniMap, ViewRect, DrawLocation, Color.White, Opacity);
                DXManager.Sprite.Flush();

                int StartPointX = (int)(ViewRect.X / ScaleX);
                int StartPointY = (int)(ViewRect.Y / ScaleY);

                MapObject Ob;


                for (int I = 0; I < MapLayer.ObjectList.Count; I++)
                {
                    Ob = MapLayer.ObjectList[I];
                    if (Ob is ItemObject || Ob.Dead) continue;
                    float X = ((Ob.Location.X - StartPointX) * ScaleX) + DrawLocation.X;
                    float Y = ((Ob.Location.Y - StartPointY) * ScaleY) + DrawLocation.Y;

                    Color C;

                    if (Ob == MapObject.User)
                        C = Color.FromArgb((int)(Opacity * 255), 255, 255, 255);
                    else
                        C = Color.FromArgb((int)(Opacity * 255), 255, 0, 0);

                    DXManager.Sprite.Draw2D(DXManager.RadarTexture, Point.Empty, 0, new PointF(X - 0.5F, Y - 0.5F), C);
                    //DXManager.Line.Draw(new Vector2[] { new Vector2(X - 2, Y), new Vector2(X, Y) }, C);
                   // DXManager.Line.Draw(new Vector2[] { new Vector2(X - 2, Y + 1), new Vector2(X, Y + 1) }, C);
                }
            }

        }

        static void SetSmallMode()
        {
            Window.Index = 2091;
            int Y = Window.DisplayRectangle.Bottom - 23;
            MailButton.Location = new Point(4, Y);
            BigMapButton.Location = new Point(25, Y);
            LocationLabel.Location = new Point(46, Y);
        }

        static void SetBigMode()
        {
            Window.Index = 2090;
            int Y = Window.DisplayRectangle.Bottom - 23;
            MailButton.Location = new Point(4, Y);
            BigMapButton.Location = new Point(25, Y);
            LocationLabel.Location = new Point(46, Y);
        }

        internal static void Toggle()
        {
            if (Opacity == 0F)
            {
                SetBigMode();
                Opacity = 0.5F;
            }
            else if (Opacity == 0.5F)
            {
                Opacity = 1F;
            }
            else
            {
                SetSmallMode();
                Opacity = 0;
            }
            
        }
    }
}
