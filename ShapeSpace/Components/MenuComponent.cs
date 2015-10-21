using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

class MenuComponent : BaseComponent, IDrawable, IUpdateable
{
    Menu currentMenu = null;

    public MenuComponent(GraphicsDevice graphicsDevice) : base(graphicsDevice) { }

    public void Draw(GameTime gameTime)
    {
        if (currentMenu != null)
            currentMenu.Draw(gameTime);
    }

    public void Update(GameTime gameTime)
    {
        if(currentMenu == null)
        {
            switch(gameState)
            {
                case GameStates.MAINMENU:
                    break;
            }
        }
    }

    public override void UpdateGameState(GameStates newGameState)
    {
        base.UpdateGameState(newGameState);

        currentMenu = null;
    }

    void LoadMenus()
    {
        //Load the menus from a file decoupled from the script?
    }
}
