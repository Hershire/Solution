using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Library
{
    public static class Functions
    {
        public static T ReadStruct<T>(Stream Stream)
        {
            byte[] Bytes = new byte[Marshal.SizeOf(typeof(T))];

            Stream.Read(Bytes, 0, Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(Bytes, GCHandleType.Pinned);
            T Temp = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return Temp;
        }
        public static bool CompareBytes(byte[] A, byte[] B)
        {
            if (A == B) return true;

            if (A == null || B == null || A.Length != B.Length)
                return false;

            for (int I = 0; I < A.Length; I++)
                if (A[I] != B[I]) return false;

            return true;
        }
        public static Point PointA(Point P1, Point P2)
        {
            return new Point(P1.X + P2.X, P1.Y + P2.Y);
        }
        public static Point PointS(Point P1, Point P2)
        {
            return new Point(P1.X - P2.X, P1.Y - P2.Y);
        }
        public static Point PointM(Point P1, Point P2)
        {
            return new Point(P1.X * P2.X, P1.Y * P2.Y);
        }
        public static Point PointM(Point P1, int X, int Y)
        {
            return new Point(P1.X * X, P1.Y * Y);
        }
        public static Point PointMove(Point P, MirDirection D, int I)
        {
            switch (D)
            {
                case MirDirection.Up:
                    P.Offset(0, -I);
                    break;
                case MirDirection.UpRight:
                    P.Offset(I, -I);
                    break;
                case MirDirection.Right:
                    P.Offset(I, 0);
                    break;
                case MirDirection.DownRight:
                    P.Offset(I, I);
                    break;
                case MirDirection.Down:
                    P.Offset(0, I);
                    break;
                case MirDirection.DownLeft:
                    P.Offset(-I, I);
                    break;
                case MirDirection.Left:
                    P.Offset(-I, 0);
                    break;
                case MirDirection.UpLeft:
                    P.Offset(-I, -I);
                    break;
            }
            return P;
        }
        public static MirDirection DirectionFromPoint(Point Source, Point Dest)
        {
            if (Source.X < Dest.X)
            {
                if (Source.Y < Dest.Y)
                    return MirDirection.DownRight;
                else if (Source.Y > Dest.Y)
                    return MirDirection.UpRight;
                return MirDirection.Right;
            }

            if (Source.X > Dest.X)
            {
                if (Source.Y < Dest.Y)
                    return MirDirection.DownLeft;
                else if (Source.Y > Dest.Y)
                    return MirDirection.UpLeft;
                return MirDirection.Left;
            }

            if (Source.Y < Dest.Y)
                return MirDirection.Down;
            return MirDirection.Up;

        }

        public static string PointToString(Point P)
        {
            return string.Format("{0}, {1}", P.X, P.Y);
        }
        public static Point StringToPoint(string S)
        {
            int TempX, TempY;

            if (string.IsNullOrWhiteSpace(S) || S.Split(',').Length <= 1) return Point.Empty;

            if (!int.TryParse(S.Split(',')[0], out TempX))
                return Point.Empty;

            return !int.TryParse(S.Split(',')[1], out TempY) ? Point.Empty : new Point(TempX, TempY);
        }

        public static string SizeToString(Size S)
        {
            return string.Format("{0}, {1}", S.Width, S.Height);
        }
        public static Size StringToSize(string S)
        {
            int TempW, TempH;

            if (string.IsNullOrWhiteSpace(S) || S.Split(',').Length <= 1) return Size.Empty;

            if (!int.TryParse(S.Split(',')[0], out TempW))
                return Size.Empty;

            return !int.TryParse(S.Split(',')[1], out TempH) ? Size.Empty : new Size(TempW, TempH);
        }

        public static bool InRange(Point A, Point B, int I)
        {
            return Math.Abs(A.X - B.X) <= I && Math.Abs(A.Y - B.Y) <= I;
        }

        public static bool TryParse(string S, out Point P)
        {
            P = Point.Empty;
            int TempX, TempY;

            if (string.IsNullOrWhiteSpace(S) || S.Split(',').Length <= 1) return false;

            if (!int.TryParse(S.Split(',')[0], out TempX))
                return false;

            if (!int.TryParse(S.Split(',')[1], out TempY))
                return false;

            P = new Point(TempX, TempY);
            return true;
        }
    }
}
