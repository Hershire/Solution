using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Client.MirGraphics
{
    class ImageLibrary
    {
        private const string WixExtention = ".Wix",
                             WilExtention = ".Wil";

        public string FileName { get; private set; }
        private bool Initialized, Attempted;
        public int ImageCount { get; private set; }

        public int ColourCount { get; private set; }
        public int PaletteSize { get; private set; }
        public int Version { get; private set; }

        private ImageInfo[] ImageList;
        private List<int> IndexList;
        private int[] Palette;

        private BinaryReader BReader;
        private FileStream FStream;

        public ImageLibrary(string FileName)
        {
            this.FileName = Path.Combine(Settings.DataPath, FileName);
            Initialized = false;
        }
        public bool Initialize()
        {
            if (Initialized) return true;
            if (Attempted) return false;
            Attempted = true;


            if (string.IsNullOrWhiteSpace(FileName))
                throw new IOException("No File assigned");

            if (!File.Exists(FileName + WixExtention))
                return false;

            if (!File.Exists(FileName + WilExtention))
                return false;

            try
            {
                FStream = new FileStream(FileName + WilExtention, FileMode.Open, FileAccess.Read);
                BReader = new BinaryReader(FStream);

                LoadImageInfo();
            }
            catch (Exception)
            {
                return false;
            }
            Initialized = true;

            return true;

        }

        private void LoadIndexFile()
        {
            IndexList = new List<int>();
            FileStream FStream = null;

            try
            {
                FStream = new FileStream(FileName + WixExtention, FileMode.Open, FileAccess.Read);

                FStream.Seek(Version == 0 ? 48 : 52, SeekOrigin.Begin);


                using (BinaryReader BReader = new BinaryReader(FStream))
                {
                    FStream = null;
                    while (BReader.BaseStream.Position <= BReader.BaseStream.Length - 4)
                        IndexList.Add(BReader.ReadInt32());
                }
            }
            finally
            {
                if (FStream != null)
                    FStream.Dispose();
            }
        }
        private void LoadImageInfo()
        {
            FStream.Seek(44, SeekOrigin.Begin);

            ImageCount = BReader.ReadInt32();
            ColourCount = BReader.ReadInt32();
            PaletteSize = BReader.ReadInt32();
            Version = BReader.ReadInt32();


            Palette = new int[ColourCount];

            LoadIndexFile();

            ImageList = new ImageInfo[IndexList.Count];

            FStream.Seek(Version == 0 ? 0 : 4, SeekOrigin.Current);

            for (int I = 1; I < Palette.Length; I++)
            {
                Palette[I] = BReader.ReadInt32() + (255 << 24);
            }
        }
        private void LoadImageInfo(int Index)
        {
            FStream.Position = IndexList[Index];
            ImageList[Index] = new ImageInfo(BReader);
        }
        public Point GetOffSet(int Index)
        {
            if (!Initialize()) return Point.Empty;
            if (Index < 0 || Index >= ImageList.Length || IndexList[Index] <= 0) return Point.Empty;
            if (ImageList[Index] == null)
                LoadImageInfo(Index);
            return ImageList[Index].OffSet;
        }
        public Size GetSize(int Index)
        {
            if (!Initialize()) return Size.Empty;
            if (Index < 0 || Index >= ImageList.Length || IndexList[Index] <= 0) return Size.Empty;
            if (ImageList[Index] == null)
                LoadImageInfo(Index);

            return ImageList[Index].Size;
        }

        public void Draw(int Index, Point Location, Color Colour, float Opacity = 1F, bool OffSet = false)
        {
            if (!CheckImage(Index)) return;

            if (OffSet) Location.Offset(ImageList[Index].OffSet);

            float OldOpacity = DXManager.Opacity;

            if (Opacity < 1F)
                DXManager.SetOpacity(Opacity);

            DXManager.Sprite.Draw2D(ImageList[Index].ImageTexture, Point.Empty, 0, Location, Colour.ToArgb());

            if (Opacity < 1F)
                DXManager.SetOpacity(OldOpacity);
        }

        public void Draw(int Index, Rectangle Section, Point Location, Color Colour, float Opacity = 1F, bool OffSet = false)
        {
            if (!CheckImage(Index)) return;

            if (OffSet) Section.Offset(ImageList[Index].OffSet);

            if (Section.Right > ImageList[Index].Size.Width)
                Section.Width -= Section.Right - ImageList[Index].Size.Width;

            if (Section.Bottom > ImageList[Index].Size.Height)
                Section.Height -= Section.Bottom - ImageList[Index].Size.Height;

            float OldOpacity = DXManager.Opacity;

            if (Opacity < 1F)
                DXManager.SetOpacity(Opacity);

            DXManager.Sprite.Draw2D(ImageList[Index].ImageTexture, Section, Section.Size, Location, Colour);

            if (Opacity < 1F)
                DXManager.SetOpacity(OldOpacity);
        }

        public void DrawBlend(int Index, Point Location, Color Colour, bool OffSet = false)
        {
            if (!CheckImage(Index)) return;

            if (OffSet) Location.Offset(ImageList[Index].OffSet);

            DXManager.SetBlend(true);
            DXManager.Sprite.Draw2D(ImageList[Index].ImageTexture, Point.Empty, 0, Location, Colour);
            DXManager.SetBlend(false);
        }

        private bool CheckImage(int Index)
        {
            if (!Initialize()) return false;

            if (Index < 0 || Index >= ImageList.Length || IndexList[Index] <= 0) return false;

            if (ImageList[Index] == null)
                LoadImageInfo(Index);

            if (!ImageList[Index].TextureValid)
                LoadTexture(Index);

            return true;
        }
        private void LoadTexture(int Index)
        {
            FStream.Seek(IndexList[Index] + (Version == 0 ? 8 : 12), SeekOrigin.Begin);
            ImageList[Index].CreateTexture(BReader, Palette);

        }

        public bool VisiblePixel(int Index, Point P)
        {
            return CheckImage(Index) && ImageList[Index].VisiblePixel(P);
        }
    }

    class ImageInfo
    {
        public Size Size;
        public Point OffSet;
        private DateTime LastAccess;

        public bool TextureValid
        {
            get { return ImageTexture != null && !ImageTexture.Disposed; }
        }

        public Texture ImageTexture;

        internal ImageInfo(BinaryReader BReader)
        {
            Size = new Size(BReader.ReadInt16(), BReader.ReadInt16());
            OffSet = new Point(BReader.ReadInt16(), BReader.ReadInt16());
        }

        public void CreateTexture(BinaryReader BReader, int[] Palette)
        {
            if (ImageTexture != null && !ImageTexture.Disposed)
                ImageTexture.Dispose();

            byte[] Bytes = FullImageBytes(BReader, Palette);
            if (Bytes == null) return;

            LastAccess = Main.Now;
            ImageTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 0, Usage.None, Format.A8R8G8B8, Pool.Managed);

            GraphicsStream GS = ImageTexture.LockRectangle(0, LockFlags.Discard);
            GS.Write(Bytes);
            GS.Dispose();
            ImageTexture.UnlockRectangle(0);

        }

        private unsafe byte[] FullImageBytes(BinaryReader BReader, int[] Palette)
        {
            byte[] Pixels = new byte[Size.Width * Size.Height * 4];
            byte[] FileBytes = BReader.ReadBytes(Size.Width * Size.Height);

            int Index = 0;

            fixed (byte* P1 = Pixels)
            {
                byte* Pi;
                for (int Y = Size.Height - 1; Y >= 0; Y--)
                {
                    Pi = P1 + Y * Size.Width * 4;
                    for (int X = 0; X < Size.Width; X++)
                    {
                        *((int*)Pi) = Palette[FileBytes[Index++]];
                        Pi += 4;
                    }
                }
            }

            return Pixels;
        }
        internal bool VisiblePixel(Point P)
        {
            bool Result = false;
            if (P.X < 0 || P.Y < 0 || P.X >= Size.Width || P.Y >= Size.Height) return false;

            if (ImageTexture != null)
            {
                GraphicsStream GS = ImageTexture.LockRectangle(0, LockFlags.ReadOnly);
                unsafe
                {
                    byte* ImageBytes = (byte*)GS.InternalData;
                    Result = ImageBytes[(P.Y * Size.Width + P.X) * 4 + 3] != 0;
                }
                GS.Dispose();
                ImageTexture.UnlockRectangle(0);
            }
            return Result;
        }

        /*   private byte[] FullImageBytes(BinaryReader BReader)
           {
               byte[] Pixels = new byte[Size.Width * Size.Height * 4];
               byte[] FileBytes = null;// BReader.ReadBytes(Length * 2);

               int End = 0, OffSet = 0, Count;

               for (int Y = 0; Y < Size.Height; Y++)
               {
                   OffSet = End;
                   End += (FileBytes[OffSet + 1] << 8 | FileBytes[OffSet]) * 2 + 2;
                   OffSet += 2; //New line
                   for (int X = 0; X < Size.Width; )
                       switch (FileBytes[OffSet])
                       {
                           case 192: //No Colour
                               X += FileBytes[OffSet + 3] << 8 | FileBytes[OffSet + 2];
                               OffSet += 4;
                               break;
                           case 193: //Solid Colour
                           case 194: //Overlay Colour
                           case 195: // ??
                               Count = FileBytes[OffSet + 3] << 8 | FileBytes[OffSet + 2];
                               OffSet += 4;
                               for (int I = 0; I < Count; I++)
                               {
                                   int ColIndex = FileBytes[OffSet + 1] << 8 | FileBytes[OffSet];
                                   OffSet += 2;
                                   if (X >= Size.Width) continue;
                                   Pixels[(Y * Size.Width + X) * 4 + 3] = 255;
                                   Pixels[(Y * Size.Width + X) * 4 + 2] = (byte)((ColIndex / 2048) * 8 + 7);
                                   Pixels[(Y * Size.Width + X) * 4 + 1] = (byte)(((ColIndex %= 2048) / 32) * 4);
                                   Pixels[(Y * Size.Width + X) * 4] = (byte)((ColIndex % 32) * 8 + 7);
                                   X++;
                               }
                               break;
                           default:
                               return null;
                       }
               }


               return Pixels;
           }*/
    }
}
