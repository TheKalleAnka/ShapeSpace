using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShapeSpace
{
    /// <summary>
    /// This is the shell
    /// </summary>
    public class ShapeSpace : Game, Observer
    {
        GraphicsDeviceManager graphics;

        GameStates gameState;
        GameStates previousGameState;

        GameComponent gc;
        UIComponent uc;

        public ShapeSpace()
        {
            this.IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public void OnNotify(object caller, string eventID)
        {
            switch(eventID)
            {
                
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
            base.Initialize();

            gc = new GameComponent(GraphicsDevice);
            uc = new UIComponent(GraphicsDevice, HandleUICallbacks);

            UpdateGameState(GameStates.MAINMENU);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
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
            MouseState mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mouseState.LeftButton == ButtonState.Pressed)
                uc.OnClick(new Vector2(mouseState.X, mouseState.Y));

            //(float)gameTime.ElapsedGameTime.TotalSeconds = DeltaTime

            // TODO: Add your update logic here
            gc.Update(gameTime);
            uc.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            gc.Draw(gameTime);
            uc.Draw(gameTime);

            base.Draw(gameTime);
        }

        void HandleUICallbacks(string id)
        {
            switch(id)
            {
                case "BUTTON_START_GAME":
                    UpdateGameState(GameStates.PLAYING);
                    break;
                case "BUTTON_QUIT_GAME":
                    this.Exit();
                    break;
            }
        }

        void UpdateGameState(GameStates newGameState)
        {
            //Don't bother changing game state if we are trying to change to the current one
            if (newGameState == gameState)
                return;

            previousGameState = gameState;
            gameState = newGameState;

            gc.UpdateGameState(gameState);
            uc.UpdateGameState(gameState);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
        }
    }
}
