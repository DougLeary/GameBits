using System;
using System.Text;
using System.Xml;

namespace GameBits
{
    /// <summary>
    /// Specification to generate an IResolver using a die roll
    /// </summary>
	public class ItemRoll : IResolver
	{
		/// <summary>
		/// IResolver object to be rolled up
		/// </summary>
		public IResolver Item { get; set; }
	
		/// <summary>
		/// Die roll to determine the number of result items
		/// </summary>
		public DieRoll Dice { get; set; }

		/// <summary>
		/// Multiplies die roll result to arrive at item count, as in 1d4 x 1000 gp
		/// </summary>
		public int Multiplier { get; set; }

		/// <summary>
		/// Percentage chance that this ItemRoll should be performed (default=100)
		/// </summary>
		public int Percent { get; set; }

        /// <summary>
        /// A d20 DieRoll used if no Dice property is assigned
        /// </summary>
        /// <returns></returns>
		public static DieRoll DefaultDieRoll()
		{
			// default to d20
			return new DieRoll(1, 20, 0);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="item"></param>
		/// <param name="dice"></param>
		/// <param name="multiplier"></param>
		/// <param name="percent"></param>
		public ItemRoll(IResolver item, DieRoll dice, int multiplier, int percent)
		{
			Item = item;
			Dice = dice;
			Multiplier = multiplier;
			Percent = percent;
		}

		public ItemRoll(IResolver item, DieRoll dice, int multiplier)
			: this(item, dice, multiplier, 100)
		{
		}

		public ItemRoll(IResolver item, DieRoll dice)
			: this(item, dice, 1)
		{
		}

		public ItemRoll(IResolver item)
			: this(item, ItemRoll.DefaultDieRoll())
		{
		}

		// for serialization only
		public ItemRoll()
//			: this(new GameObject())
		{
		}

		/// <summary>
		/// Performs a die roll to generate one or more IResolvers from Item
		/// </summary>
		/// <returns></returns>
		public IResolver Roll()
		{
			//GameObjectInstance result = new GameObjectInstance(Item, Dice.Roll() * Multiplier);
			if (Item is GameObject)
			{ 
				return new GameObjectInstance((GameObject)Item, Dice.Roll() * Multiplier);
			}
			else
			{
				ItemList list = new ItemList();
				int count = Dice.Roll();
				for (int i = 0; i < count; i++)
				{
					list.Add(Item.Resolve());
				}
                // note - we ignore the Multiplier here because only a single result item should have a multiplier;
                // if Item is a TableRoll we roll n times on it, where n = Dice.Roll()
                return list;
			}
		}

		/// <summary>
		/// Resolves the ItemRoll to a GameObject by performing a die roll
		/// </summary>
		/// <returns></returns>
		public IResolver Resolve()
		{
            string name = String.Empty;
            if (Item is GameObject) { name = ((GameObject)Item).Name; }
            else if (Item is GameObjectInstance) { name = ((GameObjectInstance)Item).Item.Name; }
            Logger.Write("Resolve ItemRoll: " + name + ", " + Dice.ToString() + " " + " x" + Multiplier + " (" + Percent + "%)");

            DieRoll pct = new DieRoll(1, 100, 0);
			if (pct.Roll() <= Percent)
			{
				return Roll();
			}
			else
			{
				return null;
			}
		}

		public int CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			else
			{
				return String.Compare(Item.ToString(), ((ItemRoll)other).Item.ToString());
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(Dice.ToString());
			if (Multiplier > 1)
			{
				sb.Append(" * ");
				sb.Append(Multiplier);
			}
			sb.Append(" ");
			if (Item is GameObject)
			{
				sb.Append(((GameObject)Item).Plural.ToString());
			}
			else
			{
				sb.Append(Item.ToString());
			}
			if (Percent != 100)
			{
				sb.Append(" (");
				sb.Append(Percent.ToString());
				sb.Append("%)");
			}
			return sb.ToString();
		}

		public static bool TryParse(string st, out ItemRoll itemRoll)
		{
			GameObject obj;
			if (GameObject.TryParse(st, out obj))
			{
				itemRoll = new ItemRoll(obj, new DieRoll(1, 6, 0));
				return true;
			}
			itemRoll = null;
			return false;
		}

	}
}
