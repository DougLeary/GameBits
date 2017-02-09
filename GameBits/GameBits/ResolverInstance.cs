using System;
using System.Text;

namespace GameBits
{
    public class ResolverInstance : IResolver, IComparable
    {
        private int _count;
        private IResolver _item;

        public IResolver Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Number of objects that occur in this instance
        /// </summary>
		public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public ResolverInstance(IResolver item, int count)
        {
            Item = item;
            Count = count;
        }

        public ResolverInstance(IResolver item)
			: this(item, 1)
		{
        }

		public ResolverInstance()
			: this(null, 0)
        {
        }

        public IResolver Resolve()
        {
            Logger.Write("Resolve ResolverInstance: " + this.ToString() + "(" + Count + ")");
            return this;
        }

        public int CompareTo(object other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                if (other is ResolverInstance)
                {
                    return String.Compare(this.Item.ToString(), ((ResolverInstance)other).Item.ToString());
                }
                else
                {
                    return String.Compare(this.Item.ToString(), other.ToString());
                }
            }
        }
    }
}
