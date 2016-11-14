using System;
using System.Text;
using System.Collections;

public class Hitachi
{
	public Hitachi()
	{
	}

	/// <summary>
	/// Returns a list of all alpha characters in the source string.
	/// </summary>
	/// <param name="SourceString"></param>
	/// <returns></returns>
	public static SortedList CountAllAlpha(string SourceString)
	{
		// Convert the source string to all uppercase.
		// Repeat the following until the string is empty:
		//	Get the length of the string.
		//	Replace all occurrences of the first character with nothing.
		//	Use the new length of the string to determine how many occurrences there were.
		//	Save the character and count in a SortedList.
		// Return the sorted list.
		SortedList list = new SortedList();
		string st = SourceString.ToUpper();
		int oldLength = st.Length;
		while (oldLength > 0)
		{
			string charToCount = st.Substring(0, 1);
			st = st.Replace(charToCount, String.Empty);
			int newLength = st.Length;
			int count = oldLength - newLength;
			list.Add(charToCount, count);
			oldLength = newLength;
		}
		return list;
	}

}
