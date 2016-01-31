using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;

public class Player : ILoadable, IUpdateable
{
    //GAMEPLAY
    protected int power = 100;
    ShapeTeam team = ShapeTeam.UNKNOWN;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;

    List<Trail> trail = new List<Trail>();
    public List<PositionInTime> positions = new List<PositionInTime>();

    Vector2 positionNow = new Vector2(100,100);
    Vector2 targetPosition = Vector2.Zero;
    float lastChangedTargetPosition = 0;
    float changeTargetPositionTime = 0;

    //PHYSICS
    public Body physicsBody { get; private set; }

    public Player(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
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
        //UIComponent.Instance._DebugString = physicsBody.LinearVelocity.ToString();

        lastChangedTargetPosition += (float)gameTime.ElapsedGameTime.Seconds;

        if(positions.Count > 0)
        {
            //Move the player to a new position along a lerp between the current and the target position
            positionNow = Vector2.Lerp(positionNow, targetPosition, MathHelper.Clamp(lastChangedTargetPosition / changeTargetPositionTime, 0, 1));

            if(lastChangedTargetPosition >= changeTargetPositionTime)
            {
                targetPosition = positions[0].Position;
                changeTargetPositionTime = positions[0].Time;

                lastChangedTargetPosition = 0;
                positions.RemoveAt(0);
            }
        }
    }

    public void Draw(ref SpriteBatch spriteBatch) 
    {
        if(texture != null)
            spriteBatch.Draw(texture, new Rectangle((int)positionNow.X, (int)positionNow.Y, power, power), Color.ForestGreen);
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
