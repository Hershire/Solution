using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.DirectX.DirectSound;


namespace Client.MirSound
{
    public enum SoundList : int
    {
            None,      
      
            //Interface
            LoginMusic = -1,
            SelectMusic = -2,
            LoginEffect = 100,
            ClickA = 103,
            ClickB = 104,
            ClickC = 105,
            Gold = 106,
            EatDrug = 107,
            ClickDrug = 108,
            TeleportOut = 109,
            TeleportIn = 110,
            
            ClickWeapon = 111,
            ClickArmour = 112,
            ClickRing = 113,
            ClickBracelet = 114,
            ClickNecklace = 115,
            ClickHelemt = 116,
            ClickBoots = 117,
            ClickItem = 118,
         
            //Movement
            WalkGroundL = 1, WalkGroundR = 2, RunGroundL = 3, RunGroundR = 4,
            WalkStoneL = 5, WalkStoneR = 6, RunStoneL = 7, RunStoneR = 8,
            WalkLawnL = 9, WalkLawnR = 10, RunLawnL = 11, RunLawnR = 12,
            WalkRoughL = 13, WalkRoughR = 14, RunRoughL = 15, RunRoughR = 16,
            WalkWoodL = 17, WalkWoodR = 18, RunWoodL = 19, RunWoodR = 20,
            WalkCaveL = 21, WalkCaveR = 22, RunCaveL = 23, RunCaveR = 24,
            WalkRoomL = 25, WalkRoomR = 26, RunRoomL = 27, RunRoomR = 28,
            WalkWaterL = 29, WalkWaterR = 30, RunWaterL = 31, RunWaterR = 32,

            //Weapon Swing
            SwingShort = 50,
            SwingWood = 51,
            SwingSword = 52,
            SwingSword2 = 53,
            SwingAxe = 54,
            SwingClub = 55,
            SwingLong = 56,
            SwingFist = 56,

            //Struck
            StruckShort = 60,
            StruckWooden = 61,
            StruckSword = 62,
            StruckSword2 = 63,
            StruckAxe = 64,
            StruckClub = 65,
            
            StruckBodySword = 70,
            StruckBodyAxe = 71,
            StruckBodyLongStick = 72,
            StruckBodyFist = 73,
            
            StruckArmourSword = 80,
            StruckArmourAxe = 81,
            StruckArmourLongStick = 82,
            StruckArmourFist = 83,
         
            //Skills
            MaleSlaying = 130,
            FemaleSlaying = 131,
            Thrusting = 132,
            Halfmoon = 133,
            RushL = 134,
            RushR = 135,
            FlameSwordReady = 136,
            FlameSword = 137,
            CrossHalfMoon = 140,
            TwinDrakeBlade = 141,
            
            
            MaleFlinch = 138,
            FemaleFlinch = 139,
            MaleDie = 144,
            FemaleDie = 145
    }
    class SoundLibrary : IDisposable
    {
        private List<SecondaryBuffer> BufferList;

        private MemoryStream MStream;
        private bool Loop;

        public SoundLibrary(string FileName, bool L)
        {
            MStream = new MemoryStream(File.ReadAllBytes(FileName), true);
           
            Loop = L;

            BufferList = new List<SecondaryBuffer>();

            Play();
        }

        public void Play()
        {
            if (MStream == null) return;

            MStream.Seek(0, SeekOrigin.Begin);

            if (Loop)
            {
                if (BufferList.Count == 0)
                    BufferList.Add(new SecondaryBuffer(MStream, SoundManager.Device));
                else if (BufferList[0] == null || BufferList[0].Disposed)
                    BufferList[0] = new SecondaryBuffer(MStream, SoundManager.Device);

                if (!BufferList[0].Status.Playing)
                    BufferList[0].Play(0, BufferPlayFlags.Looping);
            }
            else
            {
                for (int I = BufferList.Count - 1; I >= 0; I--)
                {
                    if (BufferList[I] == null || BufferList[I].Disposed)
                    {
                        BufferList.RemoveAt(I);
                        continue;
                    }

                    if (!BufferList[I].Status.Playing)
                    {
                        BufferList[I].Play(0, BufferPlayFlags.Default);
                        return;
                    }
                }

                if (BufferList.Count >= Settings.SoundOverLap) return;

                SecondaryBuffer TempB = new SecondaryBuffer(MStream, new BufferDescription { BufferBytes = (int)MStream.Length }, SoundManager.Device);
                TempB.Play(0, BufferPlayFlags.Default);
                BufferList.Add(TempB);
            }

        }
        public void Stop()
        {
            if (BufferList.Count == 0) return;

            if (Loop)
                BufferList[0].Dispose();
            else
            {
                for (int I = 0; I < BufferList.Count; I++)
                    BufferList[I].Dispose();
                BufferList.Clear();
            }
        }

        public void Dispose()
        {
            if (MStream != null)
                MStream.Dispose();
            MStream = null;

            for (int I = 0; I < BufferList.Count; I++)
                BufferList[I].Dispose();
            BufferList = null;

            Loop = false;
        }
    }

}
