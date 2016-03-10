using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

namespace ShapeSpace.Network
{
    class NetworkTrail : Trail
    {
        public Body body;
        public int OwnerId;

        public NetworkTrail(Vector2 position, float size, int ownerId, Color color, World world, Player creator)
            : base(position, size, color, null, creator)
        {
            OwnerId = ownerId;

            if (world != null)
            {
                body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(position - new Vector2(size / 2f, size / 2f)));
                //Set the body as stationary
                body.IsKinematic = true;

                //What is called when the body collides with something
                body.OnCollision += body_OnCollision;
                body.UserData = this;
            }
            //System.Console.WriteLine("creted trail");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        private bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            System.Console.WriteLine((fixtureA.Body.UserData.GetType()).ToString() + " " + (fixtureB.Body.UserData.GetType()).ToString());

            //If this trail belongs to the player that collided with it nothing should happen
            if (fixtureA.FixtureId == OwnerId || fixtureB.FixtureId == OwnerId)
                return false;

            Fixture otherFixture = fixtureA.Body.UserData as NetworkPlayer != null ? fixtureA : fixtureB;
            (otherFixture.Body.UserData as NetworkPlayer).power += size/5f;

            size = 0;

            //They should not collide
            return false;
        }

        private void CreateBodyFixture()
        {
            //Create the vertices that will create the collision shape
            Vertices verts = new Vertices();
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(size / 2f), ConvertUnits.ToSimUnits(size / 2f)));
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(size / 2f), -ConvertUnits.ToSimUnits(size / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(size / 2f), -ConvertUnits.ToSimUnits(size / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(size / 2f), ConvertUnits.ToSimUnits(size / 2f)));

            //Create the shape and attach it to the body
            PolygonShape s = new PolygonShape(verts, 0);
            body.CreateFixture(s);
        }
    }
}
