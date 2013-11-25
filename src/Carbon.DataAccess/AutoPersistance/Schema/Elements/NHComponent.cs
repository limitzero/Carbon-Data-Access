using System;
using System.Reflection;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.Extensions;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "component")]
	public class NHComponent
	{
		[XmlIgnore]
		public string Header = string.Empty;

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "access")]
		public string Access { get; set; }

		[XmlText]
		public string Properties { get; set; }

		public NHComponent Build(ModelConvention modelConvention, PropertyInfo property)
		{
			NHComponent component = new NHComponent
			                        	{
			                        		Access = modelConvention.MemberAccess.Strategy,
			                        		Class = property.PropertyType.Name,
			                        		Name = property.Name
			                        	};

			component.Header = string.Format("<!-- '{0}' has component/value-object class of  '{1}' realized as '{2}' --> ",
			                       property.DeclaringType.Name,
			                       property.PropertyType.Name,
			                       property.Name);

			var properties = new NHProperty().Build(modelConvention, 
				property.PropertyType, 
				property.PropertyType.GetProperties());

			if(properties != null)
			{
				component.Properties = properties.SerializeAll();
			}

			return component;
		}

		public string Serialize()
		{
			string results = string.Concat(Header, Environment.NewLine, ORMUtils.Serialize(this));
			return results;
		}
	}
}