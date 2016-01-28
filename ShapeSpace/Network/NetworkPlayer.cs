﻿using Microsoft.Xna.Framework;
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
        public int PlayerIndex { get; set; }

        public Body body;

        public NetworkPlayer(World world, NetConnection connection, Vector2 position) : base(null)
        {
            netConnection = connection;

            body = new Body(world, position);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Position = position;

            Vertices verts = new Vertices();
            verts.Add(new Vector2(-power / 2, power / 2));
            verts.Add(new Vector2(-power / 2, -power / 2));
            verts.Add(new Vector2(power / 2, -power / 2));
            verts.Add(new Vector2(power / 2, power / 2));

            PolygonShape s = new PolygonShape(verts, 0);

            body.CreateFixture(s);
        }

        public void Update(float loopTime)
        {
            lastChangedInput += loopTime;

            if(inputs.Count > 1)
                if(lastChangedInput >= inputs[1].TimeSincePrevious)
                {
                    inputs.RemoveAt(0);
                    lastChangedInput = 0;
                }

            if(inputs.Count > 0)
                body.ApplyForce(inputs[0].Input * 5f);
        }

        public void SetUserName(string name)
        {
            Username = name;
        }
    }
}
