using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Zenseless.Patterns
{
	/// <summary>
	/// Contains class instance serialization/deserialization methods. 
	/// Can be used for persisting class instances to disc and reading them back to memory.
	/// </summary>
	public static class XmlSerializationExtensions
	{
		/// <summary>
		/// Deserializes from an XML file into a new class instance of a given type.
		/// </summary>
		/// <typeparam name="DataType">The type of the class that will be deserialized.</typeparam>
		/// <param name="fileName">The file name from which the serialized instance will be restored from.</param>
		/// <returns>Deserialized class instance</returns>
		public static DataType? FromXMLFile<DataType>(this string fileName)
		{
			using StreamReader inFile = new(fileName);
			XmlSerializer formatter = new(typeof(DataType));
			var obj = formatter.Deserialize(inFile);
			if (obj is null) return default;
			return (DataType)obj;
		}

		/// <summary>
		/// Deserializes from an XML string into a new class instance of a given type.
		/// </summary>
		/// <typeparam name="DataType">The type of the class that will be deserialized.</typeparam>
		/// <param name="xmlString">XML string from which to deserialize.</param>
		/// <returns>Deserialized class instance</returns>
		public static DataType? FromXmlString<DataType>(this string xmlString)
		{
			using StringReader input = new(xmlString);
			XmlSerializer formatter = new(typeof(DataType));
			var obj = formatter.Deserialize(input);
			if (obj is null) return default;
			return (DataType)obj;
		}

		/// <summary>
		/// Serializes the given class instance into a XML format file.
		/// </summary>
		/// <param name="serializable">The class instance to be serialized.</param>
		/// <param name="fileName">The file name the serialized instance will be stored to.</param>
		public static void ToXMLFile(this object serializable, string fileName)
		{
			if (serializable is null) throw new ArgumentNullException(nameof(serializable));

			XmlSerializer formatter = new(serializable.GetType());
			using StreamWriter outfile = new(fileName);
			formatter.Serialize(outfile, serializable);
		}

		/// <summary>
		/// Serializes the given class instance into a XML string.
		/// </summary>
		/// <param name="serializable">The class instance to be serialized.</param>
		public static string ToXmlString(this object serializable)
		{
			if (serializable is null) throw new ArgumentNullException(nameof(serializable));
			XmlSerializer formatter = new(serializable.GetType());
			StringBuilder builder = new();
			XmlWriterSettings settings = new()
			{
				Encoding = Encoding.Default,
				Indent = false,
				OmitXmlDeclaration = true,
				NamespaceHandling = NamespaceHandling.OmitDuplicates
			};
			using (XmlWriter writer = XmlWriter.Create(builder, settings))
			{
				formatter.Serialize(writer, serializable);
			}
			string output = builder.ToString();
			return output;
		}
	}
}
