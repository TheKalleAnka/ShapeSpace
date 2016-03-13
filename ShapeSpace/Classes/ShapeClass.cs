using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Classes
{
    /// <summary>
    /// Serves as a basis for the game class that contains parameters that will be modified by derived classes 
    /// </summary>
    public abstract class ShapeClass
    {
        //public int maxPower;
        public float collisionModifier = 0.05f;
        //public float speed;
        public float acceleration = 10f;
        public float bounciness = 0.5f;
        public bool doesCreateTrail = false;
    }
}
