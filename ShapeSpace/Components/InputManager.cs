using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public static class InputManager
{
    static KeyboardState currentKeyState;
    static KeyboardState prevKeyState;

    static MouseState currentMouseState;
    static MouseState prevMouseState;

    public static bool GameIsActive { get; private set; }

    //static Actor reserveInput = null;

    public static void SetActive(bool isActive)
    {
        GameIsActive = isActive;
    }

    public static void Update(GameTime gameTime)
    {
        //Updates the keyboard state
        prevKeyState = currentKeyState;
        currentKeyState = Keyboard.GetState();

        //Updates mouse states
        prevMouseState = currentMouseState;
        currentMouseState = Mouse.GetState();
    }

    public static bool IsKeyTriggered(Keys key)
    {
        if (GameIsActive)
            if (currentKeyState.IsKeyDown(key) && !prevKeyState.IsKeyDown(key))
                return true;

        return false;
    }

    public static bool IsKeyPressed(Keys key)
    {
        if (GameIsActive)
            if (currentKeyState.IsKeyDown(key))
                return true;

        return false;
    }

    public static bool IsKeyReleased(Keys key)
    {
        if (GameIsActive)
            if (!currentKeyState.IsKeyDown(key) && prevKeyState.IsKeyDown(key))
                return true;

        return false;
    }

    public static bool IsMouseButtonTriggered()
    {
        if(GameIsActive)
            if (currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed)
                return true;

        return false;
    }

    public static bool IsMouseButtonReleased()
    {
        if (GameIsActive)
            if (currentMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                return true;

        return false;
    }

    public static bool IsScrollingMouseWheelIn()
    {
        if (GameIsActive)
            if (currentMouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue < 0)
                return true;

        return false;
    }

    public static bool IsScrollingMouseWheelOut()
    {
        if (GameIsActive)
            if (currentMouseState.ScrollWheelValue - prevMouseState.ScrollWheelValue > 0)
                return true;

        return false;
    }

    /// <summary>
    /// (NOT IMPLEMENTED)Provides classes with a framework for receiving all key presses as they are pressed down
    /// </summary>
    public static void ReserveInput()
    {
        //reserveInput = actor;
    }
    public static void UnreserveInput()
    {
        //reserveInput = null;
    }

    /// <summary>
    /// Constructs a Vector2 from movement inputs
    /// </summary>
    /// <returns>Movement Vector2</returns>
    public static Vector2 GetMovementInputAsVector()
    {
        Vector2 vector = Vector2.Zero;

        if (IsKeyPressed(Keys.A))
            vector.X = -1;
        if (IsKeyPressed(Keys.D))
            vector.X = 1;
        if (IsKeyPressed(Keys.W))
            vector.Y = -1;
        if (IsKeyPressed(Keys.S))
            vector.Y = 1;

        return vector;
    }
}
