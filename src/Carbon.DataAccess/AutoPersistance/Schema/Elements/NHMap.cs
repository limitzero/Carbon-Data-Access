using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "hibernate-mapping", Namespace = "urn:nhibernate-mapping-2.2")]
	public class NHMap
	{
		[XmlAttribute(AttributeName = "assembly")]
		public string Assembly { get; set; }

		[XmlAttribute(AttributeName = "namespace")]
		public string Namespace { get; set; }

		[XmlAttribute(AttributeName = "default-lazy")]
		public bool UseLazyLoading { get; set; }

		[XmlIgnore]
		public NHClass Class { get; set; }

		[XmlText]
		public string ClassDefinition { get; set; }

		public NHMap()
		{
			this.Class = new NHClass();
		}

		public NHMap Build(ModelConvention modelConvention, System.Type entity)
		{
			this.Assembly = entity.Assembly.GetName().Name;
			this.Namespace = entity.Namespace;
			this.Class = new NHClass().Build(modelConvention, entity);
			return this;
		}

		public string Serialize()
		{
			this.ClassDefinition = this.Class.Serialize();
			return ORMUtils.Serialize(this);
		}
	}
}