using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Gameplay;

public class Trail : Particle
{
    //Forward necessary values to base
    public Trail(Vector2 position, float size, Color color, GraphicsDevice gDev, Player creator) : base(position, size, color, gDev, creator){ }

    public override void Update(float deltaTime)
    {
        size -= deltaTime;
        base.Update(deltaTime);
    }
}
