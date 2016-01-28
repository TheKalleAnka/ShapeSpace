using Microsoft.Xna.Framework;

namespace ShapeSpace.Network
{
    public class InputWithTime
    {
        public float TimeSincePrevious;
        public Vector2 Input;

        public InputWithTime(float time, Vector2 input)
        {
            TimeSincePrevious = time;
            Input = input;
        }
    }
}