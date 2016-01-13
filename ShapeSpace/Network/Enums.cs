using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Network
{
    public enum ShapeCustomNetMessageType
    {
        ConnectionRequest,
        ConnectionSuccessful,
        ConnectionFailed,
        LocationUpdate,
        InputUpdate
    }
}
