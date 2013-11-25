using System;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	/// <summary>
	/// Base class representing the collection description for NHibernate between persistent objects.
	/// </summary>
	public abstract class BaseNHCollectionElementDefinition : BaseRelationshipStrategy
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "table")]
		public string Table { get; set; }

		[XmlAttribute(AttributeName = "schema")]
		public string Schema { get; set; }

		[XmlAttribute(AttributeName = "lazy")]
		public bool IsLazy { get; set; }

		[XmlAttribute(AttributeName = "inverse")]
		public bool IsInverse { get; set; }

		[XmlAttribute(AttributeName = "cascade")]
		public string Cascade { get; set; }

		[XmlAttribute(AttributeName = "sort")]
		public string Sort { get; set; }

		[XmlAttribute(AttributeName = "order-by")]
		public string OrderBy { get; set; }

		[XmlAttribute(AttributeName = "where")]
		public string Where { get; set; }

		[XmlAttribute(AttributeName = "fetch")]
		public string Fetch { get; set; }

		[XmlAttribute(AttributeName = "batch-size")]
		public string BatchSize { get;  set; }

		[XmlAttribute(AttributeName = "generic")]
		public bool IsGeneric { get; set; }

		[XmlAttribute(AttributeName = "access")]
		public string Access { get; set; }

		[XmlElement(ElementName = "key")]
		public NHCollectionKeyElementDefinition Key { get; set; }

		[XmlElement(ElementName = "one-to-many")]
		public NHCollectionKeyOneToManyElement OneToMany { get; set; }

		[XmlElement(ElementName = "many-to-many")]
		public NHCollectionKeyManyToManyElement ManyToMany { get; set; }

		public BaseNHCollectionElementDefinition()
		{
			this.Key = new NHCollectionKeyElementDefinition();
		}

		public void SetBatchSize(int size)
		{
			this.BatchSize = size.ToString();
		}

		public void CreateOneToManyAssociation()
		{
			this.OneToMany = new NHCollectionKeyOneToManyElement();
			this.ManyToMany = null;
		}

		public void CreateManyToManyAssociation()
		{
			this.ManyToMany = new NHCollectionKeyManyToManyElement();
			this.OneToMany = null;
		}

		public static string CreateQualifiedName(System.Type entity)
		{
			var parts = entity.AssemblyQualifiedName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			return string.Concat(entity.FullName, ", ", parts[1].Trim());
		}

	}
}