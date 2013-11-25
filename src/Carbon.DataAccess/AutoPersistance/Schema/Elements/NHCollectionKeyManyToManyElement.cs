using System.Xml.Serialization;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot("many-to-many")]
	public class NHCollectionKeyManyToManyElement
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "column")]
		public string Column { get; set; }
	}
}