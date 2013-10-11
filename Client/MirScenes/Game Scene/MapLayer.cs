using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Client.MirControls;
using System.IO;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Client.MirGraphics;
using Client.MirObjects;

namespace Client.MirScenes.Game_Scene
{
    class MapLayer
    {
        static readonly int ViewRangeX = Settings.PlayerOffSet.X + 4, ViewRangeY = Settings.PlayerOffSet.Y + 4;

        public static List<MapObject> ObjectList = new List<MapObject>();

        public static Mir2CellInfo[,] M2CellInfo;
        public static ClientMapInfo MapInfo;
        public static Size MapSize;
        public static int AnimationCount;

        private static Texture FloorTexture, LightTexture;
        private static Surface FloorSurface, LightSurface;
        private static Point FloorOffSet;
        private static bool ValidFloor;

        public static void LoadMap(ClientMapInfo MI)
        {
            if (MapInfo == MI) return;

            if (FloorTexture != null && !FloorTexture.Disposed) FloorTexture.Dispose();
            if (LightTexture != null && !LightTexture.Disposed) LightTexture.Dispose();

            if (MI == null)
            {
                M2CellInfo = null;
                return;
            }

            ObjectList.Add(MapObject.User);
            MapInfo = MI;

            try
            {
                string FileName = Path.Combine(Settings.MapPath, MapInfo.FileName + ".map");
                if (File.Exists(FileName))
                {
                    int OffSet = 21;
                    Mir2CellInfo CI;
                    byte[] FileBytes = File.ReadAllBytes(FileName);
                    int W = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    int Xor = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    int H = BitConverter.ToInt16(FileBytes, OffSet); OffSet += 2;
                    MapSize = new Size((short)W ^ (short)Xor, H ^ Xor);
                    M2CellInfo = new Mir2CellInfo[MapSize.Width, MapSize.Height];

                    OffSet = 54;

                    for (int X = 0; X < MapSize.Width; X++)
                        for (int Y = 0; Y < MapSize.Height; Y++)
                        {
                            CI = new Mir2CellInfo
                            {
                                BackImage = (int)(BitConverter.ToInt32(FileBytes, OffSet) ^ 0xAA38AA38),
                                MiddleImage = (short)(BitConverter.ToInt16(FileBytes, OffSet += 4) ^ Xor),
                                FrontImage = (short)(BitConverter.ToInt16(FileBytes, OffSet += 2) ^ Xor),
                                DoorIndex = FileBytes[OffSet += 2],
                                DoorOffset = FileBytes[++OffSet],
                                AnimationFrame = FileBytes[++OffSet],
                                AnimationTick = FileBytes[++OffSet],
                                FileIndex = FileBytes[++OffSet],
                                Light = FileBytes[++OffSet],
                                Unknown = FileBytes[++OffSet],
                            };
                            OffSet++;

                            M2CellInfo[X, Y] = CI;
                        }

                }
                else
                    M2CellInfo = null;

            }
            catch (Exception Ex)
            {
                if (Settings.LogErrors) Main.SaveError(Ex.ToString());
            }
        }
        
        public static void Render()
        {
            if (!ValidFloor || FloorOffSet != MapObject.User.MovingOffSet)
                RenderFloor();

            if (FloorTexture != null && !FloorTexture.Disposed)
                DXManager.Sprite.Draw2D(FloorTexture, Point.Empty, 0F, Point.Empty, Color.White);

            RenderObjects();
                        
            if (MapInfo.Lights != LightSetting.Day)
                RenderLights();

            if (MapObject.MouseObject != null && !(MapObject.MouseObject is ItemObject))
                MapObject.MouseObject.DrawName();

            int OffSet = 0;

            if (Settings.ShowItemNames)
                for (int I = 0; I < ItemObject.ItemList.Count; I++)
                    if (!ItemObject.ItemList[I].MouseOver(GameScene.MouseLocation))
                        ItemObject.ItemList[I].DrawName();

            for (int I = 0; I < ItemObject.ItemList.Count; I++)
            {
                ItemObject Item = ItemObject.ItemList[I];
                if (Item.MouseOver(GameScene.MouseLocation))
                {
                    Item.DrawName(new Point(0, OffSet));
                    OffSet -= Item.NameLabel.Size.Height + (Item.NameLabel.Border ? 1 : 0);
                }
            }

            if (MapObject.User.MouseOver(GameScene.MouseLocation))
                MapObject.User.DrawName();
        }

        private static void RenderFloor()
        {
            if (FloorTexture == null || FloorTexture.Disposed)
            {
                FloorTexture = new Texture(DXManager.Device, Settings.ScreenSize.Width, 
                    Settings.ScreenSize.Height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                FloorTexture.Disposing += new EventHandler(FloorTexture_Disposing);
                FloorSurface = FloorTexture.GetSurfaceLevel(0);
            }


            Surface OldSurface = DXManager.CurrentSurface;

            DXManager.SetSurface(FloorSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);            

            Point P;
            Size S;
            int ImageIndex, FileIndex;
            
            #region Big Tiles
            for (int Y = MapObject.User.Location.Y - ViewRangeY; Y <= MapObject.User.Location.Y + ViewRangeY + 2; Y++)
            {
                if (Y < 0 || Y % 2 == 1) continue;
                if (Y >= MapSize.Height) break;
                for (int X = MapObject.User.Location.X - ViewRangeX; X < MapObject.User.Location.X + ViewRangeX; X++)
                {
                    if (X < 0 || X % 2 == 1) continue;
                    if (X >= MapSize.Width) break;

                    if (M2CellInfo[X, Y].BackImage == 0) continue;
                    ImageIndex = (M2CellInfo[X, Y].BackImage & 0x7FFF);

                    P = new Point((X + Settings.PlayerOffSet.X - MapObject.User.MovePoint.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X, (Y + Settings.PlayerOffSet.Y - MapObject.User.MovePoint.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y);
                    Libraries.Tiles.Draw(ImageIndex - 1, P, Color.White, 1F, false);
                }
            }
            #endregion
            #region Small Tiles
            for (int Y = MapObject.User.Location.Y - ViewRangeY; Y <= MapObject.User.Location.Y + ViewRangeY + 2; Y++)
            {
                if (Y < 0) continue;
                if (Y >= MapSize.Height) break;
                for (int X = MapObject.User.Location.X - ViewRangeX; X < MapObject.User.Location.X + ViewRangeX; X++)
                {
                    if (X < 0) continue;
                    if (X >= MapSize.Width) break;
                    ImageIndex = M2CellInfo[X, Y].MiddleImage;

                    if (ImageIndex-- > 0)
                    {
                        P = new Point((X + Settings.PlayerOffSet.X - MapObject.User.MovePoint.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X, (Y + Settings.PlayerOffSet.Y - MapObject.User.MovePoint.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y);
                        Libraries.SmallTiles.Draw(ImageIndex, P, Color.White, 1F, false);
                    }
                }
            }
            #endregion
            #region Objects
            for (int Y = MapObject.User.Location.Y - ViewRangeY; Y <= MapObject.User.Location.Y + ViewRangeY + 2; Y++)
            {
                if (Y < 0) continue;
                if (Y >= MapSize.Height) break;
                for (int X = MapObject.User.Location.X - ViewRangeX; X < MapObject.User.Location.X + ViewRangeX; X++)
                {
                    if (X < 0) continue;
                    if (X >= MapSize.Width) break;
                    ImageIndex = M2CellInfo[X, Y].FrontImage & 0x7FFF;
                    if (ImageIndex-- > 0)
                    {
                        FileIndex = M2CellInfo[X, Y].FileIndex;
                        S = Libraries.Objects[FileIndex].GetSize(ImageIndex);

                        if (S.Width != Globals.CellWidth || S.Height != Globals.CellHeight) continue;

                        P = new Point((X + Settings.PlayerOffSet.X - MapObject.User.MovePoint.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X, (Y + Settings.PlayerOffSet.Y - MapObject.User.MovePoint.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y);
                        Libraries.Objects[FileIndex].Draw(ImageIndex, P, Color.White, 1F, false);
                    }
                }

            }
            #endregion
            
            DXManager.Sprite.Flush();
            DXManager.SetSurface(OldSurface);

            ValidFloor = true;
            FloorOffSet = MapObject.User.MovingOffSet;
        }

        private static void RenderObjects()
        {
            int ImageIndex, FileIndex;
            byte AnimationFrame, AnimationTick;
            Size S; Point P;

            List<MapObject> DrawList = new List<MapObject>();
            
            #region Objects
            for (int Y = MapObject.User.Location.Y - ViewRangeY; Y <= MapObject.User.Location.Y + ViewRangeY + 25; Y++)
            {
                if (Y < 0) continue;
                if (Y >= MapSize.Height) break;
                for (int X = MapObject.User.Location.X - ViewRangeX; X < MapObject.User.Location.X + ViewRangeX; X++)
                {
                    if (X < 0) continue;
                    if (X >= MapSize.Width) break;
                    ImageIndex = M2CellInfo[X, Y].FrontImage & 0x7FFF;
                    if (ImageIndex-- > 0)
                    {
                        AnimationFrame = (byte)(M2CellInfo[X, Y].AnimationFrame & 0x7F);
                        if (AnimationFrame > 0)
                        {
                            AnimationTick = M2CellInfo[X, Y].AnimationTick;
                            ImageIndex += (AnimationCount % (AnimationFrame + (AnimationFrame * AnimationTick))) / (1 + AnimationTick);
                        }

                        FileIndex = M2CellInfo[X, Y].FileIndex;
                        S = Libraries.Objects[FileIndex].GetSize(ImageIndex);

                        if (S.Width == Globals.CellWidth && S.Height == Globals.CellHeight) continue;

                        P = new Point((X + Settings.PlayerOffSet.X - MapObject.User.MovePoint.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X, (Y + Settings.PlayerOffSet.Y - MapObject.User.MovePoint.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y + 32 - S.Height);

                        if (AnimationFrame > 0)
                            P.Offset(Libraries.Objects[FileIndex].GetOffSet(ImageIndex));

                        if (P.Y <= Settings.ScreenSize.Height)
                        {
                            if (AnimationFrame > 0)
                                Libraries.Objects[FileIndex].DrawBlend(ImageIndex, P, Color.White, false);
                            else
                                Libraries.Objects[FileIndex].Draw(ImageIndex, P, Color.White, 1F, false);
                        }
                    }
                } 
                for (int I = 0; I < ObjectList.Count; I++)
                {
                    MapObject Ob = ObjectList[I];
                    if (Ob.DrawLocation.Y - Y == 0)
                        Ob.Draw();
                }
            }
            #endregion


            float OldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(0.5F);
            MapObject.User.DrawBody();
            MapObject.User.DrawHead();
            DXManager.SetOpacity(OldOpacity);

            if (MapObject.MouseObject != null)
                 MapObject.MouseObject.DrawBlend();

            }

        private static void RenderLights()
        {
            if (DXManager.Lights == null || DXManager.Lights.Count == 0) return;

            if (LightTexture == null || LightTexture.Disposed)
            {
                LightTexture = new Texture(DXManager.Device, Settings.ScreenSize.Width, Settings.ScreenSize.Height, 0, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                LightTexture.Disposing += new EventHandler(LightTexture_Disposing);
                LightSurface = LightTexture.GetSurfaceLevel(0);
            }

            DXManager.Sprite.Flush();

            Surface OldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(LightSurface);
            DXManager.Device.Clear(ClearFlags.Target, MapInfo.Lights == LightSetting.Night ? Color.Black : Color.FromArgb(255, 50, 50, 50), 0, 0);


            DXManager.SetBlend(true);

            int Light;
            Point Location;

            for (int I = 0; I < ObjectList.Count; I++)
                if (ObjectList[I].Light > 0)
                {

                    Light = ObjectList[I].Light;

                    if (Light >= DXManager.Lights.Count)
                        Light = DXManager.Lights.Count - 1;

                    Location = ObjectList[I].DisplayRectangle.Location;
                    Location.Offset(((Light + 1) * -65 + Globals.CellWidth) / 2, ((Light + 1) * -50 + Globals.CellHeight) / 2);


                    if (DXManager.Lights[Light] != null && !DXManager.Lights[Light].Disposed)
                        DXManager.Sprite.Draw2D(DXManager.Lights[Light], PointF.Empty, 0, Location, Color.WhiteSmoke);
                }


            for (int Y = MapObject.User.Location.Y - ViewRangeY - 5; Y <= MapObject.User.Location.Y + ViewRangeY + 5; Y++)
            {
                if (Y < 0) continue;
                if (Y >= MapSize.Height) break;
                for (int X = MapObject.User.Location.X - ViewRangeX - 5; X < MapObject.User.Location.X + ViewRangeX + 5; X++)
                {
                    if (X < 0) continue;
                    if (X >= MapSize.Width) break;
                    int ImageIndex = (M2CellInfo[X, Y].FrontImage & 0x7FFF) - 1;
                    if (M2CellInfo[X, Y].Light > 0 && M2CellInfo[X, Y].Light < 10)
                    {
                        Light = M2CellInfo[X, Y].Light * 3;
                        int FileIndex = M2CellInfo[X, Y].FileIndex;
                        Size S = Libraries.Objects[FileIndex].GetSize(ImageIndex);
                        Location = new Point(
                            (X + Settings.PlayerOffSet.X - MapObject.User.MovePoint.X) * Globals.CellWidth + MapObject.User.MovingOffSet.X,
                            (Y + Settings.PlayerOffSet.Y - MapObject.User.MovePoint.Y) * Globals.CellHeight + MapObject.User.MovingOffSet.Y + 32);// - S.Height);            
                        Location.Offset(((Light + 1) * -65 + Globals.CellWidth) / 2, ((Light + 1) * -50 + Globals.CellHeight) / 2);

                        if (M2CellInfo[X, Y].AnimationFrame > 0)
                            Location.Offset(Libraries.Objects[FileIndex].GetOffSet(ImageIndex));

                        if (Light > DXManager.Lights.Count)
                            Light = DXManager.Lights.Count - 1;

                        if (DXManager.Lights[Light] != null && !DXManager.Lights[Light].Disposed)
                            DXManager.Sprite.Draw2D(DXManager.Lights[Light], PointF.Empty, 0, Location, Color.WhiteSmoke);

                    }
                }
            }

            DXManager.SetBlend(false);
            DXManager.Sprite.Flush();
            DXManager.SetSurface(OldSurface);

            //Draw Light Scene
            DXManager.Device.RenderState.SourceBlend = Blend.Zero;
            DXManager.Device.RenderState.DestinationBlend = Blend.SourceColor;

            DXManager.Sprite.Draw2D(LightTexture, PointF.Empty, 0, PointF.Empty, Color.White);
            DXManager.Sprite.End();

            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
        }
        private static void FloorTexture_Disposing(object sender, EventArgs e)
        {
            ValidFloor = false;
            FloorTexture = null;

            if (FloorSurface != null || !FloorSurface.Disposed)
                FloorSurface.Dispose();
            FloorSurface = null;
        }
        private static void LightTexture_Disposing(object sender, EventArgs e)
        {
            LightTexture = null;

            if (LightSurface != null && !LightSurface.Disposed)
                LightSurface.Dispose();
            LightSurface = null;
        }

        public static bool EmptyCell(Point P)
        {
            if ((M2CellInfo[P.X, P.Y].BackImage & 0x20000000) != 0)// + (M2CellInfo[P.X, P.Y].FrontImage & 0x7FFF) != 0)
                return false;

            return !ObjectList.Any(O => O.Location == P && O.Blocking);
        }
        public static bool CanWalk(MirDirection Dir)
        {
            return EmptyCell(Functions.PointMove(MapObject.User.Location, Dir, 1));
        }
        public static bool CanRun(MirDirection Dir)
        {
            return CanWalk(Dir) && EmptyCell(Functions.PointMove(MapObject.User.Location, Dir, 2));
        }


        public class Mir2CellInfo
        {
            public int BackImage;
            public short MiddleImage;
            public short FrontImage;

            public byte DoorIndex;
            public byte DoorOffset;
            public byte AnimationFrame;
            public byte AnimationTick;
            public byte FileIndex;
            public byte Light;
            public byte Unknown;
        }

        internal static void AddObject(MapObject O)
        {
            ObjectList.Add(O);
        }
    }
}
