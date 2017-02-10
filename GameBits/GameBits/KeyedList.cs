using System;
using System.Collections.Generic;

namespace GameBits
{
	/// <summary>
	/// A keyed list of rollable items, where a row is selected by key instead of by dieroll. 
	/// Useful for such things as a Treasure Types table. 
	/// </summary>
	public class KeyedList : IResolvableList<string>
	{
        private Dictionary<string, IResolver> _list;

        public KeyedList()
		{
            _list = new Dictionary<string, IResolver>(StringComparer.OrdinalIgnoreCase);
        }

        public IResolver GetItem(string key)
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
        /// Return a resolved, randomly selected item from the list
        /// </summary>
        /// <returns></returns>
        public IResolver Roll()
        {
            return Roll(0, int.MaxValue);
        }
        
        /// <summary>
        /// Return a resolved, randomly selected item from the list
        /// </summary>
        /// <param name="ignoreBelow"></param>
        /// <param name="ignoreAbove"></param>
        /// <returns></returns>
        public IResolver Roll(int ignoreBelow, int ignoreAbove)
        {
            DieRoll dice = new DieRoll(1, _list.Count, 0);
            int roll = -1;
            int attempts = 0;
            while (attempts <= Constants.MaxRollAttempts && (roll < ignoreBelow || roll > ignoreAbove))
            {
                roll = dice.Roll();
                attempts++;
            }

            Dictionary<string, IResolver>.Enumerator en = _list.GetEnumerator();
            for (int i = 0; i < roll; i++) {
                en.MoveNext();
            }
            string key = en.Current.Key;
            return this.Resolve(key);
        }

        /// <summary>
        /// Return empty result since no keys are provided
        /// </summary>
        /// <returns></returns>
		public IResolver Resolve()
		{
			return Resolve(String.Empty);
		}

        public void Add(string key, IResolver item)
        {
            _list.Add(key, item);
        }

	}
}
