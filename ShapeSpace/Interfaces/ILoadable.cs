using Microsoft.Xna.Framework.Content;

interface ILoadable
{
    void LoadContent(ContentManager cManager);
    void UnloadContent();
}
