using System;

namespace GameBits
{
	/// <summary>
	/// A resolvable object that can be compared with others for sorting or normalization. 
    /// To be part of the results of a Resolve, an object must be an IResolver.
	/// </summary>
	public interface IResolver: IResolvable, IComparable
	{
    }
}
