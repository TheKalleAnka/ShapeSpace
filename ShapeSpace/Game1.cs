using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using System.Diagnostics;
using System;
using System.Threading;

namespace ShapeSpace
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        int version = 1;

        NetClient client;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {            
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            string arguments = "";
            //Add the version
            arguments += version;

            Process serverProc = Process.Start("ShapeSpaceServer.exe", arguments);

            NetPeerConfiguration config = new NetPeerConfiguration("ShapeSpace");
            config.MaximumConnections = 0;
            client = new NetClient(config);
            client.Start();

            NetOutgoingMessage connectMessage = client.CreateMessage();
            connectMessage.Write("Can I please connect?");

            while (client.ConnectionsCount == 0)
            {
                client.Connect("127.0.0.1", 5566, connectMessage);
            }

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

            // TODO: use this.Content to load your game content here
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 input = Vector2.Zero;
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.A))
                input += new Vector2(-1,0);
            if (keyState.IsKeyDown(Keys.D))
                input += new Vector2(1, 0);
            if (keyState.IsKeyDown(Keys.S))
                input += new Vector2(0, -1);
            if (keyState.IsKeyDown(Keys.W))
                input += new Vector2(0, 1);



            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Maroon);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            //Add anything that draws in here?

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
