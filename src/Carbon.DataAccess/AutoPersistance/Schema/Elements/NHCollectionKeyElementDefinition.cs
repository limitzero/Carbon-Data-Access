using System.Xml.Serialization;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "key")]
	public class NHCollectionKeyElementDefinition
	{
		[XmlAttribute(AttributeName = "column")]
		public string Column { get; set; }

		[XmlAttribute(AttributeName = "foreign-key")]
		public string ForeignKey { get; set; }

	}
}