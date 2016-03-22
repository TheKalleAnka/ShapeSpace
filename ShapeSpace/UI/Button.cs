using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeSpace.UI
{
    class Button : MenuItem, IMenuClickable
    {
        /// <summary>
        /// The event that is sent to all observers when this button is pressed
        /// </summary>
        string buttonID { get; set; }

        int posX;
        int posY;
        int sizeX;
        int sizeY;

        //FEEDBACK
        float scale = 1f;
        bool hasBeenPressed = false;

        public event UICallback Callback;

        public Button(ref SpriteBatch spriteBatch, Rectangle rect, Color color, Color textColor, SpriteFont font, string buttonID, string text)
            : base(ref spriteBatch)
        {
            this.rectangle = rect;
            this.baseColor = color;
            this.textColor = textColor;
            this.font = font;
            this.buttonID = buttonID;
            this.text = text;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.hasBeenPressed)
                scale -= (float)gameTime.ElapsedGameTime.TotalSeconds * 10;

            if (scale <= 0)
            {
                if(Callback != null)
                    Callback(buttonID);

                hasBeenPressed = false;
            }

            Vector2 fontSize = font.MeasureString(text);

            sizeX = Convert.ToInt32(/*rectangle.Width * scale*/fontSize.X * scale + 20);
            sizeY = Convert.ToInt32(/*rectangle.Height * scale*/fontSize.Y * scale + 20);

            //Makes it decrease size towards the center
            posX = Convert.ToInt32(rectangle.X + ((1 - scale) * 0.5f * rectangle.Width));
            posY = Convert.ToInt32(rectangle.Y + ((1 - scale) * 0.5f * rectangle.Height));

            spriteBatch.Draw(texture, new Rectangle(posX, posY, sizeX, sizeY), baseColor);
            spriteBatch.DrawString(font, text, new Vector2(posX + 10 * scale, posY + 10 * scale), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public void OnClick(Vector2 pos)
        {
            if (IsPressOnThisItem(pos))
            {
                hasBeenPressed = true;
            }
        }

        new bool IsPressOnThisItem(Vector2 pos)
        {
            if (pos.X > posX &&
               pos.X < posX + sizeX &&
               pos.Y > posY &&
               pos.Y < posY + sizeY)
                return true;

            return false;
        }
    }
}
