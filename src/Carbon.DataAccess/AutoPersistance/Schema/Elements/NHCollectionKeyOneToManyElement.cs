using System.Xml.Serialization;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot("one-to-many")]
	public class NHCollectionKeyOneToManyElement
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }
	}
}