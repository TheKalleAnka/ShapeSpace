using Microsoft.Xna.Framework;
using System.Collections.Generic;

class Button : MenuItem, IMenuClickable
{
    /// <summary>
    /// The event that is sent to all observers when this button is pressed
    /// </summary>
    string buttonID { get; set; }
    
    public List<Observer> observers { get; set; }

    public Button(Rectangle rect, Color color, string buttonID)
    {
        this.rectangle = rect;
        this.baseColor = color;
        this.buttonID = buttonID;
    }

    public override void Draw(GameTime gameTime)
    {
        
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }

    public void OnClick(Vector2 pos) 
    {
        if(IsPressOnThisItem(pos))
        {
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnNotify(this, buttonID);
            }
        }
    }
}
