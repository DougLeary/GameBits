using System;
using System.Collections.Generic;

namespace GameBits
{
	/// <summary>
	/// A keyed list of rollable items, where a row is selected by key instead of by dieroll. 
	/// Useful for such things as a Treasure Types table. 
	/// </summary>
	public class KeyedList : ResolvableList<string>, IResolver
	{
        public KeyedList()
		{
            _list = new Dictionary<string, IResolver>(StringComparer.OrdinalIgnoreCase);
        }

        public override IResolver GetItem(string key)
        { 
            if (_list.ContainsKey(key))
            {
                return _list[key];
            }
            else
            {
                return null;
            }
        }

        public override int CompareTo(Object other)
        {
            return String.Compare(this.ToString(), other.ToString());
        }

        /// <summary>
        /// Resolve the selected item(s) found by keys in keyList 
        /// </summary>
        /// <param name="KeyList">Comma-separated list of key values</param>
        /// <returns></returns>
        public IResolver Resolve(string KeyList)
		{
            Logger.Write("Resolve KeyedList, keys: " + KeyList);
            string[] keys = KeyList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			ItemList result = new ItemList();
			foreach (string key in keys)
			{
                IResolver item;
				if (_list.TryGetValue(key, out item))
				{
                    if (item is ItemList)
                    {
                        foreach (IResolver thing in (ItemList)item)
                        {
                            IResolver resolvedThing = thing.Resolve();
                            if (resolvedThing != null)
                            {
                                result.Add(resolvedThing);
                            }
                        }
                    }
                    else
                    {
                        return item.Resolve();
                    }
				}
			}
			return result;
		}

        /// <summary>
        /// Return empty result since no keys are provided
        /// </summary>
        /// <returns></returns>
		public override IResolver Resolve()
		{
			return Resolve(String.Empty);
		}

        public void Add(string key, IResolver item)
        {
            _list.Add(key, item);
        }

	}
}
