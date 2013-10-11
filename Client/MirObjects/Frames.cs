using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MirObjects
{
    class FrameSet
    {
        public Frame Stand;
        public Frame Walk;
        public Frame Harvest;
        public Frame Run;
        public Frame Attack1;
        public Frame Attack2;
        public Frame Attack3;
        public Frame Stance;
        public Frame Struck;
        public Frame Die;
        public Frame Dead;
        public Frame Skeleton;
    }

    class Frame
    {
        public int Start, Count, Skip;
        public int Interval;
        public int OffSet
        {
            get
            {
                return Count + Skip;
            }
        }
        public Frame(int Start, int Count, int Skip, int Interval)
        {
            this.Start = Start;
            this.Count = Count;
            this.Skip = Skip;
            this.Interval = Interval;
        }
    }

    class HumanFrames : FrameSet
    {
        public static HumanFrames This = new HumanFrames();

        private HumanFrames()
        {
            Stand = new Frame(0, 4, 4, 450);
            Harvest = new Frame(456, 2, 0, 200);
            Walk = new Frame(64, 6, 2, 100);
            Run = new Frame(128, 6, 2, 100);
            Stance = new Frame(192, 1, 0, 2000);
            Attack1 = new Frame(200, 6, 2, 100);
            Attack2 = new Frame(264, 6, 2, 100);
            Attack3 = new Frame(328, 8, 0, 80);
            Struck = new Frame(472, 3, 5, 100);
            Die = new Frame(536, 4, 4, 100);
            Dead = new Frame(539, 1, 7, 1000);
            Skeleton = Dead;
        }
    }

    class MonsterFrames : FrameSet
    {
        public static MonsterFrames[] Monster = { Monster01(), Monster02() };

        private static MonsterFrames Monster01()
        {
            return new MonsterFrames
            {
                Stand = new Frame(0, 4, 4, 1000),
                Walk = new Frame(64, 6, 2, 100),
                Attack1 = new Frame(128, 6, 2, 100),
                Struck = new Frame(192, 2, 0, 200),
                Die = new Frame(208, 4, 4, 100),
                Dead = new Frame(211, 1, 7, 1000),
            };
        }
        private static MonsterFrames Monster02()
        {
            return new MonsterFrames
            {
                Stand = new Frame(0, 4, 6, 1000),
                Walk = new Frame(80, 6, 4, 100),
                Attack1 = new Frame(160, 6, 4, 100),
                Struck = new Frame(240, 2, 0, 200),
                Die = new Frame(260, 10, 0, 100),
                Dead = new Frame(269, 1, 9, 1000),
                Skeleton = new Frame(340, 1, 0, 1000),
            };
        }
    }
}
