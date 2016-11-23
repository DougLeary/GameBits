using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	/// <summary>
	/// XML Provider for GameBits objects
	/// </summary>
	public class XmlProvider : IGameBitsProvider
	{
		public string FilePath { get; set; }

		private Repository bits;
		private XmlWriter writer;

		public XmlProvider(Repository repository, string filePath)
		{
			bits = repository;
			FilePath = filePath;
		}

		public void Save()
		{
			using (XmlWriter writer = XmlWriter.Create(FilePath))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("Tables");
				foreach (string key in bits.Tables.Keys)
				{
					bits.Tables[key].WriteXml(writer);
				}
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Close();
			}
		}

		public void Load()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(FilePath);
			XmlNodeList sourceTables = doc.GetElementsByTagName("Table");
			RollableTable tbl;

			// Make a pass to create all tables so table references will all be defined
			foreach (XmlNode tableNode in sourceTables)
			{
				tbl = Repository.GetTable(tableNode.Attributes["TableName"].Value);
			}

			// Second pass, populate table contents
			foreach (XmlNode tableNode in sourceTables)
			{
				tbl = Repository.GetTable(tableNode.Attributes["TableName"].Value);
				if (tbl.Rows.Count == 0)
				{
					FillRollableTableFromXml(tbl, tableNode);
				}
			}
		}

		private Object LoadObject(XmlNode node)
		{
			switch (node.Name)
			{
				case "GameObject":
					return GameObjectFromXml(node);
				case "Instance":
					return GameObjectInstanceFromXml(node);
				case "ItemRoll":
					return ItemRollFromXml(node);
				case "ItemList":
					return ItemListFromXml(node);
				case "TableRoll":
					return TableRollFromXml(node);
				default:
					return null;
			}
		}

		private void SaveObject(Object obj)
		{
			switch (obj.GetType().Name)
			{
				case "GameObject":
					GameObjectWriteXml((GameObject)obj);
					break;
				case "Instance":
					GameObjectInstanceWriteXml((GameObjectInstance)obj);
					break;
				case "ItemRoll":
					ItemRollWriteXml((ItemRoll)obj);
					break;
				case "ItemList":
					ItemListWriteXml((ItemList)obj);
					break;
				case "TableRoll":
					TableRollWriteXml((TableRoll)obj);
					break;
				default:
					break;
			}
		}

		private DieRoll DieRollFromXml(XmlNode node)
		{
			DieRoll obj = new DieRoll();
			obj.Dice = Utility.ParseAttribute(node, "Dice", 1);
			obj.Sides = Utility.ParseAttribute(node, "Sides", 20);
			obj.Modifier = Utility.ParseAttribute(node, "Modifier", 0);
			obj.Keep = Utility.ParseAttribute(node, "Keep", obj.Dice);
			return obj;
		}

		private void DieRollWriteXml(DieRoll obj)
		{
			writer.WriteStartElement("DieRoll");
			Utility.WriteAttribute(writer, "Dice", obj.Dice, 1);
			Utility.WriteAttribute(writer, "Sides", obj.Sides, 20);
			Utility.WriteAttribute(writer, "Modifier", obj.Modifier, 0);
			Utility.WriteAttribute(writer, "Keep", obj.Keep, obj.Dice);
			writer.WriteEndElement();
		}

		private GameObject GameObjectFromXml(XmlNode node)
		{
			GameObject obj = new GameObject();
			obj.Name = node.Attributes["Name"].Value;
			obj.Plural = Utility.ParseAttribute(node, "Plural", GameObject.DefaultPlural(obj.Name));
			obj.Description = Utility.ParseAttribute(node, "Description", obj.Name);
			return obj;
		}

		private void GameObjectWriteXml(GameObject obj)
		{
			writer.WriteStartElement("GameObject");
			writer.WriteAttributeString("Name", obj.Name);
			Utility.WriteAttribute(writer, "Plural", obj.Plural, GameObject.DefaultPlural(obj.Name));
			Utility.WriteAttribute(writer, "Description", obj.Description, obj.Name);
			writer.WriteEndElement();
		}

		private GameObjectInstance GameObjectInstanceFromXml(XmlNode node)
		{
			GameObjectInstance obj = new GameObjectInstance();
			obj.Count = Utility.ParseAttribute(node, "Count", 1);
			obj.Item = GameObjectFromXml(node.SelectSingleNode("GameObject"));
			return obj;
		}

		private void GameObjectInstanceWriteXml(GameObjectInstance obj)
		{
			writer.WriteStartElement("Instance");
			Utility.WriteAttribute(writer, "Count", obj.Count, 1);
			GameObjectWriteXml(obj.Item);
			writer.WriteEndElement();
		}

		private ItemList ItemListFromXml(XmlNode node)
		{
			ItemList obj = new ItemList();
			foreach (XmlNode child in node.ChildNodes)
			{
                IResolver item = (IResolver)(LoadObject(child));
				obj.Add(item);
			}
			return obj;
		}

		private void ItemListWriteXml(ItemList obj)
		{
			writer.WriteStartElement("ItemList");
			foreach (IResolver item in obj)
			{
				SaveObject(item);
			}
			writer.WriteEndElement();
		}

		private ItemRoll ItemRollFromXml(XmlNode node)
		{
			ItemRoll obj = new ItemRoll();
			obj.Multiplier = Utility.ParseAttribute(node, "Multiplier", 1);
			obj.Percent = Utility.ParseAttribute(node, "Percent", 100);
			// the only other node that is not a dieroll is item to be rolled
			XmlNode itemNode = node.SelectSingleNode("*[not (self::DieRoll)]");
			obj.Item = (IResolver)LoadObject(itemNode);
			obj.Dice = DieRollFromXml(node.SelectSingleNode("DieRoll"));
			return obj;
		}

		private void ItemRollWriteXml(ItemRoll obj)
		{
			writer.WriteStartElement("ItemRoll");
			Utility.WriteAttribute(writer, "Multiplier", obj.Multiplier, 1);
			Utility.WriteAttribute(writer, "Percent", obj.Percent, 100);
			SaveObject(obj.Item);
			SaveObject(obj.Dice);
			writer.WriteEndElement();
		}

		private void FillRollableTableFromXml(RollableTable tbl, XmlNode node)
		{
			tbl.TableName = node.Attributes["TableName"].Value;
			XmlNode diceNode = node.SelectSingleNode("DieRoll");
			tbl.Dice = DieRollFromXml(diceNode);
			XmlNodeList rowNodes = node.SelectNodes("Row");
			foreach (XmlNode rowNode in rowNodes)
			{
				RollableTableRow row = tbl.GetNewRow();
				row.HighRoll = Utility.ParseAttribute(rowNode, "HighRoll", int.MaxValue);
				row.LowRoll = Utility.ParseAttribute(rowNode, "LowRoll", row.HighRoll);

				// GameObject, Instance, ItemList, TableRoll, ItemRoll
                row.Item = (IResolver)(LoadObject(rowNode.FirstChild));
				tbl.Add(row);
			}
		}

		private void RollableTableWriteXml(RollableTable obj)
		{
			writer.WriteStartElement("Table");
			writer.WriteAttributeString("TableName", obj.TableName);
			SaveObject(obj.Dice);
			foreach (RollableTableRow row in obj.Rows)
			{
				writer.WriteStartElement("Row");
				Utility.WriteAttribute(writer, "HighRoll", row.HighRoll, int.MaxValue);
				SaveObject(row.Item);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private TableRoll TableRollFromXml(XmlNode node)
		{
			TableRoll obj = new TableRoll();
			obj.Table = Repository.GetTable(node.Attributes["TableName"].Value);
			obj.Rolls = Utility.ParseAttribute(node, "Rolls", 1);
			obj.IgnoreBelow = Utility.ParseAttribute(node, "IgnoreBelow", 0);
			obj.IgnoreAbove = Utility.ParseAttribute(node, "IgnoreAbove", int.MaxValue);
			return obj;
		}

		private void TableRollWriteXml(TableRoll obj)
		{
			writer.WriteStartElement("TableRoll");
			writer.WriteAttributeString("TableName", obj.Table.TableName);
			Utility.WriteAttribute(writer, "Rolls", obj.Rolls, 1);
			Utility.WriteAttribute(writer, "IgnoreBelow", obj.IgnoreBelow, 0);
			Utility.WriteAttribute(writer, "IgnoreAbove", obj.IgnoreAbove, int.MaxValue);
			writer.WriteEndElement();
		}


	}

}
