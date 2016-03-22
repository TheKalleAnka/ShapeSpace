using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeSpace.Gameplay
{
    public class CollisionRemnant : Particle
    {
        public CollisionRemnant(Vector2 position, float size, Color color, GraphicsDevice gDev, Player creator) : base(position, size, color, gDev, creator) { }
    }
}
