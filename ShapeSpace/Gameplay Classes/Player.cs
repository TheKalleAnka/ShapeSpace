﻿using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Network;

public class Player : ILoadable, IUpdateable
{
    //GAMEPLAY
    float power = 0.9f;
    public ShapeTeam team = ShapeTeam.UNKNOWN;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;
    Color color;

    //TRAIL
    List<Trail> trail = new List<Trail>();
    Vector2 positionLastAddedTrail = Vector2.Zero;

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
        texture = cManager.Load<Texture2D>("SQUARE");
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
                    if (lastChangedPosition >= positions[0].TimeSincePrevious + behindInTime)
                    {
                        positions.RemoveAt(0);

                        behindInTime += positions[0].TimeSincePrevious;
                    }
                }

                //lastChangedPosition = 0;

                //positionNow = Vector2.Lerp(positionNow, positions[0].Position, MathHelper.Clamp(positions[0].TimeSincePrevious / 0.1f, 0, 1));
            }

            previousPosition = positionNow;
            //Interpolate between the current position and the position given by the server
            //TimeSincePrevious thus adds to the input lag on top of the latency
            if(positions.Count > 0)
                positionNow = Vector2.Lerp(positionNow, positions[0].Position, MathHelper.Clamp((float)gameTime.ElapsedGameTime.TotalSeconds / positions[0].TimeSincePrevious,0,1));

            if (Vector2.Distance(positionLastAddedTrail, positionNow) > 30f)
                CreateNewRowOfTrail();
        }
        catch { }

        for (int i = 0; i < trail.Count; i++)
        {
            trail[i].Update(gameTime);
        }

        if(positions.Count > 0)
            UIComponent.Instance._DebugString = positions.Count + "  " + positions[0].Position + " " + positionNow;

    }

    public void Draw(ref SpriteBatch spriteBatch) 
    {
        for (int i = 0; i < trail.Count; i++)
        {
            trail[i].Draw(ref spriteBatch);
        }

        if (texture != null /*&& positions.Count > 0*/)
            //spriteBatch.Draw(texture, new Rectangle((int)(positionNow.X - power / 2f), (int)(positionNow.Y - power/2f), power, power), Color.ForestGreen); 
            spriteBatch.Draw(texture, position: positionNow, scale: new Vector2(power, power), color: Color.ForestGreen);
    }

    public void SetTeam(ShapeTeam team)
    {
        this.team = team;
    }
    /*
    public void SetClass(ShapeClass type)
    {

    }*/

    public void CreateNewRowOfTrail()
    {
        Trail newTrail = new Trail(positionNow, 20, Color.Blue, graphicsDevice);
        newTrail.Index = trail.Count;
        newTrail.OnDestroy += WhenTrailIsDestroyed;
        trail.Add(newTrail);

        positionLastAddedTrail = positionNow;
    }

    void WhenTrailIsDestroyed(int index)
    {
        trail.RemoveAt(index);

        for(int i = 0; i < trail.Count; i++)
        {
            trail[i].Index = i;
        }
    }
}
