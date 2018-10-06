using OSM.Simulated_Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OSM
{
    class OSM
    {
        OSMData data;

        public List<Character> Characters = new List<Character>();

        List<List<Node>> paths = new List<List<Node>>();

        Graph MovementsGraph;

        public OSM()
        {

            Settings.Init();

            data = new OSMData();

            if (Settings.Presets.RunWebSocketServer)
                ServerWebSocket.Init(Characters);

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
                        var maxCount = Settings.Presets.CharactersMaxCount;
                        if (maxCount <= 0 || Characters.Count < maxCount)
                        {
                            //TODO: run this code in new Thread, but we need to create new instance of SearchEngine with new isolated List<Node> (we need to copy it and all nodes in all edges)
                            //or maybe we can just wait until the same code is executed? ...and then create a new Thread

                            var sw = Stopwatch.StartNew();
                            var sourceNode = data.Entrances[Constants.rnd.Next(data.Entrances.Count - 1)];
                            var targetNode = data.Entrances.Where(n => n != sourceNode).ToList()[Constants.rnd.Next(data.Entrances.Count - 1)];
                            search.ChangeStartEnd(graph.GetClosestNodeOutsideBuilding(sourceNode), graph.GetClosestNodeOutsideBuilding(targetNode));
                            var path = search.GetShortestPathAstart();
                            sw.Stop();
                            if (path.Count != 1)
                            {
                                path.Insert(0, new Node(sourceNode));
                                path.Add(new Node(targetNode));
                                paths.Add(path);
                                Characters.Add(new Character(path, Constants.rnd.Next(3, 30)));
                            }
                            else if (search.NodeVisits != 1)
                            {
                            }
                        }
                        else
                        {
                            Thread.Sleep(10000);
                        }
                    }
                }).Start();
            }
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update()
        {
            try
            {
                foreach (var character in Characters.ToList())
                {
                    bool remove;
                    character.Update(out remove);
                    if (remove)
                    {
                        Characters.Remove(character);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

    }

}
