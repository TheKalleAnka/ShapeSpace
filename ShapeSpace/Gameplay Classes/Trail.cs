using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class Trail : IUpdateable
{
    public delegate void TrailCallback(int index);

    public event TrailCallback OnDestroy;

    public int Index;

    Vector2 position;
    float size;
    Color color;

    Texture2D texture;

    public Trail(Vector2 position, int size, Color color, GraphicsDevice gDev)
    {
        this.position = position;
        this.size = size;
        this.color = color;

        if(gDev != null)
        {
            texture = new Texture2D(gDev, 1, 1);
            texture.SetData<Color>(new[] { Color.White });
        }
    }

    public void Draw(ref SpriteBatch spriteBatch)
    {
        if(texture != null)
            spriteBatch.Draw(texture, position: position - new Vector2(size/2f,size/2f), scale: new Vector2(size, size), color: color);
    }

    public void Update(GameTime gameTime)
    {
        if (size <= 0)
            Destroy();

        size -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
    }

    public void Destroy()
    {
        OnDestroy(Index);
    }
}
