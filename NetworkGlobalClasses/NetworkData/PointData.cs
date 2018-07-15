using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.NetworkData
{
    [Serializable]
    public class PointData
    {
        float[] point = new float[2];

        public Vector2 Vector {
            get
            {
                return new Vector2(point[0], point[1]);
            }
            set
            {
                point[0] = value.X;
                point[1] = value.Y;
            }
        }

        public Color Color { get { return new Color(colorPackedValue); } set { colorPackedValue = value.PackedValue; } }

        uint colorPackedValue;
        public int Thickness;

        public PointData(Vector2 vector)
        {
            Vector = vector;
        }
    }
}
