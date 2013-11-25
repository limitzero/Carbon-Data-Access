using System.Reflection;
using System.Xml.Serialization;
using Iesi.Collections.Generic;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Collections
{
	[Optimization("For many to one associations on collections, the parent will always manage the child object, so the option inverse='true' will be present on collections mapped as 'set'")]
	[Optimization("For many to one associations on collections, the option generic='true' will be present for generic set collections ")]
	[XmlRoot(ElementName = "set")]
	public class ManyToOneGenericSetCollectionBuilder :
		BaseNHCollectionElementDefinition, ICollectionBuilder
	{
		public bool IsApplicable(ModelConvention modelConvention, PropertyInfo property)
		{
			if (property.PropertyType.IsGenericType == true)
			{
				var underlyingType = property.PropertyType.GetGenericArguments()[0];

				if (ORMUtils.IsComponent(modelConvention, underlyingType)) return false;

				if (typeof(ISet<>).MakeGenericType(underlyingType) == property.PropertyType)
				{
					return true;
				}
			}

			return false;
		}

		[XmlIgnore]
		public bool IsBiDirectional { get; set; }

		public void Build(ModelConvention modelConvention,
						  string parentEntityPropertyName,
						  System.Type parentEntity, System.Type childEntity,
						  PropertyInfo property)
		{
			this.Access = modelConvention.MemberAccess.Strategy;
			this.Name = parentEntityPropertyName;
			this.IsInverse = true; // OPTIMIZATION: parent will always manage the children
			this.Cascade = "all";
			this.IsGeneric = true; // default

			this.Key.Column = BuildPrimaryKeyColumnName(modelConvention, parentEntity);
			this.Key.ForeignKey = BuildForeignKeyName(modelConvention, parentEntity, childEntity);

			if (this.IsBiDirectional == false)
			{
				this.CreateOneToManyAssociation();
				this.OneToMany.Class = RelationshipDefinition.CreateQualifiedName(childEntity);
			}
			else
			{
				this.Table = BuildJoinTableName(modelConvention, parentEntity, childEntity);
				this.CreateManyToManyAssociation();

				this.ManyToMany.Class = RelationshipDefinition.CreateQualifiedName(childEntity);
				this.ManyToMany.Column = BuildPrimaryKeyColumnName(modelConvention, childEntity);
			}
		}

		public string Serialize()
		{
			return ORMUtils.Serialize(this);
		}
	}
}