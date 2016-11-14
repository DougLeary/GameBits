using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace GameBits
{
	/// <summary>
	/// An instruction to make a roll on a specific table
	/// </summary>
	public class TableRoll : IResolver
	{
		// table to roll on
		private RollableTable _table;
		public RollableTable Table
		{
			get { return _table; }
			set { _table = value; }
		}

		// number of times to roll
		public int Rolls = 1;

		// ignore values less than
		public int IgnoreBelow = 0;

		// ignore values greater than
		public int IgnoreAbove = 0;

		/// <summary>
		/// Instruction to make a roll against a table 
		/// </summary>
		/// <param name="name"></param>
		public TableRoll(RollableTable table)
		{
			Table = table;
			Rolls = 1;
			IgnoreBelow = 0;
			IgnoreAbove = int.MaxValue;
		}

		// for serialization only
		public TableRoll()
			: this(null)
		{
		}

		public IResolver Resolve()
		{
			if (Rolls > 1)
			{
				ItemList list = new ItemList();
				for (int i = 0; i < Rolls; i++)
				{
					list.Add(Table.Roll(IgnoreBelow, IgnoreAbove).Resolve());
				}
				return list;
			}
			else
			{
				IResolver ir = Table.Roll(IgnoreBelow, IgnoreAbove).Resolve();
				return Table.Roll(IgnoreBelow, IgnoreAbove).Resolve();
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Table == null)
			{
				return base.ToString();
			}
			else
			{
				// show number of rolls
				sb.Append("Roll");
				if (Rolls == 2)
				{
					sb.Append(" twice");
				}
				else if (Rolls > 2)
				{
					sb.Append(" " + Rolls.ToString() + " times");
				}
				sb.Append(" on " + Table.TableName);

				// show results to ignore
				if (IgnoreBelow > 0 || IgnoreAbove < int.MaxValue)
				{
					sb.Append(", ignoring results");
					if (IgnoreBelow > 0)
					{
						sb.Append(" below " + IgnoreBelow.ToString());
						if (IgnoreAbove < int.MaxValue)
						{
							sb.Append(" or");
						}
					}
					if (IgnoreAbove < int.MaxValue)
					{
						sb.Append(" above " + IgnoreAbove.ToString());
					}
				}
			}

			return sb.ToString();
		}

		public int CompareTo(object other)
		{
			return String.Compare(this.ToString(), other.ToString());
		}

		public static bool TryParse(string st, out TableRoll outTableRoll)
		{
			outTableRoll = new TableRoll();

			char[] blank = { ' ' };
			string[] words = st.ToLower().Split(blank, StringSplitOptions.RemoveEmptyEntries);
			int i = 0;
			int rolls = 1;
			DieRoll dieRoll = null;

			if (words[i] == "once") 
			{ 
				i++; 
			}
			else if (words[i] == "twice")
			{
				i++;
				rolls = 2;
			}
			else if (int.TryParse(words[i], out rolls))
			{
				i++;
			}
			else if (DieRoll.TryParse(words[i], out dieRoll))
			{
				i++;
			}
			if (words[i] == "times") { i++; }

			if (words[i] == "on") { i++; }

			// get the RollableTable name enclosed in double quotes
			string remainder = String.Join(" ", words, words.Length - i);
			Regex regex = new Regex("\"([^\"]*)\"");
			Match match = regex.Match(remainder);
			if (match.Success)
			{
				int ignoreBelow = 0;
				int ignoreAbove = int.MaxValue;

				// scan for ignore options
				remainder.Remove(0, match.Length);
				words = remainder.Split(blank, StringSplitOptions.RemoveEmptyEntries);
				i = 0;
				if (words[i] == "ignoring" && words[i] == "ignore")
				{
					i++;
					if (words[i] == "results") { i++; }
					switch (words[i])
					{
						case "above" :
						case ">" :
						case "greater" :
							i++;
							if (words[i] == "than") { i++; }
							if (!int.TryParse(words[i], out ignoreAbove))
							{
								return false;
							}
							break;
						case "below" :
						case "<" :
						case "less" :
							i++;
							if (words[i] == "than") { i++; }
							if (!int.TryParse(words[i], out ignoreBelow))
							{
								return false;
							}
							break;
					}
				}
				outTableRoll.Table = Repository.GetTable(match.Value);
				outTableRoll.Rolls = rolls;
				outTableRoll.IgnoreAbove = ignoreAbove;
				outTableRoll.IgnoreBelow = ignoreBelow;
			}

			return true;
		}

	}
}
