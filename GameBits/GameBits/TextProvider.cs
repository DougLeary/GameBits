namespace GameBits
{
	public class TextProvider : IGameBitsProvider
	{
		private Repository bits;
		private string[] source;

		public TextProvider(Repository repository)
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
