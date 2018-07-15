using Global.NetworkData;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM.Simulated_Objects
{
    public class Character
    {
        List<Node> path;
        public Point Point;
        int speed = 1;

        Node next;

        PointData pointData;
        public PointData PointData { get {
                pointData.Vector = Point.ToVector2();
                return pointData;
            } }

        public Character(List<Node> path)
        {
            pointData = new PointData(Color.Red, 3);
            this.path = path;
            Point = path[0].Point;
            next = path.Count != 1 ? path[1] : path[0];
        }
        public void Update(out bool remove)
        {
            if (next != null)
            {
                if (move())
                {
                    next = getNextNode();
                }
                remove = false;
            }
            else
            {
                remove = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>next node reached</returns>
        private bool move()
        {
            if (Point == next.Point)
                return true;

            if (Point.X != next.Point.X)
            {
                Point.X += Point.X < next.Point.X ? speed : -speed;
            }
            if (Point.Y != next.Point.Y)
            {
                Point.Y += Point.Y < next.Point.Y ? speed : -speed;
            }
            return false;
        }

        private Node getNextNode()
        {
            int indexNext = path.IndexOf(next) + 1;
            return indexNext != path.Count ? path[indexNext] : null;
        }
    }
}
