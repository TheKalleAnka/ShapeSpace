//Physics should only affect the player if it is on the server
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Player /*: BaseComponent?*/
{
    Vector2 position;
    //There is no rotation here since no players can spin!
    int scale = 100;//Should this be a Vector2 to allow different scaling in different axes?
    //NETWORK
    /// <summary>
    /// Determines whether this player is controlled by this client
    /// </summary>
    bool isLocal = false;
    bool isOnServer = false;
    int networkID = 0;

    //DRAWING
    GraphicsDevice graphicsDevice;
    Texture2D texture;

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

    public void LoadContent()
    {
        //Should be replaced by actual textures
        texture = new Texture2D(graphicsDevice,1,1);
        texture.SetData(new[] { Color.White });
    }
    public void UnloadContent() { }
    public void Update() { }
    public void Draw(ref SpriteBatch spriteBatch) 
    {
        spriteBatch.Draw(texture, new Rectangle((int)physicsBody.Position.X, (int)physicsBody.Position.Y, scale, scale), Color.Blue);
    }
}
