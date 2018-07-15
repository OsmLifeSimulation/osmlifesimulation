using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global
{
    public class PointDrawable
    {
        Point Point;
        //Color Color;
        //int Size;
        public PointDrawable(Point point/*, int size, Color color*/)
        {
            Point = point;
            //Size = size;
            //Color = color;
        }

        public void Draw(SpriteBatch spriteBatch, Color Color, int Size, Texture2D texture)
        {
            spriteBatch.Draw(texture, new Rectangle(Point, new Point(Size, Size)), Color);
        }
    }
}
