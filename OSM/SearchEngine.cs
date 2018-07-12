using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM
{
    public class SearchEngine
    {      
        public Node Start { get; set; }
        public Node End { get; set; }
        public int NodeVisits { get; private set; }
        public double ShortestPathLength { get; set; }
        public double ShortestPathCost { get; private set; }

        public List<Node> Nodes;

        public SearchEngine(List<Node> nodes)
        {
            Nodes = nodes;

            ////Try copy list (incorrect)
            //Nodes = nodes.Select(n => (Node)n.Clone()).ToList();
            //foreach (var node in Nodes)
            //{
            //    for (int i = 0; i < node.Connections.Count; i++)
            //    {
            //        var conn = node.Connections[i];
            //        conn = new Edge(Nodes.Find(n => n.Point == conn.ConnectedNodes[0].Point), 
            //            Nodes.Find(n => n.Point == conn.ConnectedNodes[1].Point),
            //            conn.Length, conn.Cost);
            //    }
            //}
        }

        public void ChangeStartEnd(Node start, Node end)
        {
            Start = start;
            End = end;

            //Start = Nodes.Find(n => n.Point == start.Point);
            //End = Nodes.Find(n => n.Point == end.Point);
        }

        public List<Node> GetShortestPathDijikstra()
        {
            foreach (var node in Nodes)
            {
                node.Clear();
            }

            DijkstraSearch();
            var shortestPath = new List<Node>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void BuildShortestPath(List<Node> list, Node node)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            ShortestPathLength += node.Connections.Single(x => x.AnotherNode(node) == node.NearestToStart).Length;
            ShortestPathCost += node.Connections.Single(x => x.AnotherNode(node) == node.NearestToStart).Cost;
            BuildShortestPath(list, node.NearestToStart);
        }

        private void DijkstraSearch()
        {
            NodeVisits = 0;
            Start.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(Start);
            do
            {
                NodeVisits++;
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart.Value).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
                {
                    var childNode = node.ConnectedNode(cnn);
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == End)
                    return;
            } while (prioQueue.Any());
        }

        public List<Node> GetShortestPathAstart()
        {
            foreach (var node in Nodes)
            {
                node.Clear();
                node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(End);
            }
                
            AstarSearch();
            var shortestPath = new List<Node>();
            shortestPath.Add(End);
            BuildShortestPath(shortestPath, End);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void AstarSearch()
        {
            NodeVisits = 0;
            Start.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(Start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                NodeVisits++;
                foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
                {
                    var childNode = node.ConnectedNode(cnn);
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == End)
                    return;
            } while (prioQueue.Any());
        }
    }
}
