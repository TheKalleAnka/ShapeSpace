using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Gameplay;
using ShapeSpace.Network;

public class Player : ILoadable, IUpdateable
{
    //GAMEPLAY
    public float power = 25f;
    public ShapeTeam team = ShapeTeam.UNKNOWN;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;
    Color color;

    //TRAIL
    public List<Trail> trail = new List<Trail>();
    protected Vector2 positionLastAddedTrail = Vector2.Zero;

    //NETWORK
    public int indexOnServer = -1;

    public List<PositionInTime> positions = new List<PositionInTime>();
    Vector2 previousPosition = Vector2.Zero;
    float lastChangedPosition = 0;

    public Vector2 positionNow = Vector2.Zero;

    public Player(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
    }

    public void LoadContent(ContentManager cManager)
    {
        //Should be replaced by actual textures?
        texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData<Color>(new[] { Color.White });

        //texture = cManager.Load<Texture2D>("SQUARE");
    }

    public void UnloadContent() 
    {
        texture = null;
    }

    public void Update(GameTime gameTime) 
    {
        lastChangedPosition += (float)gameTime.ElapsedGameTime.TotalSeconds;

        //Sometimes a NullReferenceException occurs for unknown reason but I have deemed it non-fatal
        //since it ususally fixes itself the next frame. Thus a try-catch to prevent error message.
        try
        {
            //Remove the first occurence if it has overlived it's time
            if (positions.Count >= 2)
            {
                float behindInTime = 0;

                for (int i = 0; i < positions.Count - 1; i++ )
                {
                    if (lastChangedPosition >= positions[0].TimeSincePrevious + behindInTime || positions[0].Temporary)
                    {
                        positions.RemoveAt(0);

                        if (!positions[0].Temporary)
                            behindInTime += positions[0].TimeSincePrevious;
                        else
                            behindInTime += lastChangedPosition;
                    }
                }
            }
            else if(positions.Count >= 1)
            {
                if(lastChangedPosition >= positions[0].TimeSincePrevious)
                {
                    positions.Add(new PositionInTime(0, positionNow + (positionNow - previousPosition), true));

                    positions.RemoveAt(0);
                }
            }

            previousPosition = positionNow;

            //Interpolate between the current position and the position given by the server
            //TimeSincePrevious thus adds to the input lag on top of the latency
            if(positions.Count > 0)
                positionNow = Vector2.Lerp(positionNow, positions[0].Position, MathHelper.Clamp((float)gameTime.ElapsedGameTime.TotalSeconds / positions[0].TimeSincePrevious - 0.1f,0,1));
            /*
            if (Vector2.Distance(positionLastAddedTrail, positionNow) > 3f)
                CreateNewRowOfTrail();*/
        }
        catch { }
        /*
        for (int i = 0; i < trail.Count; i++)
        {
            trail[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }*/
        /*
        if(positions.Count > 0)
            UIComponent.Instance._DebugString = positions.Count + "  " + positions[0].Position + " " + positionNow;*/

    }

    public void Draw(ref SpriteBatch spriteBatch) 
    {
        for (int i = 0; i < trail.Count; i++)
        {
            trail[i].Draw(ref spriteBatch);
        }

        if (texture != null)
            spriteBatch.Draw(texture, position: positionNow - new Vector2(power / 2f, power / 2f), scale: new Vector2(power, power), color: color);
    }

    /// <summary>
    /// Assigns the player to a team and handles setting the appropriate color and so on.
    /// </summary>
    /// <param name="team"></param>
    public void SetTeam(ShapeTeam team)
    {
        this.team = team;

        switch(team)
        {
            case ShapeTeam.GREEN:
                color = Color.ForestGreen;
                break;
            case ShapeTeam.RED:
                color = Color.DarkRed;
                break;
        }
    }

    public void CreateNewRowOfTrail()
    {
        Trail newTrail = new Trail(positionNow, 2, Color.Blue, graphicsDevice, null);
        newTrail.Id = trail.Count;
        newTrail.OnDestroy += DestroyTrail;
        trail.Add(newTrail);

        positionLastAddedTrail = positionNow;
    }

    public void DestroyTrail(int id)
    {
        for(int i = 0; i < trail.Count; i++)
        {
            if (trail[i].Id == id)
            {
                trail.RemoveAt(i);
                break;
            }
        }
    }
}
