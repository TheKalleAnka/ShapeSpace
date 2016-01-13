using System;
using Microsoft.Xna.Framework;

namespace ShapeSpaceHelper
{
    public struct PositionInTime
    {
        public float Time;
        public Vector2 Position;
    }

    public struct InputWithTime
    {
        public float TimeSincePrevious;
        public Vector2 Input;

        public InputWithTime(float time, Vector2 input)
        {
            TimeSincePrevious = 0;
            TimeSincePrevious = time;
            Input = Vector2.Zero;
            Input = input;
        }
    }
}
