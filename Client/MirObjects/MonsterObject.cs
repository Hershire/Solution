using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Client.MirGraphics;
using Client.MirSound;
using System.Drawing;

namespace Client.MirObjects
{
    class MonsterObject :MapObject
    {
        public MonsterObject(MonsterDetails D)
        {
            if (D == null) return;

            WeaponShape = -1;

            ObjectID = D.ObjectID;
            Name = D.Name;
            Direction = D.Direction;
            Location = D.Location;
            ActualLocation = D.Location;
            Effect = D.Effect;
            Dead = D.Dead;

            Frames = MonsterFrames.Monster[1];
            PrimaryLibrary = Libraries.Monsters[D.Image / 10];


            switch (D.Image)
            {
                case 0:
                    break;
                case 1:
                    Frames = MonsterFrames.Monster[0];
                    BaseIndex = 280;
                    break;
                case 25:
                case 26:
                case 27:
                case 100:
                case 160:
                case 161:
                case 162:
                case 163:
                    BaseIndex = 360 * (D.Image % 10);
                    break;

            }


            PopupSound = (SoundList)(200 + D.Image * 10);
            AppearSound = (SoundList)(200 + D.Image * 10 + 1);
            AttackSound = (SoundList)(200 + D.Image * 10 + 2);
            //AppearSound = 200 + P.Details.Image * 10 + 3; Spell Sound
            FlinchSound = (SoundList)(200 + D.Image * 10 + 4);
            DieSound = (SoundList)(200 + D.Image * 10 + 5);

            if (Dead)
                DoAction(MirAction.Dead);
            else
            {
                PlayAppearSound();
                DoAction(MirAction.Standing);
                Frame = Main.Rand.Next(CurrentFrame.Count);
            }
        }

        public override void DrawEffect()
        {
            switch (CurrentAction)
            {
                case MirAction.Die:
                    switch (Effect)
                    {
                        case 1: // Scarecrow
                            PrimaryLibrary.DrawBlend(2860 + Frame, DisplayRectangle.Location, Color.White, true);
                            break;
                    }
                    break;
            }

        }

    }
}
