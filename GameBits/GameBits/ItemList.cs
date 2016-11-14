using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	/// <summary>
	/// List of IResolvers that can be treated as a single item, for example as an entry in a RollableTable
	/// </summary>
	public class ItemList : List<IResolver>, IResolver
	{
		public static string Separator = "; ";

		/// <summary>
		/// Item display format (not implemented yet); Compressed means combine like items into one item with a count
		/// </summary>
		public enum Format
		{
			// TODO: implement this and also implement a way to format a single item as "Item" or "1 Item";
			// Format should be a class instead of an enum, with properties Compression, SingleItem, and maybe others. 
			Uncompressed,
			Compressed
		}

		/// <summary>
		/// An ItemList resolves to a list of its own contents individually resolved
		/// </summary>
		/// <returns></returns>
		public IResolver Resolve()
		{
			ItemList resolvedList = new ItemList();
			foreach (IResolver item in this)
			{
				IResolver resolvedItem = item.Resolve();
				if (resolvedItem != null)
				{
					resolvedList.Add(resolvedItem);
				}
			}
			return resolvedList;
		}

		public override string ToString()
		{
			// by default return the string in compressed format; { zombie, zombie, zombie, ghost } = "3 zombie, 1 ghost"
			return this.ToString(Format.Compressed);
		}

		public string ToString(Format format)
		{
			SortedList<IResolver, int> list = new SortedList<IResolver,int>();
			return ToString(format, list);
		}

		private string ToString(Format format, SortedList<IResolver, int> list)
		{
			// TODO: implement Compressed format

			StringBuilder sb = new StringBuilder();

			foreach (IResolver item in this)
			{
				if (!list.ContainsKey(item))
				{
					list.Add(item, 1);
				}
				else
				{
					list[item]++;
				}
			}

			foreach (IResolver key in list.Keys)
			{
				int count = list[key];
				if (count > 1)
				{
					sb.Append(count.ToString());
					sb.Append(" ");
					if (key.GetType() == typeof(GameObject))
					{
						sb.Append(((GameObject)key).Plural);
					}
					else if (key.GetType() == typeof(GameObjectInstance))
					{
						sb.Append(((GameObjectInstance)key).Item.Plural);
					}

					else
					{
						sb.Append(key.ToString());
					}
				}
				else
				{
					sb.Append(key.ToString());
				}
				sb.Append(ItemList.Separator);
			}

			if (sb.Length > 2)
			{
				return sb.ToString(0,sb.Length-2);
			}
			else
			{
				return sb.ToString();
			}
		}

		public int CompareTo(object other)
		{
			return String.Compare(this.ToString(), other.ToString());
		}

	}
}
