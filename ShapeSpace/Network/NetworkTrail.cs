using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeSpace.Network
{
    class NetworkTrail : Trail
    {
        Body body;

        public NetworkTrail(Vector2 position, int size) : base(position, size, Color.Beige, null)
        {

        }
    }
}
