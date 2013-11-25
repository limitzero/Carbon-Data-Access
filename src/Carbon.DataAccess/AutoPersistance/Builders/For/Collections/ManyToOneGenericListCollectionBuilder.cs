using System;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;
using NHibernate.Carbon.Extensions;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Collections
{
	[Optimization("For many to one associations on collections, the parent will always manage the child object, so the option inverse='true' will be present on collections mapped as 'bag'")]
	[Optimization("For many to one associations on collections, the option generic='true' will be present for generic implmentations of IList for 'bag' collections")]
	[XmlRoot(ElementName = "list")]
	public class ManyToOneGenericListCollectionBuilder : BaseNHCollectionElementDefinition,   ICollectionBuilder
	{
		private StringBuilder _builder = new StringBuilder();

		public bool IsApplicable(ModelConvention modelConvention, PropertyInfo property)
		{
			bool retval = false;

			retval = property.PropertyType.IsGenericType &&
			         ORMUtils.IsComponent(modelConvention, property.PropertyType.GetGenericArguments()[0]);

			return retval;
		}

		public bool IsBiDirectional { get; set; }

		public void Build(ModelConvention modelConvention, string parentEntityPropertyName, 
			System.Type parentEntity, 
			System.Type childEntity, 
			PropertyInfo property)
		{
			var nhProperties = new NHProperty().Build(modelConvention, childEntity, childEntity.GetProperties());
			var properties = nhProperties.SerializeAll();

			_builder.Append(string.Format("<list name=\"{0}\" table=\"{1}\"  access=\"{2}\" cascade=\"all\">", 
				     parentEntityPropertyName,
					 string.Concat(parentEntityPropertyName,"_components").ToLower(), 
					 modelConvention.MemberAccess.Strategy
					 )).Append(Environment.NewLine)
				.Append(string.Format("<key column=\"{0}\" />", BuildPrimaryKeyColumnName(modelConvention, parentEntity)))
				.Append(Environment.NewLine)
				.Append("<list-index column=\"pos\" />").Append(Environment.NewLine)
				.Append(string.Format("<composite-element class=\"{0}\">", childEntity.Name))
				.Append(Environment.NewLine)
				.Append(string.Format("<many-to-one name=\"{0}\" />", parentEntity.Name))
				.Append(Environment.NewLine)
				.Append(properties).Append(Environment.NewLine)
				.Append("</composite-element>").Append(Environment.NewLine)
				.Append("</list>");
		}

		public string Serialize()
		{
			return _builder.ToString();
		}
	}
}