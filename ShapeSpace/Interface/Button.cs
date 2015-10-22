using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

class Button : MenuItem, IMenuClickable
{
    /// <summary>
    /// The event that is sent to all observers when this button is pressed
    /// </summary>
    string buttonID { get; set; }

    public event MenuItemClicked OnClicked;

    public Button(ref SpriteBatch spriteBatch, Rectangle rect, Color color, string buttonID, string text) : base(ref spriteBatch)
    {
        this.rectangle = rect;
        this.baseColor = color;
        this.buttonID = buttonID;
        this.text = text;
    }

    public override void Draw(GameTime gameTime)
    {
        spriteBatch.Draw(texture,rectangle,baseColor);
    }

    public void OnClick(Vector2 pos) 
    {
        if(IsPressOnThisItem(pos))
        {
            baseColor = Color.Yellow;

            OnClicked.Invoke();
        }
    }
}
