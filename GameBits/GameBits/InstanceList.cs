namespace GameBits
{
    public class InstanceList: ItemList
    {
        public void Add(IResolver item)
        {
            ResolverInstance instance = new ResolverInstance(item);
        }
    }
}
