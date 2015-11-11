using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

class Menu : IDrawable, IMenuClickable
{
    List<MenuItem> items = new List<MenuItem>();

    public void AddItem(MenuItem item)
    {
        items.Add(item);
    }

    public void Draw(GameTime gameTime)
    {
        for(int i = 0; i < items.Count; i++)
        {
            items[i].Draw(gameTime);
        }
    }

    public void OnClick(Vector2 pos)
    {
        for (int i = 0; i < items.Count; i++)
        {
            IMenuClickable item;

            if (items[i] is IMenuClickable)
                item = items[i] as IMenuClickable;
            else
                continue;

            item.OnClick(pos);
        }
    }

    public List<Observer> observers
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public void AddObserver(Observer observer)
    {
        throw new NotImplementedException();
    }

    public void RemoveObserver(Observer observer)
    {
        throw new NotImplementedException();
    }
}
