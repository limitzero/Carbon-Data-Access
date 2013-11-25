using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "version")]
	public class NHVersionProperty
	{
		[XmlIgnore]
		public string Header = string.Empty;

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "column")]
		public string Column { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "unsaved-value")]
		public string UnSavedValue { get; set; }

		public NHVersionProperty()
		{

		}

		public NHVersionProperty Build(ModelConvention modelConvention,
		                               System.Type entity,
		                               ICollection<PropertyInfo> properties)
		{
			NHVersionProperty versionProperty = null;

			Header = string.Format("<!-- properties for entity '{0}' --> ", entity.Name);

			if (modelConvention.Versioning != null)
			{
				if (string.IsNullOrEmpty(modelConvention.Versioning.VersionPropertyName) == false &&
				    modelConvention.Versioning.UnsavedValue != null)
				{
					PropertyInfo property = (from match in properties
					                         where match.Name == modelConvention.Versioning.VersionPropertyName.Trim()
					                         select match).FirstOrDefault();

					if (property != null)
					{
						versionProperty = new NHVersionProperty
						                  	{
						                  		Column = modelConvention.PropertyNamingStrategy.Execute(property.Name, property.PropertyType),
						                  		Name = modelConvention.Versioning.VersionPropertyName,
						                  		Type = property.PropertyType.Name,
						                  		UnSavedValue = modelConvention.Versioning.UnsavedValue.ToString()

						                  	};

					}
				}
			}

			return versionProperty;
		}

		public string Serialize()
		{
			return ORMUtils.Serialize(this);
		}
	}
}