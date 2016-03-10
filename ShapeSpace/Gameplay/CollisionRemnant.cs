using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeSpace.Gameplay;

namespace ShapeSpace.Gameplay_Classes
{
    class CollisionRemnant : Particle
    {
        public CollisionRemnant(Vector2 position, float size, Color color, GraphicsDevice gDev, Player creator) : base(position, size, color, gDev, creator) { }
    }
}
