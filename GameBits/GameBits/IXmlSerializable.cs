using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GameBits
{
	public interface IXmlSerializable
	{
		/// <summary>
		/// Loads an object's properties from an Xml node
		/// </summary>
		/// <param name="node"></param>
		void FromXml(XmlNode node);

		/// <summary>
		/// Writes an object to an Xml stream
		/// </summary>
		/// <param name="writer"></param>
		void WriteXml(XmlWriter writer);
	}
}
