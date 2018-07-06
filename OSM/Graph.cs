using Microsoft.Xna.Framework;
using OSM.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM
{
    class Graph
    {
        //Вершины
        public List<List<Point>> Vertices = new List<List<Point>>();
        //Пересекаемые рёбра
        public List<GraphEdge> BlockedEdges = new List<GraphEdge>();

        public Graph(Rectangle area, List<Line> buildingLines, List<Line> roadLines)
        {
            //get graph vertices 
            for (int i = area.Y; i <= area.Bottom; i += Constants.GridFrequency)
            {
                var pointsRow = new List<Point>();
                for (int j = area.X; j <= area.Right; j += Constants.GridFrequency)
                {
                    pointsRow.Add(new Point(j, i));
                }
                Vertices.Add(pointsRow);
            }

            //find blocked edges
            for (int i = 0; i < Vertices.Count; i++)
            {
                for (int j = 0; j < Vertices[i].Count; j++)
                {
                    var vertex = Vertices[i][j];
                    var nearby = MathExtensions.AdjacentBottomRightElements(Vertices, i, j).ToList();

                    foreach (var neighbor in nearby)
                    {
                        var edgeLine = new Line(vertex, neighbor);
                        bool buildingFinded = false;
                        foreach (var line in buildingLines)
                        {
                            if (MathExtensions.LinesIntersects(edgeLine, line))
                            {
                                BlockedEdges.Add(new GraphEdge(edgeLine, LineType.building));
                                buildingFinded = true;
                                break;
                            }
                        }
                        if (!buildingFinded)
                        {
                            foreach (var line in roadLines)
                            {
                                if (MathExtensions.LinesIntersects(edgeLine, line))
                                {
                                    BlockedEdges.Add(new GraphEdge(edgeLine, LineType.road));
                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }

    }

    struct GraphEdge
    {
        public Line Line;
        public LineType IntersectsWith;
        public GraphEdge(Line line, LineType intersectsWith)
        {
            Line = line;
            IntersectsWith = intersectsWith;
        }

        public static bool operator !=(GraphEdge left, GraphEdge right)
        {
            return !(left == right);
        }

        public static bool operator ==(GraphEdge left, GraphEdge right)
        {
            return (left.Line.Start == right.Line.Start && left.Line.End == right.Line.End) ||
                (left.Line.Start == right.Line.End && left.Line.End == right.Line.Start);
        }
    }
}
