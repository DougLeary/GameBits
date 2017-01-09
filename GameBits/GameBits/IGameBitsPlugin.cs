using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBits
{
    /// <summary>
    /// Extender plugin to handle game-system-specific data objects
    /// </summary>
    public interface IGameBitsPlugin
    {
        string Name { get; }
        void Do();
    }
}
