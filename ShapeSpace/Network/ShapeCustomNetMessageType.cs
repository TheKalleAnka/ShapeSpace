using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeSpace.Network
{
    /// <summary>
    /// The custom message types for data messages sent by the game
    /// </summary>
    public enum ShapeCustomNetMessageType
    {
        SetupRequest,
        SetupSuccessful,
        SetupFailed,
        LocationUpdate,
        InputUpdate
    }
}
