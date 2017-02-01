using System.Collections.Generic;

namespace GameBits
{
	/// <summary>
	/// A namespace-like hierarchical grouping
	/// </summary>
	public class Category
	{
		public string Name { get; set; }
		private List<Category> _categories;
		private List<RollableTable> _tables;
		private List<GameObject> _gameObjects;

		public List<Category> Categories { get { return _categories; } }
		public List<RollableTable> RollableTables { get { return _tables; } }
		public List<GameObject> GameObjects { get { return _gameObjects; } }

		public Category(string name)
		{
			Name = name;
			_categories = new List<Category>();
			_tables = new List<RollableTable>();
			_gameObjects = new List<GameObject>();
		}
	}
}
