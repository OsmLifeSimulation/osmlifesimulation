using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OSM.Simulated_Objects;
using OSM.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OSM
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OSM : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        OSMData data;

        public List<Character> Characters = new List<Character>();

        Vector2 offset;

        Texture2D t; //base for the line texture

        bool showGrid = false;

        KeyboardState keyboardOldState = new KeyboardState();

        List<List<Node>> paths = new List<List<Node>>();

        Graph MovementsGraph;

        public OSM()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
            this.Window.Position = new Point(200, 50);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.IsFullScreen = false;

            data = new OSMData();


            //offset = data.buildingPoints[Constants.rnd.Next(0, data.buildingPoints.Count)][0];
            offset.X = -(data.area.Center.X - graphics.PreferredBackBufferWidth / 2);
            offset.Y = -(data.area.Center.Y - graphics.PreferredBackBufferHeight / 2);

            for (int i = 0; i < Constants.AdditionalThreadsCount; i++)
            {
                new Thread(() =>
                {
                    //Thread.CurrentThread.IsBackground = true;
                    Graph graph = data.CreateGraph();
                    MovementsGraph = graph;
                    SearchEngine search = new SearchEngine(graph.Nodes);
                    while (true)
                    {
                        //TODO: run this code in new Thread, but we need to create new instance of SearchEngine with new isolated List<Node> (we need to copy it and all nodes in all edges)
                        //or maybe we can just wait until the same code is executed? ...and then create a new Thread

                        var sw = Stopwatch.StartNew();
                        var sourceNode = data.Entrances[Constants.rnd.Next(data.Entrances.Count - 1)];
                        var targetNode = data.Entrances.Where(n => n != sourceNode).ToList()[Constants.rnd.Next(data.Entrances.Count - 1)];
                        search.ChangeStartEnd(graph.GetClosestNode(sourceNode), graph.GetClosestNode(targetNode));
                        var path = search.GetShortestPathAstart();
                        sw.Stop();
                        if (path.Count != 1)
                        {
                            path.Insert(0, new Node(sourceNode));
                            path.Add(new Node(targetNode));
                            paths.Add(path);
                            Characters.Add(new Character(path));
                        }
                        else
                        {
                        }
                }
                }).Start();
            }

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create 1x1 texture for line drawing
            t = new Texture2D(GraphicsDevice, 1, 1);
            t.SetData<Color>(
                new Color[] { Color.White });// fill the texture with white
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            //kill all Threads
            Environment.Exit(Environment.ExitCode);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var keyboardState = Keyboard.GetState();

            float speed = 10;
            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                offset.Y += speed;
            }
            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                offset.Y -= speed;
            }
            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                offset.X -= speed;
            }
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                offset.X += speed;
            }

            if (keyboardState.IsKeyDown(Keys.G) && keyboardOldState.IsKeyUp(Keys.G))
            {
                showGrid = !showGrid;
            }

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


            keyboardOldState = keyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            Matrix Transform = Matrix.CreateTranslation(offset.X, offset.Y, 0);
            spriteBatch.Begin(transformMatrix: Transform);

            //spriteBatch.Draw(t, data.area, Color.Black);

            foreach (var build in data.BuildingLines)
            {
                DrawLine(spriteBatch, build, Color.SlateGray, 2);
            }

            foreach (var road in data.RoadLines)
            {
                DrawLine(spriteBatch, road, Color.GhostWhite, 4);
            }

            //draw grid points
            //foreach (var row in data.points)
            //{
            //    foreach (var point in row)
            //    {
            //        spriteBatch.Draw(t, new Rectangle(point.X - 1, point.Y - 1, 3, 3), Color.DarkCyan);
            //    }
            //}

            //display grid
            if (showGrid && MovementsGraph != null)
            {
                var gridColor = Color.DarkGreen;
                var gridThickness = 1;
                foreach (var row in MovementsGraph.NodesMatrix)
                {
                    Vector2 start = new Vector2(data.area.X, row[0].Point.Y);
                    Vector2 end = new Vector2(data.area.Right, row[0].Point.Y);
                    DrawLine(spriteBatch, start, end, gridColor, gridThickness);
                }
                foreach (var point in MovementsGraph.NodesMatrix[0])
                {
                    Vector2 start = new Vector2(point.Point.X, data.area.Y);
                    Vector2 end = new Vector2(point.Point.X, data.area.Bottom);
                    DrawLine(spriteBatch, start, end, gridColor, gridThickness);
                }

                try
                {
                    foreach (var path in paths.ToList())
                    {
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            var current = path[i].Point;
                            var next = path[i + 1].Point;
                            DrawLine(spriteBatch, new Line(current, next), Color.Blue, 2);
                        }
                    }
                }
                catch (Exception)
                {

                }

            }

            foreach (var node in data.Entrances)
            {
                spriteBatch.Draw(t, new Rectangle(Convert.ToInt32(node.X), Convert.ToInt32(node.Y), 3, 3), Color.Blue);
            }

            try
            {
                foreach (var character in Characters.ToList())
                {
                    spriteBatch.Draw(t, new Rectangle(character.Point, new Point(3, 3)), Color.Red);
                }
            }
            catch (Exception)
            {

            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawLine(SpriteBatch sb, Line line, Color color, int thickness)
        {
            DrawLine(sb, line.Start, line.End, color, thickness);
        }
        void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            sb.Draw(t,
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
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
