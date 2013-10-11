using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library;
using Server.MirObjects;

namespace Server.MirEnvir
{
    class MapCell
    {
        public static MapCell HighWall = new MapCell { Attribute = CellAttribute.HighWall };
        public static MapCell LowWall = new MapCell { Attribute = CellAttribute.LowWall };

        public List<MapObject> Objects;
        public CellAttribute Attribute;

        public bool Valid
        {
            get { return Attribute == CellAttribute.Walk; }
        }
        public bool Blocking
        {
            get { return Objects != null && Objects.Any(Ob => Ob.Blocking); }
        }

        public void AddObject(MapObject Ob)
        {
            lock (this)
            {
                if (Objects == null)
                    Objects = new List<MapObject>();
                Objects.Add(Ob);
            }

        }
        public void RemoveObject(MapObject Ob)
        {
            lock (this)
            {
                if (Objects == null) return;

                Objects.Remove(Ob);
                if (Objects.Count == 0)
                    Objects = null;
            }
        }
    }
}
