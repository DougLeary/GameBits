using System;

namespace GameBits
{
	/// <summary>
	/// Resolves a result from a KeyedList using a Key value
	/// </summary>
	public class KeyedListRoll : IResolver
	{
		private KeyedList _list;
		public KeyedList List
		{
			get { return _list; }
			set { _list = value; }
		}

		// list key to select
		public string Key = String.Empty;

		// number of times to roll, i.e. to generate results using the same Key item
		public int Rolls = 1;

		public KeyedListRoll(KeyedList list)
		{
			List = list;
		}

		public KeyedListRoll()
			: this(null)
		{
		}

		public IResolver Resolve()
		{
            Logger.Write("Resolve KeyedListRoll");

            return List.Resolve(Key);
		}

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                return String.Compare(this.ToString(), other.ToString());
            }
        }
    }
}

