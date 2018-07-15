using Microsoft.Xna.Framework;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.NetworkData
{
    [Serializable]
    public class LineData
    {
        float[] start = new float[2];
        float[] end = new float[2];

        public Line Line {
            get
            {
                return new Line(new Vector2(start[0], start[1]), new Vector2(end[0], end[1]));
            }
            set
            {
                start[0] = value.Start.X;
                start[1] = value.Start.Y;
                end[0] = value.End.X;
                end[1] = value.End.Y;
            }
        }

        public Color Color { get { return new Color(colorPackedValue); } set { colorPackedValue = value.PackedValue; } }

        uint colorPackedValue;
        public int Thickness;

        public LineData(Line line, Color color, int thickness)
        {
            Line = line;

            Color = color;
            Thickness = thickness;
        }
        
    }
}
