using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using ShapeSpace.UI;

class UIComponent : IDrawable, IUpdateable, IMenuClickable, ILoadable
{
    //FROM BASE
    /// <summary>
    /// Contains the gamestate so that we can take appropriate action
    /// </summary>
    private GameStates gameState = GameStates.UNKNOWN;
    /// <summary>
    /// Is public due to it being set from the shell
    /// </summary>
    public SpriteBatch spriteBatch;

    private Camera camera;

    public void UpdateGameState(GameStates newGameState)
    {
        gameState = newGameState;
    }

    //SINGLETON
    private static readonly UIComponent instance = new UIComponent();
    private UIComponent() { }
    static UIComponent() { }
    public static UIComponent Instance
    {
        get
        {
            return instance;
        }
    }

    //OTHER STUFF
    /// <summary>
    /// Contains the currently open menu
    /// </summary>
    Menu currentMenu = null;
    /// <summary>
    /// The callback method in the Shell
    /// Should be set on the instance of the singleton
    /// </summary>
    public UICallback callbackShell;

    /// <summary>
    /// FOR DEBUGGING
    /// Any class that wants to add to this shall add a string to a List/Dictionary which gets formatted into a single string
    /// </summary>
    public string _DebugString = "Hej";

    SpriteFont font;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager cManager)
    {
        font = cManager.Load<SpriteFont>("text");

        //Creates the main menu so that it displays when the game starts
        currentMenu = CreateMainMenu();
    }

    public void UnloadContent()
    {
        throw new NotImplementedException();
    }

    public void Draw(GameTime gameTime)
    {
        spriteBatch.Begin(/*transformMatrix: camera.GetViewMatrix()*/);

        if (currentMenu != null)
            currentMenu.Draw(gameTime);

        spriteBatch.DrawString(font, _DebugString, Vector2.Zero, Color.White);

        spriteBatch.End();
    }

    public void Update(GameTime gameTime)
    {
        
    }

    void LocalUICallback(string id)
    {
        switch(id)
        {
            case "BUTTON_START_GAME":
                currentMenu = CreatePauseMenu();
                break;
            default:
                currentMenu = null;
                break;
        }
    }

    /// <summary>
    /// This is called by the game when it wants to show the pause menu
    /// </summary>
    public void ShowPauseMenu()
    {
        currentMenu = CreatePauseMenu();
    }

    Menu CreateMainMenu()
    {
        Menu menu = new Menu();

        Button item = new Button(ref spriteBatch, new Rectangle(100, 100, 200, 50), Color.ForestGreen, Color.White, font, "BUTTON_START_GAME", "Play");
        item.Callback += callbackShell;
        item.Callback += LocalUICallback;
        menu.AddItem(item);

        item = new Button(ref spriteBatch, new Rectangle(100, 300, 100, 30), Color.Yellow, Color.White, font, "BUTTON_QUIT_GAME", "Quit");
        item.Callback += callbackShell;
        item.Callback += LocalUICallback;
        menu.AddItem(item);

        return menu;
    }

    Menu CreatePauseMenu()
    {
        Menu menu = new Menu();

        Button item = new Button(ref spriteBatch, new Rectangle(100, 100, 200, 50), Color.ForestGreen, Color.White, font, "BUTTON_RETURN_TO_PLAY", "Resume");
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
