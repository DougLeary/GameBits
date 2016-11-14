using System;
namespace GameBits
{
	public interface IRepository
	{
		System.Collections.Generic.Dictionary<string, RollableTable> Tables { get; }
	}
}
