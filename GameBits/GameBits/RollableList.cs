using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	/// <summary>
	/// A keyed list of rollable items, where a row is selected by key instead of by dieroll. 
	/// Useful for such things as a Treasure Types table. 
	/// </summary>
	public class RollableList : Dictionary<string, ItemList>
	{
		public RollableList() : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		/// <summary>
		/// Resolve the selected item 
		/// </summary>
		/// <param name="keyList">Comma-separated list of key values</param>
		/// <returns></returns>
		public IResolver ResolveItem(string keyList)
		{
			string[] keys = keyList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			ItemList resolvedList = new ItemList();
			foreach (string key in keys)
			{
				ItemList rollList;
				if (TryGetValue(key, out rollList))
				{
					foreach (IResolver item in rollList)
					{
						IResolver resolvedItem = item.Resolve();
						if (resolvedItem != null)
						{
							resolvedList.Add(resolvedItem);
						}
					}
				}
			}
			return resolvedList;
		}

		public IResolver Resolve()
		{
			return ResolveItem("dummy");
		}

		public int CompareTo(object other)
		{
			return String.Compare(this.ToString(), other.ToString());
		}

	}
}
