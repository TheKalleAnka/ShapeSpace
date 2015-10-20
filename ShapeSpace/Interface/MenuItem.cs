using Microsoft.Xna.Framework;

/// <summary>
/// Abstract class which all items in a menu derive from
/// </summary>
abstract class MenuItem
{
    public delegate void ClickedItem();
    /// <summary>
    /// Executes whenever an item in a menu is clicked
    /// </summary>
    public event ClickedItem OnClicked;

    public Vector2 Position = Vector2.Zero;
}
