using System;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

class GameComponent : BaseComponent, IDrawable, IUpdateable, ILoadable, IInitializable
{
    //GAMEPLAY
    Player player;

    //PHYSICS
    World physWorld;

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Initialize()
    {
        physWorld = new World(new Vector2(10,0));
        player = new Player(true, false, 1, spriteBatch.GraphicsDevice, new Vector2(0,0), physWorld);
    }

    public void LoadContent(ContentManager cManager)
    {
        player.LoadContent(cManager);
    }

    public void UnloadContent()
    {
        player.UnloadContent();
    }

    public void Update(GameTime gameTime)
    {
        physWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

        if(player != null)
            player.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        if(player != null)
            player.Draw(ref spriteBatch);

        spriteBatch.End();
    }
}
