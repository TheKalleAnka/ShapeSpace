using System;
using Microsoft.Xna.Framework;

namespace ShapeSpaceHelper
{
    public enum ShapeCustomNetMessageType
    {
        LocationUpdate,
        InputUpdate
    }

    public struct PositionInTime
    {
        float Time;
        Vector2 Position;
    }
}
