using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Library
{
    public class InIReader
    {
        #region Fields

        private readonly string FileName;
        public bool AutoSave;
        private readonly List<string> Contents;

        #endregion

        #region Constructor

        public InIReader(string _FileName)
        {
            FileName = _FileName;
            AutoSave = true;
            Contents = new List<string>();
            try
            {
                if (File.Exists(FileName))
                    Contents.AddRange(File.ReadAllLines(FileName));
            }
            catch
            {
                //TODO: Error Handling.
            }
        }

        #endregion

        #region Functions

        private string FindValue(string Section, string Key)
        {
            for (int A = 0; A < Contents.Count; A++)
                if (string.Compare(Contents[A], "[" + Section + "]") == 0)
                    for (int B = A + 1; B < Contents.Count; B++)
                        if (string.Compare(Contents[B].Split('=')[0], Key) == 0)
                            return Contents[B].Split('=')[1];
                        else if (Contents[B].StartsWith("[") && Contents[B].EndsWith("]"))
                            return null;
            return null;
        }

        private int FindIndex(string Section, string Key)
        {
            for (int A = 0; A < Contents.Count; A++)
                if (string.Compare(Contents[A], "[" + Section + "]") == 0)
                    for (int B = A + 1; B < Contents.Count; B++)
                        if (string.Compare(Contents[B].Split('=')[0], Key) == 0)
                            return B;
                        else if (Contents[B].StartsWith("[") && Contents[B].EndsWith("]"))
                        {
                            Contents.Insert(B, Key + "=");
                            return B;
                        }
                        else if (Contents.Count - 1 == B)
                        {
                            Contents.Add(Key + "=");
                            return Contents.Count - 1;
                        }
            if (Contents.Count > 0)
                Contents.Add("");

            Contents.Add("[" + Section + "]");
            Contents.Add(Key + "=");
            return Contents.Count - 1;
        }

        public void Save()
        {
            try
            {
                File.WriteAllLines(FileName, Contents);
            }
            catch
            {
                //TODO: Error Handling.
            }
        }

        #endregion

        #region Read

        public bool ReadBoolean(string Section, string Key, bool Default)
        {
            bool Result;

            if (!bool.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public byte ReadByte(string Section, string Key, byte Default)
        {
            byte Result;

            if (!byte.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        public sbyte ReadSByte(string Section, string Key, sbyte Default)
        {
            sbyte Result;

            if (!sbyte.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        public ushort ReadUInt16(string Section, string Key, ushort Default)
        {
            ushort Result;

            if (!ushort.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        public short ReadInt16(string Section, string Key, short Default)
        {
            short Result;

            if (!short.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        public uint ReadUInt32(string Section, string Key, uint Default)
        {
            uint Result;

            if (!uint.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public int ReadInt32(string Section, string Key, int Default)
        {
            int Result;

            if (!int.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public ulong ReadUInt64(string Section, string Key, ulong Default)
        {
            ulong Result;

            if (!ulong.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public long ReadInt64(string Section, string Key, long Default)
        {
            long Result;

            if (!long.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        public float ReadSingle(string Section, string Key, float Default)
        {
            float Result;

            if (!float.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public double ReadDouble(string Section, string Key, double Default)
        {
            double Result;

            if (!double.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public decimal ReadDecimal(string Section, string Key, decimal Default)
        {
            decimal Result = Default;

            if (!decimal.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public string ReadString(string Section, string Key, string Default)
        {
            string Result = FindValue(Section, Key);

            if (string.IsNullOrEmpty(Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public char ReadChar(string Section, string Key, char Default)
        {
            char Result;

            if (!char.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }

            return Result;
        }

        public Point ReadPoint(string Section, string Key, Point Default)
        {
            string Temp = FindValue(Section, Key);
            int TempX = 0, TempY = 0;
            if (Temp == null || !int.TryParse(Temp.Split(',')[0], out TempX))
            {
                Write(Section, Key, Default);
                return Default;
            }
            if (!int.TryParse(Temp.Split(',')[1], out TempY))
            {
                Write(Section, Key, Default);
                return Default;
            }

            return new Point(TempX, TempY);
        }

        public Size ReadSize(string Section, string Key, Size Default)
        {
            string Temp = FindValue(Section, Key);
            int TempX = 0, TempY = 0;
            if (!int.TryParse(Temp.Split(',')[0], out TempX))
            {
                Write(Section, Key, Default);
                return Default;
            }
            if (!int.TryParse(Temp.Split(',')[1], out TempY))
            {
                Write(Section, Key, Default);
                return Default;
            }

            return new Size(TempX, TempY);
        }

        public TimeSpan ReadTimeSpan(string Section, string Key, TimeSpan Default)
        {
            TimeSpan Result = Default;

            if (!TimeSpan.TryParse(FindValue(Section, Key), out Result))
            {
                Result = Default;
                Write(Section, Key, Default);
            }


            return Result;
        }

        #endregion

        #region Write

        public void Write(string Section, string Key, bool Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, byte Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, sbyte Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, ushort Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, short Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, uint Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, int Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, ulong Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, long Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, float Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, double Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, decimal Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, string Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, char Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, Point Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value.X + "," + Value.Y;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, Size Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value.Width + "," + Value.Height;
            if (AutoSave) Save();
        }

        public void Write(string Section, string Key, TimeSpan Value)
        {
            Contents[FindIndex(Section, Key)] = Key + "=" + Value;
            if (AutoSave) Save();
        }

        #endregion
    }
}
