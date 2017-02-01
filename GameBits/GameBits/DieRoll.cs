using System;
using System.Collections.Generic;
using System.Text;

namespace GameBits
{
	/// <summary>
	/// Die Roll Class
	/// </summary>
	public class DieRoll
	{
		private int _dice;		// number of dice to roll
		private int _sides;		// sides per die
		private int _modifier;	// +/- value added to total rolled
		private int _keep;		// number of best dice to keep

		private static Random random = new Random();

		#region Properties
		/// <summary>
		/// Number of dice to roll
		/// </summary>
		public int Dice
		{
			get { return _dice; }
			set { _dice = value; }
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
		/// Number of best dice to keep
		/// </summary>
		public int Keep
		{
			get { return _keep; }
			set { _keep = value; }
		}

		/// <summary>
		/// Maximum single roll result possible
		/// </summary>
		/// <returns></returns>
		public int Maximum
		{
			get
			{
				return Keep * Sides + Modifier;
			}
		}

		/// <summary>
		/// Minimum single roll result possible
		/// </summary>
		/// <returns></returns>
		public int Minimum
		{
			get
			{
				return Keep + Modifier;
			}
		}

		/// <summary>
		/// Average single result, rounded down
		/// </summary>
		/// <returns></returns>
		public int Average
		{
			get
			{
				double avg;
				try
				{
					avg = (this.Minimum + this.Maximum) / 2;
					return (int)Math.Round(avg);
				}
				catch (Exception ex)
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Full range of possible roll results
		/// </summary>
		public string RangeText
		{
			get
			{
				if (Minimum == Maximum)
				{
					return Maximum.ToString();
				}
				else
				{
					return Minimum.ToString() + "-" + Maximum.ToString();
				}
			}
		}

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Generate a new DieRoll object from a string of the form '4d8+1:3'
		/// </summary>
		/// <param name="dieRollString"></param>
		/// <returns></returns>
		public DieRoll(string dieRollString)
		{

			_dice = 0;
			_sides = 0;
			_modifier = 0;
			_keep = 0;

			///// parse dieRoll

			// strip out blanks
			string st = dieRollString.Replace(" ", String.Empty);

			// get the number of dice
			int d = st.IndexOf("d");
			if (d == 0)
			{
				_dice = 1;
			}
			else if (d > 0)
			{
				try
				{
					_dice = Convert.ToInt32(st.Substring(0, d));
				}
				// ignore invalid value; dice will be 0
				catch { }
			}

			// parse and strip off the keep value if present
			int iKeep = st.IndexOf(":");
			if (iKeep >= 0)
			{
				try
				{
					_keep = Convert.ToInt32(st.Substring(iKeep + 1));
				}
				// ignore invalid value
				catch { }
				finally
				{
					st = st.Remove(iKeep);
				}
			}
			// default keep to # of dice if not explicitly and correctly set
			if (_keep == 0)
			{
				_keep = _dice;
			}

			// get the modifier by parsing everything after +/-
			char[] signs = { '+', '-' };
			int sign = st.IndexOfAny(signs);
			if (_dice > 0 && sign < 0)
			{
				_modifier = 0;
			}
			else
			{
				// either there is a sign (so there is a modifier) or there is no "d" (so if there is only a fixed value it becomes the modifier)
				try
				{
					_modifier = Convert.ToInt32(st.Substring(sign + 1));
					if (sign >= 0 && st.Substring(sign, 1) == "-")
					{
						_modifier *= -1;
					}
				}
				// ignore invalid value; modifier will be 0
				catch { }
			}

			// get the number of sides if there are dice (dice=0 indicates a fixed value roll)
			if (_dice > 0)
			{
				// get the sides by parsing everything between "d" and the sign, or everything after "d" if there is no sign;
				if (sign < 0)
				{
					// parse everything after "d"
					try
					{
						_sides = Convert.ToInt32(st.Substring(d + 1));
					}
					// ignore invalid value; sides will be 0
					catch { }
				}
				else if (sign > d)
				{
					try
					{
						_sides = Convert.ToInt32(st.Substring(d + 1, sign - d - 1));
					}
					// ignore invalid value; sides will be 0
					catch { }
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dice"></param>
		/// <param name="sides"></param>
		/// <param name="modifier"></param>
		/// <param name="keep"></param>
		public DieRoll(int dice, int sides, int modifier, int keep)
		{
			_dice = dice;
			_sides = sides;
			_modifier = modifier;
			_keep = keep;
		}

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="dice"></param>
		/// <param name="sides"></param>
		/// <param name="modifier"></param>
		public DieRoll(int dice, int sides, int modifier) : this(dice, sides, modifier, dice)
		{
		}

		/// <summary>
		/// for serialization only
		/// </summary>
		public DieRoll()
			: this(1, 20, 0)
		{
		}

		#endregion Constructors

		#region Methods
		/// <summary>
		/// Perform a die roll
		/// </summary>
		/// <returns></returns>
		public int Roll()
		{
			List<int> results = new List<int>();
            if (Dice == 0 || Sides == 0) return Math.Max(0, Modifier);

            int total = 0;
            for (int i = 0; i < Dice; i++)
			{
				results.Add(DieRoll.random.Next(1, Sides + 1));
			}
			results.Sort();

			for (int i = Dice-1; i >= Dice - Keep; i--)
			{
				total += results[i];
			}

			return total + Modifier;
		}

		/// <summary>
		/// Perform a series of die rolls and keep the best results
		/// </summary>
		/// <param name="NumberOfRolls"></param>
		/// <param name="KeepHowMany"></param>
		/// <returns></returns>
		public DieRollResults MultiRoll(int NumberOfRolls, int KeepHowMany)
		{
			// make the required number of rolls
			DieRollResults rolls = new DieRollResults();
			for (int i = 0; i < NumberOfRolls; i++)
			{
				rolls.Add(this.Roll());
			}

			// keep at least 1 and not more than we rolled
			rolls.KeepBest(Math.Min(Math.Max(KeepHowMany, 1), NumberOfRolls));
			return rolls;
		}

		/// <summary>
		/// Perform a series of die rolls and keep all results
		/// </summary>
		/// <param name="NumberOfRolls"></param>
		/// <returns></returns>
		public DieRollResults MultiRoll(int NumberOfRolls)
		{
			return MultiRoll(NumberOfRolls, NumberOfRolls);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Dice > 0 && Sides > 0)
			{
				if (Dice > 1)
				{
					sb.Append(Dice.ToString());
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

				if (Keep > 0 && Keep < Dice)
				{
					sb.Append(":");
					sb.Append(Keep.ToString());
				}
			}
			else
			{
				sb.Append(Modifier.ToString());
			}
			return sb.ToString();
		}
		#endregion

		#region Static Methods

		/// <summary>
		/// Attempt to parse a die roll string, returning true if successful.
		/// </summary>
		/// <param name="dieRollString"></param>
		/// <param name="dieRoll"></param>
		/// <returns></returns>
		public static bool TryParse(string dieRollString, out DieRoll dieRoll)
		{
			// 10/19/2011 - ideas for new ultimate syntax: 
			//	3/4d6+1:8/10 = roll 4d6, keep best 3, add 1; do this 10 times and return best 8.

			// At least one of [dice, sides or modifier] must be a positive integer.
			// If dice=0 or sides=0 then the modifier must be a positive int; in that case Roll will always return the modifier as the result. 
			// If keep is used it must be greater than 0. 

			int dice = 0;
			int sides = 0;
			int modifier = 0;
			int keep = 0;

			dieRoll = null;

			// strip out blanks
			string st = dieRollString.Replace(" ", String.Empty).ToLower();

			// a dieRoll can be a numeric constant - in this case make it the modifier and we're done
			if (int.TryParse(st, out modifier))
			{
				dieRoll = new DieRoll(dice, sides, modifier, keep);
				return (modifier > 0);
			}

			// get the number of dice
			int dPos = st.IndexOf("d");
			if (dPos < 0)
			{
				return false;
			}
			else
			{
				if (!int.TryParse(st.Substring(0, dPos), out dice))
				{
					dice = 1;
				}
			}

			// parse and strip off the keep value if present
			int iKeep = st.IndexOf(":");
			if (iKeep >= 0)
			{
				if (!int.TryParse(st.Substring(iKeep + 1), out keep))
				{
					return false;
				}
				st = st.Remove(iKeep);
			}

			// default keep to # of dice if not explicitly and correctly set
			if (keep == 0)
			{
				keep = dice;
			}

			// get the modifier by parsing everything after +/-
			char[] signs = { '+', '-' };
			int sign = st.IndexOfAny(signs);
			if (sign < 0)
			{
				modifier = 0;
			}
			else
			{
				// either there is a sign (so there is a modifier) or there is no "d" (so if there is only a fixed value it becomes the modifier)
				if (!int.TryParse(st.Substring(sign), out modifier))
				{
					return false;
				}
			}

			// get the number of sides if there are dice (dice=0 indicates a fixed value roll)
			if (dice > 0)
			{
				// get the sides by parsing everything between "d" and the sign, or everything after "d" if there is no sign;
				if (sign < 0)
				{
					// parse everything after "d"
					if (!int.TryParse(st.Substring(dPos + 1), out sides))
					{
						return false;
					}
				}
				else if (sign > dPos)
				{
					if (!int.TryParse(st.Substring(dPos + 1, sign - dPos - 1), out sides))
					{
						return false;
					}
				}
			}

			dieRoll = new DieRoll(dice, sides, modifier, keep);
			return true;
		}


		/// <summary>
		/// Generate a new DieRoll object from a string
		/// </summary>
		/// <param name="dieRollString"></param>
		/// <returns></returns>
		public static DieRoll FromString(string dieRollString)
		{
			DieRoll d;
			if (TryParse(dieRollString, out d))
				return d;
			else
				throw new System.ArgumentException("Invalid DieRoll string", "dieRollString");
		}

		/// <summary>
		/// Perform a die roll from die roll stats
		/// </summary>
		/// <param name="dice"></param>
		/// <param name="sides"></param>
		/// <param name="modifier"></param>
		/// <param name="keep"></param>
		/// <returns></returns>
		public static int Roll(int dice, int sides, int modifier, int keep)
		{
			DieRoll dr = new DieRoll(dice, sides, modifier);
			dr.Keep = keep;
			return dr.Roll();
		}

		public static int Roll(int dice, int sides, int modifier)
		{
			return Roll(dice, sides, modifier, dice);
		}

		public static int Roll(int dice, int sides)
		{
			return Roll(dice, sides, 0, dice);
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
		#endregion Static Methods


	}
}
