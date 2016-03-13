using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Classes
{
    public class ShapeClassBank : ShapeClass
    {
        public ShapeClassBank()
        {
            acceleration = 2f;
            doesCreateTrail = false;
        }
    }
}
