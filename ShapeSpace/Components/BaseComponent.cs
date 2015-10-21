using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

abstract class BaseComponent
{
    /// <summary>
    /// Contains the gamestate so that we can take appropriate action
    /// </summary>
    protected GameStates gameState = GameStates.UNKNOWN;

    protected SpriteBatch spriteBatch;

    public BaseComponent(GraphicsDevice graphicsDevice)
    {
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public virtual void UpdateGameState(GameStates newGameState)
    {
        gameState = newGameState;
    }
}
