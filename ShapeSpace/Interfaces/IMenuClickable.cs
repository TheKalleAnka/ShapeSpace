using Microsoft.Xna.Framework;

interface IMenuClickable : Subject
{
    void OnClick(Vector2 pos);
}
