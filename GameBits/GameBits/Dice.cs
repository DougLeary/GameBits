using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBits
{
	class Dice
	{
		public class DieRoll
		{
			private int _count;		// number of dice to roll
			private int _sides;		// sides per die
			private int _modifier;	// +/- value added to total rolled

			public static Random random = new Random();

			/// <summary>
			/// Number of dice to roll
			/// </summary>
			public int Count
			{
				get { return _count; }
				set { _count = value; }
			}

			/// <summary>
			/// Number of sides per die
			/// </summary>
			public int Sides
			{
				get { return _sides; }
				set { _sides = value; }
			}

			/// <summary>
			/// +/- Modifier to add to the end result
			/// </summary>
			public int Modifier
			{
				get { return _modifier; }
				set { _modifier = value; }
			}

			/// <summary>
			/// constructor
			/// </summary>
			/// <param name="count"></param>
			/// <param name="sides"></param>
			/// <param name="modifier"></param>
			public DieRoll(int count, int sides, int modifier)
			{
				_count = count;
				_sides = sides;
				_modifier = modifier;
			}

			/// <summary>
			/// Perform a die roll with an instance object
			/// </summary>
			/// <returns></returns>
			public int Roll()
			{
				int total = 0;
				for (int i = 1; i <= Count; i++)
				{
					total += DieRoll.random.Next(1, Sides + 1);
				}
				return total + Modifier;
			}

			/// <summary>
			/// Maximum single result possible
			/// </summary>
			/// <returns></returns>
			public int Maximum()
			{
				return Count * Sides + Modifier;
			}

			/// <summary>
			/// Minimum single result possible
			/// </summary>
			/// <returns></returns>
			public int Minimum()
			{
				return Count + Modifier;
			}

			/// <summary>
			/// Average single result, rounded down
			/// </summary>
			/// <returns></returns>
			public int Average()
			{
				double avg;
				try
				{
					avg = (this.Minimum() + this.Maximum()) / 2;
					return (int)Math.Round(avg);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}

			/// <summary>
			/// Perform a die roll from die roll stats
			/// </summary>
			/// <param name="count"></param>
			/// <param name="sides"></param>
			/// <param name="modifier"></param>
			/// <returns></returns>
			public static int Roll(int count, int sides, int modifier)
			{
				DieRoll dieRoll = new DieRoll(count, sides, modifier);
				return dieRoll.Roll();
			}

			/// <summary>
			/// Perform a die roll from a string such as "3d8+1"
			/// </summary>
			/// <param name="dieRollString"></param>
			/// <returns></returns>
			public static int Roll(string dieRollString)
			{
				DieRoll dieRoll = DieRoll.FromString(dieRollString);
				return dieRoll.Roll();
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				if (Count > 0 && Sides > 0)
				{
					if (Count > 1)
					{
						sb.Append(Count.ToString());
					}
					sb.Append("d");
					sb.Append(Sides.ToString());

					if (Modifier > 0)
					{
						sb.Append("+");
						sb.Append(Modifier.ToString());
					}
					else if (Modifier < 0)
					{
						sb.Append(Modifier.ToString());
					}
				}
				else
				{
					sb.Append(Modifier.ToString());
				}
				return sb.ToString();
			}

			public static DieRoll FromString(string dieRollString)
			{
				int count = 0;
				int sides = 0;
				int modifier = 0;

				///// parse dieRoll

				// strip out blanks
				string st = dieRollString.Replace(" ", String.Empty);

				// get the number of dice
				int d = st.IndexOf("d");
				if (d == 0)
				{
					count = 1;
				}
				else if (d > 0)
				{
					try
					{
						count = Convert.ToInt32(st.Substring(0, d));
					}
					// ignore invalid value; dice will be 0
					catch { }
				}

				// get the modifier by parsing everything after +/-
				char[] signs = { '+', '-' };
				int sign = st.IndexOfAny(signs);
				if (count > 0 && sign < 0)
				{
					modifier = 0;
				}
				else
				{
					// either there is a sign (so there is a modifier) or there is no "d" (so if there is only a fixed value it becomes the modifier)
					try
					{
						modifier = Convert.ToInt32(st.Substring(sign + 1));
						if (sign >= 0 && st.Substring(sign, 1) == "-")
						{
							modifier *= -1;
						}
					}
					// ignore invalid value; modifier will be 0
					catch { }
				}

				// get the number of sides if there are dice (count=0 indicates a fixed value roll)
				if (count > 0)
				{
					// get the sides by parsing everything between "d" and the sign, or everything after "d" if there is no sign;
					if (sign < 0)
					{
						// parse everything after "d"
						try
						{
							sides = Convert.ToInt32(st.Substring(d + 1));
						}
						// ignore invalid value; sides will be 0
						catch { }
					}
					else if (sign > d)
					{
						try
						{
							sides = Convert.ToInt32(st.Substring(d + 1, sign - d - 1));
						}
						// ignore invalid value; sides will be 0
						catch { }
					}
				}

				return new DieRoll(count, sides, modifier);
			}
		}

		public class RollResults : ArrayList
		{
			public enum SortOrder
			{
				Ascending,
				Descending
			}

			public int Total
			{
				get
				{
					int sum = 0;
					for (int i = 0; i < this.Count; i++)
					{
						sum += (int)this[i];
					}
					return sum;
				}
			}

			public RollResults TopResults(int count)
			{
				RollResults list = this.Sorted();
				for (int i = 0; i < count; i++)
				{
					list.RemoveAt(0);
				}
				return list;
			}

			/// <summary>
			/// Highest single result in a series of rolls
			/// </summary>
			/// <returns></returns>
			public int Highest()
			{
				return (int)this.Sorted()[this.Count - 1];
			}

			/// <summary>
			/// Lowest single result in a series of rolls
			/// </summary>
			/// <returns></returns>
			public int Lowest()
			{
				return (int)this.Sorted()[0];
			}

			/// <summary>
			/// Average result in a series of rolls
			/// </summary>
			/// <returns></returns>
			public int Average()
			{
				double avg = 0;
				try
				{
					avg = this.Total / this.Count;
					return (int)Math.Round(avg);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}

			/// <summary>
			/// Results sorted in ascending order
			/// </summary>
			/// <returns></returns>
			public RollResults Sorted()
			{
				RollResults list = new RollResults();
				for (int i = 0; i < this.Count; i++)
				{
					list.Add(this[i]);
				}
				list.Sort();
				return list;
			}

			public RollResults Sorted(SortOrder sortOrder)
			{
				RollResults results = this.Sorted();
				if (sortOrder == SortOrder.Descending)
				{
					results.Reverse();
				}
				return results;
			}

			public override string ToString()
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < this.Count; i++)
				{
					if (sb.Length > 0)
					{
						sb.Append(", ");
					}
					sb.Append(this[i].ToString());
				}
				return sb.ToString();
			}

		}

	}
}
