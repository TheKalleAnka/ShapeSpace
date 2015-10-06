using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//***********The master class*******************//
//Anything that shall be displayed on the screen must derive from this
namespace ShapeSpace
{
    class Actor : DrawableGameComponent
    {
        public Actor(Game game, ref SpriteBatch spriteBatch) : base(game)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
