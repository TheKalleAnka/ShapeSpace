﻿using System.Collections.Generic;
//Physics should only affect the player if it is on the server
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

class Player : ILoadable, IUpdateable
{
    Vector2 position;
    //There is no rotation here since no players can spin!
    int scale = 100;//Should this be a Vector2 to allow different scaling in different axes?
    //NETWORK
    /// <summary>
    /// Determines whether this player is controlled by this client
    /// </summary>
    bool isLocal = false;
    /// <summary>
    /// Is this player on a server
    /// </summary>
    bool isOnServer = false;
    /// <summary>
    /// An ID that is unique on this game
    /// </summary>
    int networkID = 0;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;

    List<Trail> trail = new List<Trail>();

    //PHYSICS
    Body physicsBody;

    public Player(bool isLocal, bool isOnServer, int networkID, GraphicsDevice graphicsDevice, Vector2 position, World physicsWorld)
    {
        this.isLocal = isLocal;
        this.isOnServer = isOnServer;
        this.networkID = networkID;
        this.graphicsDevice = graphicsDevice;
        this.position = position;

        //if (isOnServer)
            physicsBody = new Body(physicsWorld, position);
            physicsBody.BodyType = BodyType.Dynamic;
    }

    public void LoadContent(ContentManager cManager)
    {
        //Should be replaced by actual textures
        texture = new Texture2D(graphicsDevice,1,1);
        texture.SetData(new[] { Color.White });
    }
    public void UnloadContent() 
    {
        texture = null;
    }
    public void Update(GameTime gameTime) { }
    public void Draw(ref SpriteBatch spriteBatch) 
    {
        if(texture != null)
        spriteBatch.Draw(texture, new Rectangle((int)physicsBody.Position.X, (int)physicsBody.Position.Y, scale, scale), Color.Blue);
    }
}
