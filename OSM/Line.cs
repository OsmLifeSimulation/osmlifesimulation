using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    public class Line
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
        public void Draw(SpriteBatch spriteBatch, int thickness, Color color, Texture2D texture)
        {
            Vector2 edge = End - Start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            spriteBatch.Draw(texture,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)Start.X,
                    (int)Start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    thickness), //width of line, change this to make thicker line
                null,
                color, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0, 0), // point in line about which to rotate
                SpriteEffects.None,
                0);
        }
    }
}
