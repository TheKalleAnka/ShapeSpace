using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Classes
{
    public class ShapeClassKnocker : ShapeClass
    {
        public ShapeClassKnocker()
        {
            collisionModifier = 0f;
            acceleration = 15f;
            doesCreateTrail = false;
        }
    }
}
