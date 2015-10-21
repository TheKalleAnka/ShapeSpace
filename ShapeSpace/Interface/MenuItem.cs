using Microsoft.Xna.Framework;

/// <summary>
/// Abstract class which all items in a menu derive from
/// </summary>
abstract class MenuItem : IDrawable
{
    protected Rectangle rectangle { get; set; }
    protected Color baseColor { get; set; }
    protected Color foreColor { get; set; }
    protected Color textColor { get; set; }

    protected bool IsPressOnThisItem(Vector2 pos)
    {
        if(pos.X > rectangle.X &&
           pos.X < rectangle.X + rectangle.Width && 
           pos.Y > rectangle.Y &&
           pos.Y < rectangle.Y + rectangle.Height)
        {
            return true;
        }

        return false;
    }

    public virtual void Draw(GameTime gameTime) { }
}
