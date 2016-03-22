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

        public NetworkTrail(Vector2 position, float size, int ownerId, Color color, World world, Player creator)
            : base(position, size, color, null, creator)
        {
            OwnerId = ownerId;

            if (world != null)
            {
                body = BodyFactory.CreateBody(world, ConvertUnits.ToSimUnits(position - new Vector2(size / 2f, size / 2f)));
                //Set the body as stationary
                body.BodyType = BodyType.Static;
                CreateBodyFixture();

                body.CollidesWith = Category.Cat1;
                body.CollisionCategories = Category.Cat1;
                body.UserData = this;
            }
            //System.Console.WriteLine("creted trail");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            //Updates the userdata with the current size
            body.UserData = this;
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
            body.FixtureList[0].IsSensor = true;
        }
    }
}
