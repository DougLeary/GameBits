using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	/// <summary>
	/// An object generator that can resolve itself to an atomic object or another object generator
	/// </summary>
	public interface IResolver : IComparable
	{
		/// <summary>
		/// An object that can resolve itself to an atomic object (such as itself or a generated object) 
		/// or to an object generator; this allows interchangeable use of atomic objects and generators.
		/// </summary>
		/// <returns></returns>
		IResolver Resolve();
	}
}
