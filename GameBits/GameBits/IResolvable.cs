using System;

namespace GameBits
{
    /// <summary>
    /// An object generator that can be resolved to an atomic object or another object generator.
    /// Note: Resolve methods always generate IResolvers,  not IResolvables. 
    /// </summary>
    public interface IResolvable
    {
        IResolver Resolve();
    }
}
