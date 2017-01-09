using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
    /// <summary>
    /// Generated instance of a GameObject, and the number of individuals occurring. 
    /// </summary>
	public class GameObjectInstance : IResolver
	{
		private GameObject _item;
        private ItemList _contents;
        private int _count;

        /// <summary>
        /// GameObject from which the instance was generated
        /// </summary>
		public GameObject Item
		{
			get { return _item; }
			set { _item = value; }
		}

        /// <summary>
        /// Number of objects that occur in this instance
        /// </summary>
		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

        public ItemList Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        // parameterless constructor for serialization only
        public GameObjectInstance()
		{
		}

		public GameObjectInstance(GameObject item, int count)
		{
			Item = item;
			Count = count;
            Contents = null;
		}

		public GameObjectInstance(GameObject item)
			: this(item, 1)
		{
		}

		public static bool TryParse(string text, out GameObjectInstance instance)
		{
			// TODO: find an existing GameObject with the same name before creating one;
            //   parsing the output of GameObjectInstance.ToString() will require converting pluralized names back to singular;
            //   We might only be able to parse well-structured plurals, as in "N singular-name" or "singular-name (xN)".
            //   ToString() could emit well-structured plurals and leave conversational formatting up to the client,
            //   or it could have a pluralization format parameter, knowing that reverse-parsing conversational plurals will not work.
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
            return ToString(String.Empty);
        }

        public string ToString(string fmt)
        {
            if (Count <= 0) return String.Empty; 

            StringBuilder sb = new StringBuilder();
            if (Count == 1)
            {
                sb.Append(Item.Name);
            }
            else
            {
                if (fmt == String.Empty)
                {
                    sb.Append(Count.ToString());
                    sb.Append(" ");
                    sb.Append(Item.Plural);
                }
                else if (fmt == "N")
                {
                    sb.Append(Count.ToString());
                    sb.Append(" ");
                    sb.Append(Item.Name);
                }
                else if (fmt == "xN")
                {
                    sb.Append(Item.Name);
                    sb.Append(" (x");
                    sb.Append(Count.ToString());
                    sb.Append(")");
                }
            }

            if (Contents != null)
            {
                sb.Append(" (");
                sb.Append(Contents.ToString());
                sb.Append(")");
            }

            return sb.ToString();
		}

		public IResolver Resolve()
		{
            Logger.Write("Resolve GameObjectInstance: " + Item.Name + "(" + Count + ")");
            return this;
		}

		//public int CompareTo(object other)
		//{
		//	if (other == null)
		//	{
		//		return 1;
		//	}
		//	else
		//	{
		//		return String.Compare(this.ToString(), other.ToString());
		//	}
		//}

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                if (other is GameObjectInstance)
                {
                    GameObjectInstance otherInstance = (GameObjectInstance)other;
                    if (this.Item.Name == otherInstance.Item.Name)
                        return this.Count.CompareTo(otherInstance.Count);
                    else
                        return this.Item.Name.CompareTo(otherInstance.Item.Name);
                }
                else if (other is GameObject)
                {
                    string otherName = other.ToString();
                    if (this.Item.Name == otherName)
                        return this.Count.CompareTo(1);
                    else
                        return this.Item.Name.CompareTo(otherName);

                }
                else
                {
                    return String.Compare(this.ToString(), other.ToString());
                }
            }
        }


    }
}
