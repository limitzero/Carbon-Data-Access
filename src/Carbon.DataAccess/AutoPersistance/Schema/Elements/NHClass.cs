using System;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Builders;
using NHibernate.Carbon.AutoPersistance.Builders.For.Components;
using NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys;
using NHibernate.Carbon.AutoPersistance.Builders.For.Properties;
using NHibernate.Carbon.AutoPersistance.Builders.For.Subclasses;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "class")]
	public class NHClass
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "table")]
		public string Table { get; set; }

		[XmlIgnore]
		public NHIDPropertyElement IDProperty { get; set; }

		/// <summary>
		/// Gets or sets the underlying definition of the class for properties, components, relationships, subclasses, etc.
		/// </summary>
		[XmlText]
		public string Text { get; set; }

		public NHClass()
		{
			this.IDProperty = new NHIDPropertyElement();
		}

		public NHClass Build(ModelConvention modelConvention, System.Type entity)
		{
			NHClass nhClass = new NHClass
								{
									Name = entity.Name,
									Table = modelConvention.CanPluralizeTableNames ?
											new Inflector().Pluralize(entity.Name) : entity.Name
								};

			this.BuildIdColumn(nhClass, modelConvention, entity);
			this.BuildPropertiesAndComponents(nhClass, modelConvention, entity);
			this.BuildSubClasses(nhClass, modelConvention, entity);
			this.BuildRelationships(nhClass, modelConvention, entity);

			return nhClass;
		}

		private void BuildIdColumn(NHClass nhClass, 
			ModelConvention modelConvention, 
			System.Type entity)
		{
			var builder = new PrimaryKeyBuilder(modelConvention, entity);
			nhClass.Text += builder.Build();
		}

		private void BuildPropertiesAndComponents(NHClass nhClass, 
			ModelConvention modelConvention, 
			System.Type entity)
		{
			var propertyBuilder = new PropertyBuilder(modelConvention, entity);
			nhClass.Text += propertyBuilder.Build();

			var componentBuilder = new ComponentBuilder(modelConvention, entity);
			componentBuilder.PropertiesForInspection = propertyBuilder.ExcludedProperties;
			nhClass.Text += componentBuilder.Build();
		}

		private void BuildComponents(NHClass nhClass, ModelConvention modelConvention, System.Type entity)
		{
			var propertyBuilder = new PropertyBuilder(modelConvention, entity);
			propertyBuilder.Build();

			var builder = new ComponentBuilder(modelConvention, entity);
			builder.PropertiesForInspection = propertyBuilder.ExcludedProperties;

			
			//nhClass.Components = builder.Build();
		}

		private void BuildSubClasses(NHClass nhClass, ModelConvention modelConvention, System.Type entity)
		{
			ISubclassBuilder subclassBuilder = new SubClassBuilder(modelConvention, entity);
			nhClass.Text += subclassBuilder.Build();
		}

		private void BuildRelationships(NHClass nhClass,ModelConvention modelConvention, System.Type entity)
		{
			ManyToOneRelationshipStrategy many2OneBuilder = new ManyToOneRelationshipStrategy(modelConvention, entity);
			ManyToManyRelationshipStrategy many2ManyBuilder = new ManyToManyRelationshipStrategy(modelConvention, entity);
			nhClass.Text += many2OneBuilder.Build();
			nhClass.Text += many2ManyBuilder.Build();
		}

		public string Serialize()
		{
			string results = string.Concat(string.Format("<!-- entity: {0} -->", this.Name), 
				System.Environment.NewLine, 
				ORMUtils.Serialize(this));
			return results;
		}
	}
}