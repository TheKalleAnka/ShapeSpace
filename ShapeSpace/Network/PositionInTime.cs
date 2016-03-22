using Microsoft.Xna.Framework;

namespace ShapeSpace.Network
{
    public class PositionInTime
    {
        public float TimeSincePrevious;
        public Vector2 Position;
        public bool Temporary;

        public PositionInTime(float time, Vector2 pos, bool temp)
        {
            TimeSincePrevious = time;
            Position = pos;
            Temporary = temp;
        }
    }
}
