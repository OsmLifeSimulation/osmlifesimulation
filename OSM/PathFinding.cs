using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OSM
{
    class PathRequest
    {
        public bool InProcess = false;
        public SemaphoreSlim Semaphore { get; private set; }

        public Point SourceNode { get; private set; }
        public Point TargetNode { get; private set; }

        public List<Point> Path { get; set; }

        public PathRequest(SemaphoreSlim semaphore, Point sourceNode, Point targetNode)
        {
            Semaphore = semaphore;
            SourceNode = sourceNode;
            TargetNode = targetNode;
        }
    }

    static class PathFinding
    {
        static Graph MovementsGraph;
        static List<List<Node>> paths = new List<List<Node>>();

        static List<PathRequest> pathRequests = new List<PathRequest>();

        public static void Init(OSMData data)
        {
            for (int i = 0; i < Settings.Presets.AdditionalThreadsCount; i++)
            {
                new Thread(() =>
                {
                    //Thread.CurrentThread.IsBackground = true;
                    Graph graph = data.CreateGraph();
                    MovementsGraph = graph;
                    SearchEngine search = new SearchEngine(graph.Nodes);
                    while (true)
                    {
                        var request = pathRequests.ToList().FirstOrDefault(x => !x.InProcess);
                        if (request != null)
                        {
                            //TODO: run this code in new Thread, but we need to create new instance of SearchEngine with new isolated List<Node> (we need to copy it and all nodes in all edges)
                            //or maybe we can just wait until the same code is executed? ...and then create a new Thread

                            request.InProcess = true;

                            var sw = Stopwatch.StartNew();
                            search.ChangeStartEnd(graph.GetClosestNodeOutsideBuilding(request.SourceNode), graph.GetClosestNodeOutsideBuilding(request.TargetNode));
                            var path = search.GetShortestPathAstart();
                            sw.Stop();
                            if (path.Count != 1)
                            {
                                path.Insert(0, new Node(request.SourceNode));
                                path.Add(new Node(request.TargetNode));
                                request.Path = path.Select(x => x.Point).ToList();
                                request.Semaphore.Release();
                            }
                            else if (search.NodeVisits != 1)
                            {
                            }
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                }).Start();
            }
        }

        //public static async Task<List<Point>> GetPath(System.Drawing.Point sourceNode, System.Drawing.Point targetNode)
        //{
        //    return await GetPath(sourceNode.ToXnaPoint(), targetNode.ToXnaPoint());
        //}

        public static async Task<List<Point>> GetPath(Point sourceNode, Point targetNode)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
            PathRequest pathRequest = new PathRequest(semaphore, sourceNode, targetNode);
            pathRequests.Add(pathRequest);

            await semaphore.WaitAsync();

            pathRequests.Remove(pathRequest);
            return pathRequest.Path;
        }

    }
}
