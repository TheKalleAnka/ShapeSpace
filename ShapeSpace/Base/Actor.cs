using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Is the base class that anything that will exist in the world or gets drawn on the screen will inherit from
/// </summary>
class Actor
{
    /// <summary>
    /// References to all the different components
    /// </summary>
    InputComponent InputComponent;
    UIComponent UIComponent;
    GameComponent GameComponent;

    public Actor(ref InputComponent input, ref UIComponent ui, ref GameComponent game)
    {
        
    }
}
