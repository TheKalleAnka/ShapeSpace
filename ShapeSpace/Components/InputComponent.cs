using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

class InputComponent : IUpdateable
{
    /// <summary>
    /// Contains references to all actors that want input
    /// </summary>
    List<Actor> requestsInputs = new List<Actor>();

    Actor reserveInput = null;

    public void Update(GameTime gameTime)
    {
        
    }

    public void Register(ref Actor actor)
    {
        requestsInputs.Add(actor);
    }

    public void Unregister(ref Actor actor)
    {
        requestsInputs.Remove(actor);
    }

    public void ReserveInput(ref Actor actor)
    {
        reserveInput = actor;
    }

    public void UnreserveInput()
    {
        reserveInput = null;
    }

    public static Vector2 GetMovementInputAsVector()
    {
        Vector2 vector = Vector2.Zero;

        if (Keyboard.GetState().IsKeyDown(Keys.A))
            vector.X -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.D))
            vector.X += 1;
        if (Keyboard.GetState().IsKeyDown(Keys.W))
            vector.Y -= 1;
        if (Keyboard.GetState().IsKeyDown(Keys.S))
            vector.Y += 1;

        return vector;
    }
}
