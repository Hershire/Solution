using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX.DirectSound;

namespace Client.MirSound
{
    class SoundManager
    {
        private static Dictionary<SoundList, SoundLibrary> Sounds = new Dictionary<SoundList, SoundLibrary>();
        public static Dictionary<SoundList, string> IndexList = new Dictionary<SoundList, string>();
        public static Device Device { get; private set; }
        private static Control Target;

        static SoundManager()
        {
            Target = Main.This;
            Device = new Device();
            Device.SetCooperativeLevel(Target, CooperativeLevel.Normal);
            LoadSoundList();
        }
        public static void LoadSoundList()
        {
            string FileName = Path.Combine(Settings.SoundPath, "mirsound.lst");

            if (!File.Exists(FileName)) return;

            string[] Lines = File.ReadAllLines(FileName);

            int Index;
            string[] Split;
            for (int I = 0; I < Lines.Length; I++)
            {
                Split = Lines[I].Replace(" ", "").Split(':', '\t');

                if (Split.Length <= 1 || !int.TryParse(Split[0], out Index)) continue;

                if (!IndexList.ContainsKey((SoundList)Index))
                    IndexList.Add((SoundList)Index, Split[Split.Length - 1].Replace("wav\\", ""));
            }

            IndexList.Add(SoundList.LoginMusic, "log-in-long2.wav");
            IndexList.Add(SoundList.SelectMusic, "sellect-loop2.wav");
        }

        public static void StopSound(SoundList Sound)
        {
            if (Sounds.ContainsKey(Sound))
                Sounds[Sound].Stop();
        }
        public static void PlaySound(SoundList Sound, bool Loop)
        {
            if (!Settings.SoundOn) return;

            if (Sounds.ContainsKey(Sound))
            {
                Sounds[Sound].Play();
                return;
            }

            if (IndexList.ContainsKey(Sound))
                PlaySound(Sound, IndexList[Sound], Loop);
        }
        private static void PlaySound(SoundList Sound, string FileName, bool Loop)
        {
            FileName = Path.Combine(Settings.SoundPath, FileName);

            if (!File.Exists(FileName)) return;

            Sounds.Add(Sound, new SoundLibrary(FileName, Loop));
        }


        public static void Dispose()
        {
            foreach (var Ob in Sounds.Values)
            {
                if (Ob != null)
                    Ob.Dispose();
            }
            if (Device != null && !Device.Disposed)
                Device.Dispose();

        }
    }
}
