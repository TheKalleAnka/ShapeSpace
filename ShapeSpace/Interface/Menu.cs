using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

class Menu : IDrawable
{
    List<MenuItem> items = new List<MenuItem>();

    public void Draw(GameTime gameTime)
    {
        for(int i = 0; i < items.Count; i++)
        {
            items[0].Draw(gameTime);
        }
    }
}
