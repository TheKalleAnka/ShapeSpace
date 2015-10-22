using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

class GameComponent : BaseComponent, IDrawable, IUpdateable, Observer, Subject
{
    //GAMEPLAY
    Player player;

    public List<Observer> observers { get; set; }

    public GameComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) 
    {
        camera = new Camera(graphicsDevice.Viewport);//Use for spritebatch.begin
    }

    public void Update(GameTime gameTime)
    {
        //throw new NotImplementedException();
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(transformMatrix: camera.GetViewMatrix());

        //Draw here

        spriteBatch.End();
    }

    public void OnNotify(object caller, string eventID)
    {
        throw new NotImplementedException();
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }
}
