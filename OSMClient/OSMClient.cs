using Global.NetworkData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace OSMClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class OSMClient : Game
    {

        static Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        static MemoryStream ms = new MemoryStream(new byte[256], 0, 256, true, true);
        static BinaryWriter writer = new BinaryWriter(ms);
        static BinaryReader reader = new BinaryReader(ms);
        public static BinaryFormatter formatter = new BinaryFormatter();

        static Random random = new Random();

        List<LineData> linesData = new List<LineData>();
        List<PointData> characters = new List<PointData>();

        static void SendPacket(PacketInfo info)
        {
            ms.Position = 0;

            switch (info)
            {
                default:
                    writer.Write((int)info);
                    socket.Send(ms.GetBuffer());
                    break;
            }
        }

        int ReceivePacket()
        {
            ms.Position = 0;
            socket.Receive(ms.GetBuffer());
            int code = reader.ReadInt32();

            switch ((PacketInfo)code)
            {
                case PacketInfo.ID: return reader.ReadInt32();
                case PacketInfo.CameraOffset:
                    offset = ((PointData)formatter.Deserialize(ms)).Vector;
                    break;

                case PacketInfo.Map:
                    linesData.Add((LineData)formatter.Deserialize(ms));
                    break;
                case PacketInfo.Character:
                    var character = (PointData)formatter.Deserialize(ms);
                    var updateCharacter = characters.Find(c => c.Id == character.Id);
                    if (updateCharacter != null)
                        updateCharacter.Vector = character.Vector;
                    else
                        characters.Add(character);
                    break;
            }

            return -1;
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector2 offset;

        Texture2D t; //base for the line texture

        public OSMClient()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
            this.Window.Position = new Point(200, 50);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.IsFullScreen = false;


            socket.Connect("127.0.0.1", 2048);

            SendPacket(PacketInfo.ID);
            int id = ReceivePacket();

            SendPacket(PacketInfo.Map);
            SendPacket(PacketInfo.CameraOffset);

            Task.Run(() => { while (true) ReceivePacket(); });
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

            // TODO: Add your update logic here

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

            try
            {
                foreach (var lineData in linesData)
                {
                    lineData.Line.Draw(spriteBatch, lineData.Thickness, lineData.Color, t);
                }
            }
            catch (Exception)
            {

            }
            try
            {
                foreach (var character in characters)
                {
                    spriteBatch.Draw(t, new Rectangle(character.Vector.ToPoint(), new Point(3, 3)), Color.Red);
                }
            }
            catch (Exception)
            {

            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
