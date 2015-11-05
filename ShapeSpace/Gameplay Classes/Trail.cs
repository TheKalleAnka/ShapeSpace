using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
class Trail : IUpdateable
{
    public delegate void TrailCallback(string id);

    public event TrailCallback OnDestroy;

    Vector2 position;
    int size;
    Color color;

    Texture2D texture;

    public Trail(Vector2 position, int size, Color color, GraphicsDevice gDev)
    {
        this.position = position;
        this.size = size;
        this.color = color;

        texture = new Texture2D(gDev, 1, 1);
        texture.SetData<Color>(new[]{Color.White});
    }

    public void Draw(ref SpriteBatch spriteBatch, GameTime gameTime)
    {
        Rectangle rect = new Rectangle((int)position.X,(int)position.Y,size,size);

        spriteBatch.Draw(texture, rect, color);
    }

    public void Update(GameTime gameTime)
    {
        if (size <= 1)
            Destroy();

        size -= 1;
    }

    public void Destroy()
    {
        OnDestroy("DESTROYED");
    }
}
