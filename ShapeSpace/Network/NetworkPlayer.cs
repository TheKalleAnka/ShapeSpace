using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Lidgren.Network;

namespace ShapeSpace.Network
{
    public class NetworkPlayer : Player
    {
        public NetConnection netConnection { get; private set; }

        public NetworkPlayer(World world, NetConnection connection) : base(null,Vector2.Zero,world)
        {
            netConnection = connection;
        }
    }
}
