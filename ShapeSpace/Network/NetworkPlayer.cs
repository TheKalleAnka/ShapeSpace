using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using System.Collections.Generic;

namespace ShapeSpace.Network
{
    public class NetworkPlayer : Player
    {
        public List<InputWithTime> inputs = new List<InputWithTime>();
        private float lastChangedInput = 0;

        public NetConnection netConnection { get; private set; }
        public string Username { get; private set; }
        //public int PlayerIndex { get; set; }

        public Body body;

        public NetworkPlayer(ref World world, NetConnection connection, Vector2 position) : base(null)
        {
            netConnection = connection;

            body = new Body(world, position);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Position = position;
            body.Restitution = 10;

            Vertices verts = new Vertices();
            verts.Add(new Vector2(-power / 2, power / 2));
            verts.Add(new Vector2(-power / 2, -power / 2));
            verts.Add(new Vector2(power / 2, -power / 2));
            verts.Add(new Vector2(power / 2, power / 2));

            PolygonShape s = new PolygonShape(verts, 0);

            body.CreateFixture(s);
        }

        public void Update(float deltaTime)
        {
            lastChangedInput += deltaTime;

            //Remove the first occurence if it has overlived it's time
            if (inputs.Count >= 2)
                if (lastChangedInput >= inputs[1].TimeSincePrevious)
                    inputs.RemoveAt(0);

            if (inputs.Count >= 1)
                //body.Position += inputs[0].Input;
                body.ApplyForce(inputs[0].Input * 50f);

            //System.Console.WriteLine(body.LinearVelocity);
        }

        public void SetUserName(string name)
        {
            Username = name;
        }
    }
}