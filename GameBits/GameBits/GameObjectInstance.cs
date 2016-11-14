using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	public class GameObjectInstance : IResolver
	{
		private GameObject _item;
		private int _count;

		public GameObject Item
		{
			get { return _item; }
			set { _item = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		// parameterless constructor for serialization only
		public GameObjectInstance()
		{
		}

		public GameObjectInstance(GameObject item, int count)
		{
			Item = item;
			Count = count;
		}

		public GameObjectInstance(GameObject item)
			: this(item, 1)
		{
		}

		public static bool TryParse(string text, out GameObjectInstance instance)
		{
			// TO DO: find an existing GameObject with the same name
			char[] blank = { ' ' };
			string[] tokens = text.Trim().Split(blank, StringSplitOptions.RemoveEmptyEntries);

			int count;
			if (tokens.Length > 1 && int.TryParse(tokens[0], out count))
			{
				string name = text.Trim().Substring(tokens[0].Length).Trim();
				GameObject obj = new GameObject(name);
				instance = new GameObjectInstance(obj, count);
				return true;
			}
			else
			{
				instance = null;
				return false;
			}
		}

		public override string ToString()
		{
			if (Count <= 0)
			{
				return String.Empty;
			}

			if (Count > 1)
			{
				return Count.ToString() + " " + Item.Plural;
			}

			return Item.Name;
		}

		public IResolver Resolve()
		{
			return this;
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
