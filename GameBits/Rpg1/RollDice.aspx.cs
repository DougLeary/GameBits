﻿using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using GameBits;

namespace RealmSmith
{
	public partial class RollDice : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void ButtonRoll_Click(object sender, EventArgs e)
		{
			DieRoll dr = DieRoll.FromString(TextDieRoll.Text);

			int rollCount;
			try
			{
				rollCount = Convert.ToInt32(TextRollCount.Text);
			}
			catch (Exception ex)
			{
				rollCount = 1;
			}

			if (rollCount > 0)
			{
				DieRollResults results = Roll(dr, rollCount);

				StringBuilder sb = new StringBuilder();
				sb.Append("Minimum, Maximum, Average for ");
				sb.Append(dr.ToString());
				sb.Append(": ");
				sb.Append(dr.Minimum.ToString());
				sb.Append(", ");
				sb.Append(dr.Maximum.ToString());
				sb.Append(", ");
				sb.Append(dr.Average.ToString());
				sb.Append("<br />Roll results: ");
				sb.Append(results.ToString());
				sb.Append("<br />Sorted: ");
				sb.Append(results.Sorted().ToString());
				sb.Append("<br />Reversed: ");
				sb.Append(results.Sorted(DieRollResults.SortOrder.Descending).ToString());
				sb.Append("<br />Lowest, Highest, Average rolled: ");
				sb.Append(results.Lowest.ToString());
				sb.Append(", ");
				sb.Append(results.Highest.ToString());
				sb.Append(", ");
				sb.Append(results.Average.ToString());

				if (rollCount > 3)
				{
					results.KeepBest(3);
					sb.Append("<br />Top 3 rolls: ");
					sb.Append(results.ToString());
					sb.Append(", total ");
					sb.Append(results.Total.ToString());
				}
				LabelResult.Text = sb.ToString();
			}
			else
			{
				LabelResult.Text = "Number of Rolls must be a positive integer.";
			}

		}

		private DieRollResults Roll(DieRoll dr, int count)
		{
			DieRollResults rolls = new DieRollResults();
			for (int i = 1; i <= count; i++)
			{
				rolls.Add(dr.Roll());
			}
			return rolls;
		}

	}
}
