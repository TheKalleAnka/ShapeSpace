using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using ShapeSpace.Classes;
using ShapeSpace.Gameplay;

namespace ShapeSpace.Network
{
    public class NetworkPlayer : Player
    {
        public List<InputWithTime> inputs = new List<InputWithTime>();
        private float lastChangedInput = 0;

        public NetConnection netConnection { get; private set; }
        public string Username { get; private set; }
        //public int PlayerIndex { get; set; }

        //The body that interacts in the physicsworld
        public Body body;
        //Container object for the world
        private World world;

        private ShapeClass shapeClass;

        public delegate void CreateRemnantEventHandler(Vector2 pos, float size, float angle, int ownerId, Player creator);
        public event CreateRemnantEventHandler OnCreateRemnant;

        public NetworkPlayer(World world, NetConnection connection, Vector2 position) : base(null)
        {
            this.world = world;
            this.netConnection = connection;

            //Define the body
            body = BodyFactory.CreateBody(world);
            body.BodyType = BodyType.Dynamic;
            body.FixedRotation = true;
            body.Position = ConvertUnits.ToSimUnits(position);
            body.Friction = 0;
            body.UserData = this;
            body.IsBullet = true;

            body.CollidesWith = Category.Cat1;
            body.CollisionCategories = Category.Cat1;

            //Give the body its starting fixture
            CreateFixture();
        }

        public void Update(float deltaTime)
        {
            lastChangedInput += deltaTime;

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

            //Actual movement occurs here
            //The class determines how quickly the player accelerates
            if (inputs.Count >= 1)
                body.ApplyForce(inputs[0].Input * shapeClass.acceleration);

            //Create the trail if the class allows it
            if(shapeClass.doesCreateTrail)
                if (Vector2.Distance(positionLastAddedTrail, ConvertUnits.ToDisplayUnits(body.Position)) > power * 0.5f)
                    CreateNewRowOfTrail(ConvertUnits.ToDisplayUnits(body.Position), power, trail.Count);
           
            for(int i = 0; i < trail.Count; i++)
            {
                trail[i].Update(deltaTime);
            }
        }

        /// <summary>
        /// Handles what happens when colliding with objects
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns>True if the objects should collide, false if they should pass through each other</returns>
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //Check which fixture is us and which is the fixture we collided with
            Fixture otherFixture = (fixtureA.Body.UserData as NetworkPlayer).indexOnServer == this.indexOnServer ? fixtureB : fixtureA;

            //Handles when we collide with another player
            if (otherFixture.Body.UserData as NetworkPlayer != null)
            {
                for (float angle = 0; angle < 360; angle += 6)
                {
                    if (OnCreateRemnant != null)
                        OnCreateRemnant(ConvertUnits.ToDisplayUnits(body.Position), power / 10f, angle, indexOnServer, this);
                }
                contact.Restitution = 1;

                return true;
            }
            else if(otherFixture.Body.UserData as Particle != null)
            {
                Particle particle = otherFixture.Body.UserData as Particle;

                if (indexOnServer != particle.OwnerId || particle.canCollideWithOwner)
                {
                    //SetPower(power + particle.size / 10f);
                    particle.size = 0;
                }

                return false;
            }

            return true;
        }

        void body_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            //CreateFixture();
        }

        /// <summary>
        /// Creates a new trail particle
        /// </summary>
        /// <param name="pos">The position in world draw coordinates</param>
        /// <param name="size">The size in pixels</param>
        /// <param name="id"></param>
        public void CreateNewRowOfTrail(Vector2 pos, float size, int id)
        {
            NetworkTrail newTrail = new NetworkTrail(pos, size, indexOnServer, Color.Blue, world, this);
            newTrail.Id = id;
            newTrail.body.UserData = indexOnServer;
            newTrail.OnDestroy += DestroyTrail;
            trail.Add(newTrail);

            positionLastAddedTrail = ConvertUnits.ToDisplayUnits(body.Position);
        }

        /// <summary>
        /// Creates the fixture from a shape and attaches it to the player
        /// The fixture is the collision object
        /// </summary>
        void CreateFixture()
        {
            if(body.FixtureList.Count > 0)
            {
                body.DestroyFixture(body.FixtureList[0]);
                //return;
            }
            
            body.CreateFixture(CreateShape());
            //body.FixtureList[0].OnCollision += body_OnCollision;
            body.OnCollision += body_OnCollision;
            body.OnSeparation += body_OnSeparation;
        }

        /// <summary>
        /// Creates a PolygonShape that corresponds to the size of the player
        /// </summary>
        /// <returns></returns>
        PolygonShape CreateShape()
        {
            Vertices verts = new Vertices();
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(power / 2f), ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(-ConvertUnits.ToSimUnits(power / 2f), -ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(power / 2f), -ConvertUnits.ToSimUnits(power / 2f)));
            verts.Add(new Vector2(ConvertUnits.ToSimUnits(power / 2f), ConvertUnits.ToSimUnits(power / 2f)));

            return new PolygonShape(verts, 0);
        }

        /// <summary>
        /// Sets the class and all corresponding values
        /// </summary>
        /// <param name="type">The class</param>
        public void SetClass(ShapeClass type)
        {
            shapeClass = type;

            SetPower(shapeClass.startPower);

            CreateFixture();
        }

        public void SetPower(float amount)
        {
            power = amount;

            //Update the fixture with the new size
            //CreateFixture();
        }

        /// <summary>
        /// Set the username of the player
        /// </summary>
        /// <param name="name"></param>
        public void SetUserName(string name)
        {
            Username = name;
        }
    }
}