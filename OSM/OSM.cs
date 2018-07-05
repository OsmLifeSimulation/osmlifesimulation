using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OSM.Structures;
using System;
using System.Linq;

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

        Vector2 offset;

        Texture2D t; //base for the line texture

        bool showGrid = false;

        KeyboardState keyboardOldState = new KeyboardState();

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

            foreach (var node in data.nodes)
            {
                spriteBatch.Draw(t, new Rectangle(Convert.ToInt32(node.X), Convert.ToInt32(node.Y), 3, 3), Color.DarkCyan);
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
            if (showGrid)
            {
                var gridColor = Color.DarkGreen;
                var gridThickness = 1;
                foreach (var row in  data.MovementsGraph.Vertices)
                {
                    Vector2 start = new Vector2(data.area.X, row[0].Y);
                    Vector2 end = new Vector2(data.area.Right, row[0].Y);
                    DrawLine(spriteBatch, start, end, gridColor, gridThickness);
                }
                foreach (var point in data.MovementsGraph.Vertices[0])
                {
                    Vector2 start = new Vector2(point.X, data.area.Y);
                    Vector2 end = new Vector2(point.X, data.area.Bottom);
                    DrawLine(spriteBatch, start, end, gridColor, gridThickness);
                }

                foreach (var edge in data.MovementsGraph.BlockedEdges)
                {
                    DrawLine(spriteBatch, edge.Line, edge.IntersectsWith == LineType.building ? Color.Red : Color.Yellow, 1);
                }
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
