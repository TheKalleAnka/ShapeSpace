using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Network
{
    /// <summary>
    /// Serves as a basis for the game class that contains parameters that will be modified by derived classes 
    /// </summary>
    abstract class ShapeClass
    {
        int maxPower;
        float collisionModifier;
        float speed;
        float acceleration;
    }
}
