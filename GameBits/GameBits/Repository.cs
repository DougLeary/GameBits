using System.Collections.Generic;

namespace GameBits
{

	/// <summary>
	/// Objects and tables available to the user during their session
	/// </summary>
	public class Repository : GameBits.IRepository
	{
		private string _name;
		public string Name { get; set; }

		private Dictionary<string, RollableTable> _tables;

		/// <summary>
		/// Collection of rollable tables available to the user session
		/// </summary>
		public Dictionary<string, RollableTable> Tables
		{
			get { return _tables; }
		}

		private static string _defaultRepositoryName = "UserSessionRepository";

		/// <summary>
		/// Constructor
		/// </summary>
		public Repository()
		{
			_name = _defaultRepositoryName;
			_tables = new Dictionary<string, RollableTable>();
		}

		/// <summary>
		/// Get or create the current repository
		/// </summary>
		/// <returns></returns>
		public static Repository GetCurrentRepository()
		{
			Repository bits;
			if (System.Web.HttpContext.Current.Session[_defaultRepositoryName] == null)
			{
				bits = new Repository();
				System.Web.HttpContext.Current.Session[_defaultRepositoryName] = bits;
			}
			else
			{
				bits = (Repository)(System.Web.HttpContext.Current.Session[_defaultRepositoryName]);
			}
			return bits;
		}

        /// <summary>
        /// Return the GameBits Repository from the current ASP.Net user session; 
        /// meant to be called from a web client page or web service. 
        /// </summary>
        /// <param name="provider">data provider</param>
		/// <param name="reload">if true then remove the current GameBits Repository and start a new one</param>
        /// <returns></returns>
        // not implemented: <param name="filePath">path to an XML file containing table definitions</param>
        public static void Connect(XmlProvider provider, bool reload)
		{
			if (reload)
			{
				System.Web.HttpContext.Current.Session.Remove(_defaultRepositoryName);
			}
			provider.Load();

		}

		public static void Connect(XmlProvider provider)
		{
			Connect(provider, false);
		}

		public static void Connect()
		{
			Connect(null);
		}


		/// <summary>
		/// Get or create a RollableTable by name
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static RollableTable GetTable(string tableName)
		{
			Repository bits = GetCurrentRepository();

			if (bits.Tables.ContainsKey(tableName))
			{
				return bits.Tables[tableName];
			}
			else
			{
				return NewTable(tableName);
			}
		}

		/// <summary>
		/// Return true if a table name exists in the session
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static bool TableExists(string tableName)
		{
			Repository bits = GetCurrentRepository();

			return (bits.Tables.ContainsKey(tableName));
		}

		/// <summary>
		/// Create a RollableTable and add it to MyTables
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static RollableTable NewTable(string tableName)
		{
			Repository bits = GetCurrentRepository();

			RollableTable rt = new RollableTable(tableName);
			bits.Tables.Add(tableName, rt);
			return rt;
		}

		public static void Save(XmlProvider provider)
		{
			if (provider != null)
			{
				provider.Save();
			}
		}

		public static void Load(XmlProvider provider)
		{
			if (provider != null)
			{
				provider.Load();
			}
		}
	}
}
