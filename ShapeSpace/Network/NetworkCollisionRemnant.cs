using System;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using ShapeSpace.Gameplay;

namespace ShapeSpace.Network
{
    public class NetworkCollisionRemnant : CollisionRemnant
    {
        public Body body;

        private float enableCollisionCountdownTimer = 1;

        public NetworkCollisionRemnant(Vector2 position, float size, float angleOfStartForce, int ownerId, Color color, World world, Player creator)
            : base(position, size, color, null, creator)
        {
            OwnerId = ownerId;

            body = BodyFactory.CreateBody(world);
            body.BodyType = BodyType.Dynamic;
            body.Position = ConvertUnits.ToSimUnits(position);
            body.FixedRotation = true;
            body.LinearDamping = 0.8f;

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
            body.UserData = this;

            body.CollidesWith = Category.Cat2;
            body.CollisionCategories = Category.Cat2;
            //body.OnSeparation += body_OnSeparation;
            
            body.ApplyLinearImpulse(new Vector2((float)Math.Cos(angleOfStartForce),(float)Math.Sin(angleOfStartForce)) * 3f);
        }

        void body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            //Check which fixture is us and which is the fixture we collided with
            Fixture otherFixture = (fixtureA.Body.UserData as NetworkCollisionRemnant).Id == this.Id ? fixtureB : fixtureA;

            //Does not work
            if(otherFixture.Body.UserData as NetworkPlayer != null)
                if ((otherFixture.Body.UserData as NetworkPlayer).indexOnServer == OwnerId)
                    canCollideWithOwner = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (enableCollisionCountdownTimer <= 0)
            {
                body.CollidesWith = Category.Cat1;
                body.CollisionCategories = Category.Cat1;
            }
            else
                enableCollisionCountdownTimer -= deltaTime;

            //w.Step(0.1f);
            position = ConvertUnits.ToDisplayUnits(body.Position);
            //body.ApplyForce(new Vector2(10,20));
        }
    }
}
