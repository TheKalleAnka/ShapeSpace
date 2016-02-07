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
    float lastChangedPosition = 0;


    public Vector2 positionNow = new Vector2();


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
        //Remove the first occurence if it has overlived it's time
        if (positions.Count >= 2)
            if (lastChangedPosition >= positions[1].TimeSincePrevious)
            {
                positions.RemoveAt(0);

                lastChangedPosition = 0;

                UIComponent.Instance._DebugString += positions.Count + "  " + positions[0].Position;
            }
    }

    public void Draw(ref SpriteBatch spriteBatch) 
    {
        if(texture != null && positions.Count > 0)
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
