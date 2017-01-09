using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
			// Format should be a static class instead of an enum, with properties Compression, SingleItem, and maybe others. 
			Uncompressed,
			Compressed
		}

		/// <summary>
		/// An ItemList resolves to an ItemList of resolved items
		/// </summary>
		/// <returns></returns>
		public IResolver Resolve()
		{
            Logger.Write("Resolve ItemList " + this.ToString());
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
            // make an empty output list for the compressed version
			SortedList<IResolver, int> list = new SortedList<IResolver,int>();
			return ToString(format, list);
		}

		private string ToString(Format format, SortedList<IResolver, int> list)
		{
            // load the supplied (presumed empty) SortedList with the sorted content of ItemList.
            // Each ItemList item is added to the SortedList once.
            // In each SortedList item the IResolver is the ItemList item and the int is the number of times it occurs in the ItemList.
			// TODO: implement Uncompressed format (sorted, but with all items included and each count = 1). 
			StringBuilder sb = new StringBuilder();

            // add each ItemList item to list or increment existing item's count
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
                        Logger.Write("ToString ItemList item GameObject: " + ((GameObject)key).Name);
                        sb.Append(((GameObject)key).Plural);
					}
					else if (key.GetType() == typeof(GameObjectInstance))
					{
                        Logger.Write("ToString ItemList item GameObjectInstance: " + ((GameObjectInstance)key).Item.Name);
                        sb.Append(((GameObjectInstance)key).Item.Plural);
					}

					else
					{
                        Logger.Write("ToString ItemList count > 1 key: " + key.ToString());
						sb.Append(key.ToString());
					}
				}
				else
				{
                    Logger.Write("ToString ItemList item " + key.ToString());
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
