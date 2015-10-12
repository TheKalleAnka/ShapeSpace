using Microsoft.Xna.Framework;

class Player /*: BaseComponent?*/
{
    Vector2 position;
    //There is no rotation here since no players can spin!
    int scale;//Should this be a Vector2 to allow different scaling in different axes?
    /// <summary>
    /// Determines whether this player is controlled by this instance of the program
    /// </summary>
    bool isLocal = false;

    public void Update() { }
    public void Draw() { }
}
