using System;

namespace GameBits
{
	/// <summary>
	/// Resolves a result from a KeyedList using a Key value
	/// </summary>
	public class KeyedListRoll
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

	}
}

/*
 * TODO: need a syntax for specifying a KeyedListRoll in a RollableTable;
 * Example:
 *		<KeyedListRoll TableName="Monster Treasure" Key="D" Rolls="2" />.
 * Also need a string designator for a Treasure Type in a Monster description;
 * Example:
 *		"A,C,2H" would generate 3 KeyedListRolls: (Key="A", Rolls=1), (Key="C", Rolls=1), (Key="H", Roll=2);
 * 
 * Come to think of it, RollableTable and KeyedList should both be descendants of an abstract type ResolvableTable.
 * ResolvableTable would have a Dictionary<T, string> and an abstract method GetItem(<T> keyValue).
 * KeyedList would use Dictionary<string, string>
 * RollableTable would use Dictionary<int, string>
 * KeyedList's GetItem method would use the base Dictionary's string match.
 * RollableTable's GetItem would sequentially find the first item whose key is >= keyValue.
 * 
 * The original reason for implementing RollableTable as a DataTable containing RollableTableRows was
 * to make it easily bindable to ASP.Net web controls; with AJAX/JSON/SOA this is not a design issue anymore. 

*/
