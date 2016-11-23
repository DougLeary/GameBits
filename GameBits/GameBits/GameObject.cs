using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
    /// <summary>
    /// Base class for an in-game entity, corresponding to a definition of an object such as a creature or magic spell in a game manual,
    /// used to generate instances of that object.  
    /// </summary>
	public class GameObject : IResolver
	{
		private string _name;
		private string _plural;
		private string _description;
        private ItemList _contents;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Plural
		{
			get { return _plural; }
			set { _plural = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

        public ItemList Contents
        {
            get { return _contents;  }
            set { _contents = value; }
        }

		public GameObject(string name, string plural)
		{
			Name = name;
			Plural = plural;
			Description = name;
            Contents = null;
		}

		public GameObject(string name)
			: this(name, DefaultPlural(name))
		{
		}

		public static bool TryParse(string name, out GameObject gameObject)
		{
			gameObject = new GameObject(name);
			return true;
		}

		public static string DefaultPlural(string name)
		{
			// default pluralization
			if (name.Contains(" of "))
			{
				return name.Replace(" of ", "s of ");
			}
			if (name.Contains(" +"))
			{
				return name.Replace(" +", "s +");
			}
			if (name.EndsWith("y") && !name.EndsWith("ey"))
			{
				return name.Substring(0, name.Length - 1) + "ies";
			}
			else if (name.EndsWith("s"))
			{
				return name.Substring(0, name.Length - 1) + "ses";
			}
			else
			{
				return name + "s";
			}
		}


		/// <summary>
		/// Default constructor, used internally when a new RollableTable row is created
		/// </summary>
		public GameObject()
			: this(String.Empty)
		{
		}

		public override string ToString()
		{
			return Name;
		}

		public IResolver Resolve()
		{
			GameObjectInstance instance = new GameObjectInstance(this, 1);
            if (this.Contents == null)
            {
                instance.Contents = null;
            }
            else
            {
                instance.Contents = new ItemList();
                instance.Contents = (ItemList)(this.Contents.Resolve());
            }
            return instance;
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
