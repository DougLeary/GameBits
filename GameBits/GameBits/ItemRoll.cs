using System;
using System.Text;
using System.Xml;

namespace GameBits
{
    /// <summary>
    /// Roll a rollable item a random number of times as determined by die roll.
    /// </summary>
	public class ItemRoll : IResolver
	{
		/// <summary>
		/// IResolver object to be rolled up
		/// </summary>
		public IResolver Item { get; set; }
	
		/// <summary>
		/// Die roll to determine the number of results
		/// </summary>
		public DieRoll Dice { get; set; }

		/// <summary>
		/// Multiplies die roll result to arrive at item count, as in 1d4 x 1000 gp. Default=1.
		/// </summary>
		public int Multiplier { get; set; }

		/// <summary>
		/// Percentage chance that this ItemRoll should be performed (default=100)
		/// </summary>
		public int Percent { get; set; }

        /// <summary>
        /// Used for TableRolls only; if true (default) 4d10 means do 4 TableRolls and multiply each result by d10, instead of doing 4d10 TableRolls. 
        /// </summary>
        public bool IsGrouped { get; set; }
        
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
            IsGrouped = true;
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
                GameObjectInstance item = new GameObjectInstance((GameObject)Item, Dice.Roll() * Multiplier);
                Logger.Write("... rolled " + item.ToString());
                return item;
			}
			else if (Item is TableRoll && IsGrouped)
            {
                // Number of dice in Dice determines number of TableRolls performed. 
                // Dice rolled with a single die determines number of occurrences of each TableRoll results.
                // e.g. if Dice = 4d10+2, 4 TableRolls are performed, and d10+2 is rolled to determine number of occurrences of each result. 

                TableRoll t = (TableRoll)Item;
                Logger.Write("... performing " + Dice.Dice.ToString() + " rolls on " + t.Table.TableName);

                ItemList list = new ItemList();
                DieRoll singleRoll = new DieRoll(1, Dice.Sides, Dice.Modifier);
                for (int i = 0; i < Dice.Dice; i++)
                {
                    IResolver item = Item.Resolve();
                    if (item != null)
                    {
                        int count = singleRoll.Roll();
                        Logger.Write("... occurring " + count + " times");
                        for (int j = 0; j < count; j++)
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
            else 
			{
				ItemList list = new ItemList();
				int count = Dice.Roll() * Multiplier;

                Logger.Write("... rolled " + Dice.ToString() + ": " + count.ToString());

                for (int i = 0; i < count; i++)
				{
					list.Add(Item.Resolve());
				}
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
            Logger.Write("Resolve ItemRoll: " + Item.ToString() + ", " + Dice.ToString() + " " + " x" + Multiplier + " (" + Percent + "%)");

            DieRoll percentRoll = new DieRoll(1, 100, 0);
            int percent = percentRoll.Roll();
            Logger.Write("... rolled " + percentRoll.ToString() + ": " + percent.ToString() + "%" + ((percent <= Percent) ? "" : " NO ROLL"));

            if (percent <= Percent)
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
