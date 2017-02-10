using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBits
{
    public interface IRollable
    {
        IResolver Roll();
    }
}
