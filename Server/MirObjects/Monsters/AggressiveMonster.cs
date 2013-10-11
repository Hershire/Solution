using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.MirEnvir;

namespace Server.MirObjects.Monsters
{
    class AgressiveMonster : MonsterObject
    {
        public AgressiveMonster(MapRespawn R)
            : base(R)
        {
        }

        public override void FindTarget()
        {
            int SearchX = Location.X;
            int SearchY = Location.Y;

            for (int Dist = 0; Dist < ViewRange; Dist++)
            {
                for (int Y = SearchY - Dist; Y <= SearchY + Dist; Y++)
                {
                    if (Y < 0) continue;
                    if (Y >= CurrentMap.MapSize.Height) break;

                    for (int X = SearchX - Dist; X <= SearchX + Dist; X += (Y == SearchY - Dist || Y == SearchY + Dist) ? 1 : Dist * 2)
                    {
                        if (X < 0) continue;
                        if (X >= CurrentMap.MapSize.Width) break;
                        if (!CurrentMap.ValidPoint(X, Y) || CurrentMap.MapCells[X, Y].Objects == null) continue;
                        List<MapObject> Objects = CurrentMap.MapCells[X, Y].Objects;
                        for (int I = 0; I < Objects.Count; I++)
                        {
                            MapObject Ob = Objects[I];
                            if (!Ob.Dead && IsAttackTarget(Ob))
                            {
                                Target = Ob;
                                return;
                            }
                        }
                    }
                }
            }

        }
    }
}
