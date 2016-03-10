using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Lidgren.Network;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using System.Collections.Generic;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics;

namespace ShapeSpace.Network
{
    public class NetworkPlayer : Player
    {
        public delegate void CollisionWithTrail(float amount);
        public event CollisionWithTrail OnCollisionWithTrail;

        public List<InputWithTime> inputs = new List<InputWithTime>();
        private float lastChangedInput = 0;

        public NetConnection netConnection { get; private set; }
        public string Username { get; private set; }
        //public int PlayerIndex { get; set; }

        public Body body;
        private World world;

        public NetworkPlayer(ref World world, NetConnection connection, Vector2 position) : base(null)
        {
            this.world = world;
            this.netConnection = connection;

            body = new Body(world, position);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Position = position;
            body.Restitution = 10;
            body.UserData = this;

            Vertices verts = new Vertices();
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(power / 2f), ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(power / 2f), -ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(power / 2f), -ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(power / 2f), ConvertUnits.ToSimUnits(power / 2f)));

            PolygonShape s = new PolygonShape(verts, 0);

            body.CreateFixture(s);

            body.OnCollision += body_OnCollision;
        }
        
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //System.Console.WriteLine((fixtureA.Body.UserData.GetType()).ToString() + " " + (fixtureB.Body.UserData.GetType()).ToString());

            //Does not work due to the other fixture reporting userdata that is System.Single. Unknown why. EDIT: Now fixed!
            if(fixtureA.Body.UserData as NetworkPlayer != null && fixtureB.Body.UserData as NetworkPlayer != null)
            {
                //Fixture otherFixture = (fixtureA.Body.UserData as NetworkPlayer).indexOnServer != this.indexOnServer ? fixtureA : fixtureB;

                //power += ((NetworkTrail)(otherFixture.Body.UserData)).size;

                //return false;
            }

            return true;
        }

        public void Update(float deltaTime)
        {
            lastChangedInput += deltaTime;
            /*
            //Remove the first occurence if it has overlived it's time
            if (inputs.Count >= 2)
                if (lastChangedInput >= inputs[1].TimeSincePrevious)
                    inputs.RemoveAt(0);
            */
            if (inputs.Count >= 2)
            {
                float behindInTime = 0;

                for (int i = 0; i < inputs.Count - 1; i++)
                {
                    if (lastChangedInput >= inputs[0].TimeSincePrevious + behindInTime)
                    {
                        inputs.RemoveAt(0);

                        behindInTime += inputs[0].TimeSincePrevious;
                    }
                }
            }

            if (inputs.Count >= 1)
                //body.Position += inputs[0].Input;
                body.ApplyForce(inputs[0].Input * 5f);

            //System.Console.WriteLine(body.LinearVelocity);

            if (Vector2.Distance(positionLastAddedTrail, ConvertUnits.ToDisplayUnits(body.Position)) > 3f)
                CreateNewRowOfTrail(ConvertUnits.ToDisplayUnits(body.Position), power * 2f/3f, trail.Count);

            for(int i = 0; i < trail.Count; i++)
            {
                trail[i].Update(deltaTime);
            }
        }

        public void CreateNewRowOfTrail(Vector2 pos, float size, int id)
        {
            NetworkTrail newTrail = new NetworkTrail(pos, size, body.FixtureList[0].FixtureId, Color.Blue, world, this);
            newTrail.Id = id;
            newTrail.body.UserData = indexOnServer;
            newTrail.OnDestroy += DestroyTrail;
            trail.Add(newTrail);

            positionLastAddedTrail = ConvertUnits.ToDisplayUnits(body.Position);
        }

        public void AddPower(float amount)
        {
            power += amount;
        }

        public void SetUserName(string name)
        {
            Username = name;
        }
    }
}