using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Structures
{
    public struct Line
    {
        public Vector2 Start;
        public Vector2 End;

        public Line(Point start, Point end)
        {
            Start = start.ToVector2();
            End = end.ToVector2();
        }
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
        public Rectangle toRectangle()
        {
            return Start.X < End.X ? new Rectangle(Start.ToPoint(), (End - Start).ToPoint()) :
                new Rectangle(End.ToPoint(), (Start - End).ToPoint());
        }
    }
}
