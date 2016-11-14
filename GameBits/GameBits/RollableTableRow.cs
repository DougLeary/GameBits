using System;
using System.Data;
using System.Linq;
using System.Text;

namespace GameBits
{
	/// <summary>
	/// One row of a RollableTable
	/// </summary>
	public class RollableTableRow : DataRow
	{
		public virtual RollableTable Table
		{
			get { return (RollableTable)base.Table; }
		}

		internal RollableTableRow(DataRowBuilder builder)
			: base(builder)
		{
			LowRoll = 1;
			HighRoll = Table.Dice.Maximum;
			Item = null;
		}

		public int Normalize(int rollValue)
		{
			if (rollValue < Table.Dice.Minimum)
			{
				return Table.Dice.Minimum;
			}
			else if (rollValue > Table.Dice.Maximum)
			{
				return Table.Dice.Maximum;
			}
			else
			{
				return rollValue;
			}
		}

		public int LowRoll
		{
			get { return (int)base["LowRoll"]; }
			set { base["LowRoll"] = Normalize(value); }
		}

		public int HighRoll
		{
			get { return (int)base["HighRoll"]; }
			set { base["HighRoll"] = Normalize(value); }
		}

		public string RangeText
		{
			get
			{
				if (LowRoll == HighRoll)
				{
					return HighRoll.ToString();
				}
				else
				{
					return LowRoll.ToString() + "-" + HighRoll.ToString();
				}
			}
			set { base["RangeText"] = value; }
		}

		public IResolver Item
		{
			get { return (IResolver)base["Item"]; }
			set { base["Item"] = value; }
		}
	}

}
