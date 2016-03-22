using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeSpace.Gameplay
{
    public abstract class Particle
    {
        public delegate void ParticleCallbackEventHandler(int index);
        public event ParticleCallbackEventHandler OnDestroy;

        public int Id;
        public int OwnerId;
        public float size;
        float currentSize;
        public Vector2 position;
        Vector2 currentPosition;

        public float lastUpdatedValues = -1;

        public bool canCollideWithOwner = false;

        protected Color color;
        protected Texture2D texture;

        //Allows the derived classes to set necessary values
        protected Particle(Vector2 pos, float size, Color color, GraphicsDevice gDev, Player creator)
        {
            this.position = pos;
            this.currentPosition = pos;
            this.size = size;
            this.currentSize = size;
            this.color = color;

            if (gDev != null)
            {
                texture = new Texture2D(gDev, 1, 1);
                texture.SetData<Color>(new[] { Color.White });
            }
            if(creator != null)
            {
                //creator.power -= size / 500f;
            }
        }

        /// <summary>
        /// Draws the particle
        /// Only gets called on the client
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the game</param>
        public void Draw(ref SpriteBatch sb)
        {
            currentSize = size;

            //Prevents NullReferenceException
            if (texture != null)
                sb.Draw(texture, position: position - new Vector2(size / 2f, size / 2f), scale: new Vector2(currentSize, currentSize), color: color);
        }

        public virtual void Update(float deltaTime)
        {
            if (size <= 0)
                Destroy();

            if(lastUpdatedValues != -1)
            {
                //currentPosition = Vector2.Lerp(currentPosition, position, MathHelper.Clamp(deltaTime / lastUpdatedValues, 0, 1));
                currentSize = currentSize + (size - currentSize) * MathHelper.Clamp(deltaTime / lastUpdatedValues, 0, 1);
            }
        }

        /// <summary>
        /// Destroys this trail
        /// </summary>
        protected void Destroy()
        {
            OnDestroy(Id);
        }
    }
}
