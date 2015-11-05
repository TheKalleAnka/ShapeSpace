using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

class UIComponent : BaseComponent, IDrawable, IUpdateable, IMenuClickable
{
    Menu currentMenu = null;
    UICallback callbackShell;

    public UIComponent(GraphicsDevice graphicsDevice, UICallback callback) : base(graphicsDevice) 
    {
        this.callbackShell = callback;

        currentMenu = CreateMainMenu();
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(/*transformMatrix: camera.GetViewMatrix()*/);

        if (currentMenu != null)
            currentMenu.Draw(gameTime);

        spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        
    }

    public override void UpdateGameState(GameStates newGameState)
    {
        base.UpdateGameState(newGameState);

        //currentMenu = null;
    }

    void LocalUICallback(string id)
    {
        switch(id)
        {
            case "BUTTON_START_GAME":
                currentMenu = CreatePauseMenu();
                break;
        }
    }

    void LoadMenu()
    {
        //Load the menus from a file decoupled from the script?
    }

    Menu CreateMainMenu()
    {
        Menu menu = new Menu();

        Button item = new Button(ref spriteBatch,new Rectangle(100,100,200,50), Color.ForestGreen, "BUTTON_START_GAME", "Play");
        item.Callback += callbackShell;
        item.Callback += LocalUICallback;
        menu.AddItem(item);

        item = new Button(ref spriteBatch, new Rectangle(100, 300, 100, 30), Color.Yellow, "BUTTON_QUIT_GAME", "Quit");
        item.Callback += callbackShell;
        menu.AddItem(item);

        return menu;
    }

    Menu CreatePauseMenu()
    {
        Menu menu = new Menu();

        Button item = new Button(ref spriteBatch, new Rectangle(100, 100, 200, 50), Color.ForestGreen, "BUTTON_RETURN_TO_PLAY", "Resume");
        item.Callback += callbackShell;
        menu.AddItem(item);

        return menu;
    }

    public void OnClick(Vector2 pos)
    {
        if(currentMenu != null)
            currentMenu.OnClick(pos);
    }
}
