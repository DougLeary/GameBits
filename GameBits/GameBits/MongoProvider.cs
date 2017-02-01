namespace GameBits
{
    public class MongoProvider : IGameBitsProvider
    {
        private Repository bits;
        private string[] source;

        public MongoProvider(Repository repository)
        {
            bits = repository;
        }

        public void Save()
        {

        }

        public void Load()
        {
            Load(source);
        }

        public void Load(string[] sourceArray)
        {

        }
    }
}
