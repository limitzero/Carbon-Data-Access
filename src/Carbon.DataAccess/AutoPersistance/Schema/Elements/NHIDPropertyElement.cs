using System;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Builders;
using NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys;
using NHibernate.Carbon.AutoPersistance.Conventions;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "id")]
	public class NHIDPropertyElement
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "column")]
		public string Column { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "access")]
		public string Access { get; set; }

		[XmlText]
		public string IDGeneration { get; set; }

		[XmlIgnore]
		public NHIDGenerator IDGenerator { get; private set; }

		public NHIDPropertyElement()
		{
			this.IDGenerator = new NHIDGenerator();
		}

		public NHIDPropertyElement Build(ModelConvention modelConvention, System.Type entity)
		{
			NHIDPropertyElement idProperty = new NHIDPropertyElement();
			var builder = new PrimaryKeyNameBuilder(modelConvention, entity);

			var idColumn = entity.GetProperty(modelConvention.PrimaryKey.PrimaryKeyName);

			idProperty.Name = modelConvention.PrimaryKey.PrimaryKeyName;
			idProperty.Column = builder.Build();
			idProperty.Access = modelConvention.PrimaryKey.MemberAccess.Strategy;
			idProperty.Type = idColumn.PropertyType.Name;

			CheckForIdGenerationConvention(idProperty, entity, idColumn);

			return idProperty;
		}

		private void CheckForIdGenerationConvention(NHIDPropertyElement idProperty, System.Type entity, PropertyInfo idColumn)
		{
			//check to see if we have an interceptor for generating the ID value per entity type:
			var idGenerationInterceptorType = typeof(IIDGenerationConventionInterceptor<>).MakeGenericType(entity);

			var foundInterceptorType = (from match in entity.Assembly.GetTypes()
			                            where match.IsClass == true
			                                  && match.IsAbstract == false
			                                  && idGenerationInterceptorType.IsAssignableFrom(match)
			                            select match).FirstOrDefault();

			if (foundInterceptorType != null)
			{
			    var interceptor = entity.Assembly.CreateInstance(foundInterceptorType.FullName) as IIDGenerationConventionInterceptor;

				if (interceptor != null)
				{
					interceptor.Configure();
					idProperty.IDGenerator.CreateGenerator(interceptor.IDGenerationType);
				}
			}
			else
			{
			    if (typeof(Guid) == idColumn.PropertyType)
			        idProperty.IDGenerator.CreateGenerator(IdGenerationTypes.CombGuid);
			    else
			    {
			        idProperty.IDGenerator.CreateGenerator(IdGenerationTypes.Native);
			    }
			}
		}

		public string Serialize()
		{
			this.IDGeneration = this.IDGenerator.Serialize();
			return ORMUtils.Serialize(this);
		}
	}
}