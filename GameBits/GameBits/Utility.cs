using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	class Utility
	{
		/// <summary>
		/// Get an attribute from an XmlNode or use the default value
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int ParseAttribute(XmlNode node, string attributeName, int defaultValue)
		{
			XmlAttribute att = node.Attributes[attributeName];
			if (att != null)
			{
				return Convert.ToInt32(att.Value);
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get an attribute from an XmlNode or use the default value
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string ParseAttribute(XmlNode node, string attributeName, string defaultValue)
		{
			XmlAttribute att = node.Attributes[attributeName];
			if (att != null)
			{
				return att.Value;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Write an attribute to XML if its value differs from the default
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		/// <param name="defaultValue"></param>
		public static void WriteAttribute(XmlWriter writer, string attributeName, int attributeValue, int defaultValue)
		{
			if (attributeValue != defaultValue)
			{
				writer.WriteAttributeString(attributeName, attributeValue.ToString());
			}
		}

		/// <summary>
		/// Write an attribute to XML if its value differs from the default
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attributeName"></param>
		/// <param name="attributeValue"></param>
		/// <param name="defaultValue"></param>
		public static void WriteAttribute(XmlWriter writer, string attributeName, string attributeValue, string defaultValue)
		{
			if (attributeValue != defaultValue)
			{
				writer.WriteAttributeString(attributeName, attributeValue);
			}
		}

	}
}
