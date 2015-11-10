using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System;

class Button : MenuItem, IMenuClickable
{
    /// <summary>
    /// The event that is sent to all observers when this button is pressed
    /// </summary>
    string buttonID { get; set; }

    //FEEDBACK
    float scale = 1f;
    bool hasBeenPressed = false;

    public event UICallback Callback;

    public Button(ref SpriteBatch spriteBatch, Rectangle rect, Color color, string buttonID, string text) : base(ref spriteBatch)
    {
        this.rectangle = rect;
        this.baseColor = color;
        this.buttonID = buttonID;
        this.text = text;
    }

    public override void Draw(GameTime gameTime)
    {
        if (this.hasBeenPressed)
            scale -= (float)gameTime.ElapsedGameTime.TotalSeconds * 10;

        if (scale <= 0)
        {
            Callback(buttonID);
            hasBeenPressed = false;
        }

        int posX = Convert.ToInt32(rectangle.X + ((1 - scale)* 0.5f * rectangle.Width));
        int posY = Convert.ToInt32(rectangle.Y + ((1 - scale) * 0.5f * rectangle.Height));
        int sizeX = Convert.ToInt32(rectangle.Width * scale);
        int sizeY = Convert.ToInt32(rectangle.Height * scale);

        spriteBatch.Draw(texture,new Rectangle(posX, posY, sizeX, sizeY),baseColor);
    }

    public void OnClick(Vector2 pos) 
    {
        if(IsPressOnThisItem(pos))
        {
            hasBeenPressed = true;
        }
    }
}
