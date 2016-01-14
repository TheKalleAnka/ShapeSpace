using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;

public class Player : ILoadable, IUpdateable
{
    //GAMEPLAY
    int power = 100;
    ShapeTeam team = ShapeTeam.UNKNOWN;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;

    List<Trail> trail = new List<Trail>();
    List<Vector2> locations;

    //PHYSICS
    public Body physicsBody { get; private set; }

    public Player(GraphicsDevice graphicsDevice, Vector2 position, World physicsWorld)
    {
        this.graphicsDevice = graphicsDevice;

        physicsBody = new Body(physicsWorld, position);
        physicsBody.BodyType = BodyType.Dynamic;//Should be static if we are not on the server
        physicsBody.FixedRotation = true;
        physicsBody.Position = position;
    }

    public void LoadContent(ContentManager cManager)
    {
        //Should be replaced by actual textures?
        texture = new Texture2D(graphicsDevice,1,1);
        texture.SetData(new[] {Color.White});
    }

    public void UnloadContent() 
    {
        texture = null;
    }

    public void Update(GameTime gameTime) 
    {
        //For debugging
        UIComponent.Instance._DebugString = physicsBody.LinearVelocity.ToString();
    }

    public void Draw(ref SpriteBatch spriteBatch) 
    {
        if(texture != null)
            spriteBatch.Draw(texture, new Rectangle((int)physicsBody.Position.X, (int)physicsBody.Position.Y, power, power), Color.Blue);
    }

    //HELPER METHODS
    public Vector2 GetPosition()
    {
        return physicsBody.Position;
    }

    public void SetTeam(ShapeTeam team)
    {
        this.team = team;
    }
    /*
    public void SetClass(ShapeClass type)
    {

    }*/
}
