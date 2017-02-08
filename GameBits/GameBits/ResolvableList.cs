using System;
using System.Collections.Generic;

namespace GameBits
{
    public abstract class ResolvableList<KeyType> : IResolver
    {
        protected Dictionary<KeyType, IResolver> _list;

        public abstract IResolver Resolve();
        public abstract IResolver GetItem(KeyType keyValue);
        public abstract int CompareTo(Object other);
    }
}
