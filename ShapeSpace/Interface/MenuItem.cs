using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Abstract class which all items in a menu derive from
/// </summary>
abstract class MenuItem : IDrawable
{
    public delegate void MenuItemClicked();

    protected SpriteBatch spriteBatch;
    protected Texture2D texture;

    protected Rectangle rectangle { get; set; }
    protected Color baseColor { get; set; }
    protected Color foreColor { get; set; }
    protected Color textColor { get; set; }
    protected string text { get; set; }

    public MenuItem(ref SpriteBatch spriteBatch)
    {
        this.spriteBatch = spriteBatch;
        this.texture = new Texture2D(spriteBatch.GraphicsDevice,1,1);
        this.texture.SetData<Color>(new[]{Color.White});
    }

    protected bool IsPressOnThisItem(Vector2 pos)
    {
        if(pos.X > rectangle.X &&
           pos.X < rectangle.X + rectangle.Width && 
           pos.Y > rectangle.Y &&
           pos.Y < rectangle.Y + rectangle.Height)
               return true;

        return false;
    }

    public virtual void Draw(GameTime gameTime) { }
}
