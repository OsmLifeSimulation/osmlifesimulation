using Microsoft.Xna.Framework;
using OSM.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM
{
    class Graph
    {
        //Вершины
        public List<List<Node>> NodesMatrix = new List<List<Node>>();
        public List<Node> Nodes = new List<Node>();

        public SearchEngine Search { get; private set; }

        //for tests
        public List<Node> path = new List<Node>();

        public Graph(Rectangle area, List<Line> buildingLines, List<Line> roadLines)
        {
            //get graph vertices 
            for (int i = area.Y; i <= area.Bottom; i += Constants.GridFrequency)
            {
                var pointsRow = new List<Node>();
                for (int j = area.X; j <= area.Right; j += Constants.GridFrequency)
                {
                    pointsRow.Add(new Node(new Point(j, i)));
                }
                NodesMatrix.Add(pointsRow);
            }

            //find blocked edges
            for (int i = 0; i < NodesMatrix.Count; i++)
            {
                for (int j = 0; j < NodesMatrix[i].Count; j++)
                {
                    var vertex = NodesMatrix[i][j];
                    var nearby = MathExtensions.AdjacentBottomRightElements(NodesMatrix, i, j).ToList();

                    foreach (var neighbor in nearby)
                    {
                        var edgeLine = new Line(vertex.Point, neighbor.Point);
                        //var edge = new Edge(vertex, neighbor, MathExtensions.LineLength(edgeLine), 2);


                        bool buildingFinded = false;
                        foreach (var line in buildingLines)
                        {
                            if (MathExtensions.LinesIntersects(edgeLine, line))
                            {
                                buildingFinded = true;
                                break;
                            }
                        }
                        if (!buildingFinded)
                        {
                            double cost = 1;

                            foreach (var line in roadLines)
                            {
                                if (MathExtensions.LinesIntersects(edgeLine, line))
                                {
                                    cost = 4;
                                    break;
                                }
                            }

                            var edge = new Edge(vertex, neighbor, MathExtensions.LineLength(edgeLine), cost);
                            vertex.Connections.Add(edge);
                            neighbor.Connections.Add(edge);
                        }
                    }
                }
            }

            Nodes = NodesMatrix.SelectMany(x => x).ToList();

            Search = new SearchEngine(Nodes);

            //try find way
            Search.Start = NodesMatrix.First().First();
            Search.End = NodesMatrix.Last().Last();

            var sw = Stopwatch.StartNew();
            path = Search.GetShortestPathAstart();
            sw.Stop();
            var mill = sw.ElapsedMilliseconds;
        }

        public Node GetClosestNode(Point sourcePoint)
        {
            return Nodes.OrderBy(node => MathExtensions.LineLength(new Line(node.Point, sourcePoint))).Where(n => n.Connections.Count != 0).First();
        }
    }

    public class Node : ICloneable
    {
        public Point Point { get; set; }
        public List<Edge> Connections { get; set; } = new List<Edge>();

        public double? MinCostToStart { get; set; }
        public Node NearestToStart { get; set; }
        public bool Visited { get; set; }
        public double StraightLineDistanceToEnd { get; set; }

        public Node(Point point)
        {
            Point = point;
        }

        public Node ConnectedNode(Edge edge)
        {
            return Connections.Find(e => e == edge).AnotherNode(this);
        }
        public List<Node> ConnectedNodes
        {
            get
            {
                return Connections.Select(c => c.AnotherNode(this)).ToList();
            }
        }

        public double StraightLineDistanceTo(Node end)
        {
            return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
        }

        internal bool ToCloseToAny(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var d = Math.Sqrt(Math.Pow(Point.X - node.Point.X, 2) + Math.Pow(Point.Y - node.Point.Y, 2));
                if (d < 0.01)
                    return true;
            }
            return false;
        }

        public object Clone()
        {
            var node = new Node(Point);
            node.Connections = Connections;
            return node;
        }

        public void Clear()
        {
            MinCostToStart = null;
            NearestToStart = null;
            Visited = false;
            StraightLineDistanceToEnd = 0;
        }
    }

    public class Edge
    {
        public double Length { get; set; }
        public double Cost { get; set; }
        public Node[] ConnectedNodes { get; set; } = new Node[2];

        public Edge(Node first, Node second, double length, double cost)
        {
            ConnectedNodes[0] = first;
            ConnectedNodes[1] = second;
            Length = length;
            Cost = cost;
        }

        public Node AnotherNode(Node node)
        {
            return ConnectedNodes.First(n => n != node);
        }
    }
}
