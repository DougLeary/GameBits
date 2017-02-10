using System;
using System.Collections.Generic;
using System.Text;

namespace GameBits
{
    /// <summary>
    /// List of IResolvers that can be treated as a single item, for example as an entry in a RollableTable
    /// </summary>
    public class ItemList : List<IResolver>, IResolvableList<int>, IResolver, IRollable
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

        public IResolver  GetItem(int Index)
        {
            if (Index < this.Count) {
                return this[Index];
            }
            else
            {
                return null;
            }
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

        public IResolver Roll()
        {
            return Roll(0, int.MaxValue);
        }

        public IResolver Roll(int ignoreBelow, int ignoreAbove)
        {
            DieRoll dice = new DieRoll(1, this.Count, 0);
            int roll = -1;
            int attempts = 0;
            while (attempts <= Constants.MaxRollAttempts && (roll < ignoreBelow || roll > ignoreAbove))
            {
                roll = dice.Roll();
                attempts++;
            }

            IResolver result = this[roll];
            return result.Resolve();
        }

        /// <summary>
        /// Return a new flattened list (convert nested lists to individual items)
        /// </summary>
        /// <returns></returns>
        public ItemList Flatten()
        {
            ItemList list = new ItemList();
            foreach (IResolver item in this)
            {
                if (item is ItemList)
                {
                    ItemList innerList = ((ItemList)item).Flatten();
                    foreach (IResolver thing in innerList)
                    {
                        list.Add(thing);
                    }
                }
                else list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// Returns a flattened list with multiple occurrences of identically named objects converted to instances with counts.
        /// </summary>
        /// <returns></returns>
        public ItemList Normalize()
        {
            ItemList list = this.Flatten();
            if (list.Count == 0) return list;

            //for (int i = 0; i < list.Count; i++)
            //{
            //    if (list[i] is ItemList)
            //    {
            //        list[i] = ((ItemList)list[i]).Normalize();
            //    }
            //}

            list.Sort();
            int count = 1;
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].CompareTo(list[i-1]) == 0)
                {
                    count++;
                    ((ResolverInstance)list[i - 1]).Count = count;
                    list.RemoveAt(i);
                }
                else
                {
                    count = 1;
                }
            }
            return list;
        }

        public override string ToString()
        {
            // by default return the string in compressed format; { zombie, zombie, zombie, ghost } = "3 zombie, 1 ghost"
            return this.ToString(Format.Compressed);
        }

        public string ToString(Format format)
        {
            if (this.Count == 0) return String.Empty;
            ItemList list;
            if (format == Format.Compressed)
                list = this.Normalize();
            else
                list = this.Flatten();

            // Add each item to the output once. If the item is already in the output list increments its count.
            // In each SortedList item the IResolver is the ItemList item and the int is the number of times it occurs in the ItemList.
            // TODO: implement Uncompressed format (sorted, but with all items included and each count = 1). 
            StringBuilder sb = new StringBuilder();

            foreach (IResolver item in list)
            {   // TEMPORARY
                sb.Append(item.ToString());
                sb.Append(ItemList.Separator);
            }

            //foreach (IResolver key in list.Keys)
            //{
            //    int count = list[key];
            //    if (count > 1)
            //    {
            //        sb.Append(count.ToString());
            //        sb.Append(" ");
            //        if (key.GetType() == typeof(GameObject))
            //        {
            //            sb.Append(((GameObject)key).Plural);
            //        }
            //        else if (key.GetType() == typeof(GameObjectInstance))
            //        {
            //            sb.Append(((GameObjectInstance)key).Item.Plural);
            //        }

            //        else
            //        {
            //            sb.Append(key.ToString());
            //        }
            //    }
            //    else
            //    {
            //        sb.Append(key.ToString());
            //    }
            //    sb.Append(ItemList.Separator);
            //}

            if (sb.Length > 2)
            {
                return sb.ToString(0, sb.Length - 2);
            }
            else
            {
                return sb.ToString();
            }
        }

        /// <summary>
        /// Always compare only the first element of ItemList with the other object to avoid recursive sorting
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public int CompareTo(object Other)
        {
            String thisString = String.Empty;

            if (this.Count > 0)
            {
                thisString = this[0].ToString();
            }

            String otherString = String.Empty;
            if (Other is ItemList)
            {
                ItemList otherList = (ItemList)Other;
                if (otherList.Count > 0)
                {
                    otherString = otherList[0].ToString();
                }
            }
            else
            {
                otherString = Other.ToString();
            }

            return thisString.CompareTo(otherString);
        }

    }
}
