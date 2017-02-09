using System;
using System.Collections.Generic;

namespace GameBits
{
    public interface IResolvableList<KeyType> : IResolvable
    {
        IResolver GetItem(KeyType keyValue);
    }
}
