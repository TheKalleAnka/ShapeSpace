using Microsoft.Xna.Framework;

namespace ShapeSpace.Network
{
    public class PositionInTime
    {
        public float TimeSincePrevious;
        public Vector2 Position;

        public PositionInTime(float time, Vector2 pos)
        {
            TimeSincePrevious = time;
            Position = pos;
        }
    }
}
