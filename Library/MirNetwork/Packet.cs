using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.DirectX.Direct3D;

namespace Library.MirNetwork
{
    //| 2Bytes: Packet Size | 2Bytes: Packet ID | Packet Data |

    public abstract class Packet
    {
        #region Static Region
        private static List<Type> PacketList;
        static Packet()
        {
            PacketList = new List<Type>();
            foreach (Type T in Assembly.GetExecutingAssembly().GetTypes().Where(T => T.BaseType == typeof(Packet)))
                PacketList.Add(T);
        }
        #endregion

        private short Index
        {
            get
            {
                return (short)PacketList.IndexOf(GetType());
            }
        }

        public static Packet ReceivePacket(byte[] RawBytes, out byte[] Extra)
        {
            Extra = RawBytes;

            Packet P = null;

            if (RawBytes.Length < 4) return P; //| 2Bytes: Packet Size | 2Bytes: Packet ID |

            int Length = (RawBytes[1] << 8) + RawBytes[0];

            if (Length > RawBytes.Length) return P; 

            using (MemoryStream MStream = new MemoryStream(RawBytes, 2, Length - 2))
            using (BinaryReader BReader = new BinaryReader(MStream))
            {
                short ID = BReader.ReadInt16();

                P = (Packet)Activator.CreateInstance(PacketList[ID]);
                P.ReadPacket(BReader);
            }

            Extra = new byte[RawBytes.Length - Length];
            Buffer.BlockCopy(RawBytes, Length, Extra, 0, RawBytes.Length - Length);

            return P;
        }

        public byte[] GetPacketBytes()
        {
            if (Index < 0) return new byte[0];

            byte[] Data = null;

            using (MemoryStream MStream = new MemoryStream())
            {
                MStream.SetLength(2);
                MStream.Seek(2, SeekOrigin.Begin);
                using (BinaryWriter BWriter = new BinaryWriter(MStream))
                {
                    BWriter.Write((short)Index);
                    WritePacket(BWriter);
                    MStream.Seek(0, SeekOrigin.Begin);
                    BWriter.Write((short)MStream.Length);
                    MStream.Seek(0, SeekOrigin.Begin);

                    Data = new byte[MStream.Length];
                    MStream.Read(Data, 0, Data.Length);
                }
            }

            return Data;
        }

        protected abstract void ReadPacket(BinaryReader BReader);
        protected abstract void WritePacket(BinaryWriter BWriter);

    }

}
