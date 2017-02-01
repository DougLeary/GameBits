using System;
using System.Collections;
using System.Text;

namespace GameBits
{
    /// <summary>
    /// List of integer results of a series of one or more die rolls
    /// </summary>
	public class DieRollResults : ArrayList
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

		/// <summary>
		/// Highest single result value rolled
		/// </summary>
		/// <returns></returns>
		public int Highest
		{
			get
			{
				return (int)this.Sorted()[this.Count - 1];
			}
		}

		/// <summary>
		/// Lowest single result value rolled
		/// </summary>
		/// <returns></returns>
		public int Lowest
		{
			get
			{
				return (int)this.Sorted()[0];
			}
		}

		/// <summary>
		/// Average result value rolled, rounded to nearest integer
		/// </summary>
		/// <returns></returns>
		public int Average
		{
			get
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
		}

        /// <summary>
        /// Keep only a subset of highest result values
        /// </summary>
        /// <param name="KeepHowMany">Number of values to keep</param>
        /// <returns></returns>
        public void KeepBest(int KeepHowMany)
		{
			// make sure we keep at least 1 and not more than we rolled
			int keep = Math.Max(KeepHowMany, 1);
			keep = Math.Min(Count, keep);

			// sort the results from lowest to highest
			this.Sort();

			// throw away the lowest ones
			int removeCount = this.Count - keep;
			for (int i = 0; i < removeCount; i++)
			{
				this.RemoveAt(0);
			}
		}

		/// <summary>
		/// Retgurns the results sorted in ascending order
		/// </summary>
		/// <returns></returns>
		public DieRollResults Sorted()
		{
			return this.Sorted(SortOrder.Ascending);
		}

		/// <summary>
		/// Results sorted in ascending or descending order
		/// </summary>
		/// <param name="sortOrder"></param>
		/// <returns></returns>
		public DieRollResults Sorted(SortOrder sortOrder)
		{
			DieRollResults list = new DieRollResults();
			for (int i = 0; i < this.Count; i++)
			{
				list.Add(this[i]);
			}
			list.Sort();
			if (sortOrder == SortOrder.Descending)
			{
				list.Reverse();
			}
			return list;
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
