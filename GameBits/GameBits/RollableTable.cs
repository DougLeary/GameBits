using System;
using System.Collections.Generic;
using System.Data;

namespace GameBits
{
	/// <summary>
	/// Typed DataTable for rolling items off a list
	/// </summary>
	public class RollableTable : DataTable, IResolver
	{
		// maximum rolls attempted, to prevent infinite loops on poorly designed tables
		public static int MaxRollAttempts = 100;

		private DieRoll _dice;
		public DieRoll Dice
		{
			get { return _dice; }
			set { _dice = value; }
		}

		protected override Type GetRowType()
		{
			return typeof(RollableTableRow);
		}

		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new RollableTableRow(builder);
		}

		public static DieRoll DefaultDieRoll()
		{
			// default to d20
			return new DieRoll(1, 20, 0);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public RollableTable()
			: this("Rollable Table")
		{
		}

		public RollableTable(string tableName)
		{
			TableName = tableName;
			Dice = RollableTable.DefaultDieRoll();
			Columns.Add(new DataColumn("LowRoll", typeof(int)));
			Columns.Add(new DataColumn("HighRoll", typeof(int)));
			Columns.Add(new DataColumn("RangeText", typeof(string)));
			Columns.Add(new DataColumn("Item", typeof(IResolver)));
		}

		/// <summary>
		/// Append a row
		/// </summary>
		/// <param name="row"></param>
		public void Add(RollableTableRow row)
		{
			Rows.Add(row);
			SetComputedValues();
		}

		/// <summary>
		/// Append a new row constructed of an item and a highRoll
		/// </summary>
		/// <param name="item"></param>
		/// <param name="highRoll"></param>
		public void Add(IResolver item, int highRoll)
		{
			Add(GetNewRow(item, highRoll));
		}

		public void Add(IResolver item)
		{
			Add(GetNewRow(item));
		}

		public void Add()
		{
			Add(GetNewRow());
		}

		private void InsertAt(RollableTableRow row, int pos)
		{
			Rows.InsertAt(row, pos);
			SetComputedValues();
		}

		public void InsertAt(IResolver item, int highRoll, int pos)
		{
			InsertAt(GetNewRow(item, highRoll), pos);
		}

		public void InsertAt(IResolver item, int pos)
		{
			InsertAt(GetNewRow(item), pos);
		}

		public void InsertAt(int pos)
		{
			InsertAt(GetNewRow(), pos);
		}

		/// <summary>
		/// Create a new row 
		/// </summary>
		/// <param name="item"></param>
		/// <param name="highRoll"></param>
		/// <returns></returns>
		public RollableTableRow GetNewRow(IResolver item, int highRoll)
		{
			RollableTableRow row = (RollableTableRow)NewRow();
			row.LowRoll = highRoll;
			row.HighRoll = highRoll;
			row.Item = item;
			return row;
		}

		/// <summary>
		/// Create a new roll using the default highRoll 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public RollableTableRow GetNewRow(IResolver item)
		{
			return GetNewRow(item, Dice.Maximum);
		}

		public RollableTableRow GetNewRow()
		{
			RollableTableRow row = (RollableTableRow)NewRow();
			return row;
		}

		public void Remove(RollableTableRow row)
		{
			Rows.Remove(row);
			SetComputedValues();
		}

		/// <summary>
		/// Compute LowRoll and RangeText for each row based on that row and the previous row
		/// </summary>
		public void SetComputedValues()
		{
			RollableTableRow prevRow = null;
			foreach (RollableTableRow row in Rows)
			{
				if (prevRow != null)
				{
					row.LowRoll = prevRow.HighRoll + 1;
				}
				else
				{
					row.LowRoll = ((RollableTable)row.Table).Dice.Minimum;
				}

				if (row.LowRoll == row.HighRoll)
				{
					row.RangeText = row.HighRoll.ToString();
				}
				else
				{
					row.RangeText = row.LowRoll.ToString() + "-" + row.HighRoll.ToString();
				}

				prevRow = row;
			}
		}

		// to do: remove parameterless Roll() method and put its functionality in Resolve()
		public IResolver Resolve()
		{
			return Roll();
		}

		public IResolver Roll()
		{
			return Roll(0, int.MaxValue);
		}

		public IResolver Roll(int ignoreBelow, int ignoreAbove)
		{
			int roll = -1;
			int attempts = 0;
			while (attempts <= MaxRollAttempts && (roll < ignoreBelow || roll > ignoreAbove))
			{
				roll = Dice.Roll();
				attempts++;
			}

			// todo: return null if roll is not within ignoreAbove/Below limits

			foreach (DataRow row in Rows)
			{
				if (row == Rows[Rows.Count - 1] || (int)row["HighRoll"] >= roll)
				{
					IResolver result = (IResolver)row["Item"];
					return result.Resolve();
				}
			}
			return null;
		}

		public SortedList<string, int> RollList(int NumberOfRolls)
		{
			SortedList<string, int> list = new SortedList<string, int>();

			if (Rows.Count > 0)		// table can become empty after a session timeout
			{

				// perform a number of rolls on a table and return the end results as a dictionary
				for (int i = 0; i < NumberOfRolls; i++)
				{
					IResolver result = Roll();
					// add result if not on list
					string st = result.ToString();
					if (!list.ContainsKey(st))
					{
						list.Add(st, 0);
					}
					// inc result count
					int count = (int)list[st];
					list[st] = count + 1;
				}
			}

			return list;
		}

		/// <summary>
		/// Indexer
		/// </summary>
		/// <param name="idx"></param>
		/// <returns></returns>
		public RollableTableRow this[int idx]
		{
			get { return (RollableTableRow)Rows[idx]; }
		}


		/// <summary>
		/// Generate a RollableTable from an array of strings
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="dieRoll"></param>
		/// <param name="list"></param>
		/// <returns></returns>
		public static RollableTable FromList(string tableName, DieRoll dieRoll, string[] list)
		{
			if (tableName == null || dieRoll == null || list == null)
			{
				return null;
			}

			//RollableTable table = new RollableTable(tableName);
			//table.Dice = dieRoll;

			//char[] comma = {','};
			//char[] blank = {' '};
			//foreach (string listMember in list)
			//{
			//	RollableTableRow row = table.GetNewRow();
			//}

			RollableTable table;
			if (TryParse(list, out table))
			{
				return table;
			}

			return null;
		}

		public static RollableTable FromList(string tableName, string[] list)
		{
			if (list.Length > 0)
			{
				return FromList(tableName, new DieRoll(1, list.Length, 0), list);
			}
			else
			{
				return null;
			}
		}

		public static RollableTable FromList(string[] list)
		{
			return FromList(list[0], list);
		}

		public int CompareTo(object other)
		{
			return String.Compare(this.ToString(), other.ToString());
		}

		/// <summary>
		/// Convert a string array to a RollableTable
		/// <param name="array"></param>
		/// <param name="table"></param>
		/// <returns>true if success</returns>
		/// </summary>
		public static bool TryParse(string[] array, out RollableTable table)
		{
			// generate a RollableTableRow for each array element
			table = new RollableTable();
			for (int i = 0; i < array.Length; i++)
			{
				RollableTableRow row = table.GetNewRow(ParseItem(array[0]), i + 1);
			}
			return (table.Rows.Count > 0);
		}

		/// <summary>
		/// Convert a text string to an IResolver
		/// </summary>
		/// <param name="itemText"></param>
		/// <returns></returns>
		public static IResolver ParseItem(string itemText)
		{
			char[] comma = { ',' };
			char[] blank = { ' ' };

			// if the item is a comma-separated list, parse it as an ItemList
			string[] items = itemText.Split(comma, StringSplitOptions.RemoveEmptyEntries);
			if (items.Length > 1)
			{
				ItemList itemList = new ItemList();
				foreach (string item in items)
				{
					IResolver obj = ParseItem(item);
					if (obj != null)
					{
						itemList.Add(obj);
					}
				}
				return itemList;
			}

			// parse an individual item
			else if (items.Length == 1)
			{
				DieRoll dieRoll;
				GameObject gameObject;
				string item = items[0];

				// ItemRoll
				string[] tokens = item.Split(blank, StringSplitOptions.RemoveEmptyEntries);
				if (DieRoll.TryParse(tokens[0], out dieRoll))
				{
					string theRest = item.Substring(tokens[0].Length).Trim();
					if (GameObject.TryParse(theRest, out gameObject))
					{
						return new ItemRoll(gameObject, dieRoll);
					}
				}

				// GameObjectInstance
				GameObjectInstance gameObjectInstance;
				if (GameObjectInstance.TryParse(item, out gameObjectInstance))
				{
					return gameObjectInstance;
				}

				// TableRoll
				if (item.ToLower().StartsWith("roll"))
				{
					TableRoll tableRoll;
					if (TableRoll.TryParse(item.Remove(0, 5), out tableRoll))
					{
						return (tableRoll);
					}
				}

				// GameObject
				if (GameObject.TryParse(item, out gameObject))
				{
					return gameObject;
				}
			}
			return null;
		}
	}

}
