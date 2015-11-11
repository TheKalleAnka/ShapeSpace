using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
class InputField : MenuItem, IMenuClickable
{
    public InputField(ref SpriteBatch spriteBatch, Rectangle rect, Color color, Color textColor, SpriteFont font, string text) : base(ref spriteBatch)
    {
        this.rectangle = rect;
        this.baseColor = color;
        this.textColor = textColor;
        this.font = font;
        this.text = text;
    }

    public override void Draw(GameTime gameTime)
    {
        spriteBatch.Draw(texture, rectangle, baseColor);
        spriteBatch.DrawString(font, text, new Vector2(rectangle.X,rectangle.Y), textColor);
    }

    public void OnClick(Vector2 pos)
    {
        if(IsPressOnThisItem(pos))
        {
            
        }
    }
}
