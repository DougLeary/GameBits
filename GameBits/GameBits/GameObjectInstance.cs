using System;
using System.Text;

namespace GameBits
{
    /// <summary>
    /// Generated instance of a GameObject, and the number of individuals occurring. 
    /// </summary>
	public class GameObjectInstance : ResolverInstance
	{
        private ItemList _contents;

        /// <summary>
        /// GameObject from which the instance was generated
        /// </summary>
		public GameObject Item
		{
			get { return (GameObject)base.Item; }
			set { base.Item = value; }
		}

        public ItemList Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

		public GameObjectInstance(GameObject item, int count)
            : base(item, count)
		{
            Contents = null;
		}

		public GameObjectInstance(GameObject item)
			: this(item, 1)
		{
		}

        // parameterless constructor for serialization only
        public GameObjectInstance()
            : this(new GameObject(), 1)
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

        public string ToString(string format)
        {
            if (Count <= 0) return String.Empty;

            string name = Item.ToString();

            StringBuilder sb = new StringBuilder();
            if (Count == 1)
            {
                sb.Append(name);
            }
            else
            {
                if (format == String.Empty)
                {
                    sb.Append(Count.ToString());
                    sb.Append(" ");
                    sb.Append(Item.Plural);
                }
                else if (format == "N")
                {
                    sb.Append(Count.ToString());
                    sb.Append(" ");
                    sb.Append(name);
                }
                else if (format == "xN")
                {
                    sb.Append(name);
                    sb.Append(" (x");
                    sb.Append(Count.ToString());
                    sb.Append(")");
                }
            }

            // make a show-contents format
            //if (Contents != null)
            //{
            //    sb.Append(" (");
            //    sb.Append(Contents.ToString());
            //    sb.Append(")");
            //}

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
