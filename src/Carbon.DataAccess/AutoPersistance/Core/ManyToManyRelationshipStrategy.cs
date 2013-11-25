using System;
using System.Linq;
using System.Text;
using System.Reflection;
using Iesi.Collections;
using NHibernate.Carbon.AutoPersistance.Builders.For.Collections;

namespace NHibernate.Carbon.AutoPersistance.Core
{
	/// <summary>
	/// Concrete instance of a relationship that will handle 
	/// the man-to-many relationship between one entity and 
	/// others in the model. 
	/// </summary>
	public class ManyToManyRelationshipStrategy : BaseRelationshipStrategy, IRelationship
	{
		private readonly ModelConvention _convention;
		private readonly System.Type _entity;

		public ManyToManyRelationshipStrategy(ModelConvention convention, System.Type entity)
		{
			_convention = convention;
			_entity = entity;
		}

		public string Build()
		{
			string results = string.Empty;

			results = BuildAllRelationshipsBasedOnCollectionsFor(_entity);

			return results;
		}

		private string BuildAllRelationshipsBasedOnCollectionsFor(System.Type currentEntity)
		{
			StringBuilder results = new StringBuilder();

			foreach (PropertyInfo property in currentEntity.GetProperties())
			{
				System.Type propertyCollectionType = GetCollectionTypeFor(property.PropertyType);
				if (propertyCollectionType != null)
				{
					// the current entity has a property that is defined as a collection type
					// need to look on the reverse side to determine the "directedness" 
					// (i.e. collections of each type on both sides -> use join table...bi-directional, 
					// collection of one type on either side -> use regular many-to-many join)
					bool isBiDirectional = false;

					foreach (PropertyInfo pr in propertyCollectionType.GetProperties())
					{
						if (pr.PropertyType.IsGenericType)
						{
							if (pr.PropertyType.GetGenericArguments()[0] == currentEntity)
							{
								// this is a bidirectional relationship;
								isBiDirectional = true;
								break;
							}
						}
					}

					results.Append(DefineRelationshipFor(property.Name, 
						currentEntity, 
						propertyCollectionType, 
						isBiDirectional, 
						property.PropertyType.IsGenericType));
				}
			}

			return results.ToString();
		}

		private PropertyInfo FindPropertyDefinedByTypeOn(System.Type entity, 
			System.Type propertyDefinitionToFind)
		{
			PropertyInfo results = null;
			foreach (PropertyInfo property in entity.GetProperties())
			{
				if (property.PropertyType == propertyDefinitionToFind)
				{
					results = property;
					break;
				}
			}

			return results;
		}

		private System.Type GetCollectionTypeFor(System.Type property)
		{
			System.Type modelType = null;

			bool isObjectArray = property.FullName.Contains("[]");
			bool isGenericList = property.IsGenericType;

			// exclude the conversion of ValueType? to Nullable<ValueType>
			// which is interpreted as a collection:
			if (property.FullName.StartsWith(typeof(Nullable<>).FullName))
			{
				return modelType;
			}

			if (isObjectArray)
			{
				object typ = property.Assembly.CreateInstance(property.FullName);
				modelType = typ.GetType();
			}

			if (isGenericList)
				modelType = property.GetGenericArguments()[0];

			return modelType;
		}


		private string DefineRelationshipFor(string parentEntityPropertyName, 
			System.Type parentEntity, 
			System.Type childEntity, 
			bool isBiDirectional, 
			bool isGeneric)
		{
			var result = string.Empty;
			var property = _entity.GetProperty(parentEntityPropertyName);

			ICollectionBuilder builder = this.GetCollectionBuilder(property);
			builder.IsBiDirectional = isBiDirectional;

			if(isBiDirectional == false)
			{
				// many-to-many relationship (non-bi-directional, i.e. no "join table"):	
				builder.Build(_convention, parentEntityPropertyName, parentEntity, childEntity, property);
				
				result =
					string.Concat(
						string.Format("<!-- \"{0}\" has many instances of \"{1}\" -->", parentEntity.Name,
						              childEntity.Name),
						System.Environment.NewLine,
						builder.Serialize());
			}
			else
			{
				// many-many relationship (bi-directional, need "join table" for this):
				result = string.Format("<!-- '{0}' has many '{1}' and '{1}' has many '{0}' linked by '{2}' --> ",
				                       parentEntity.Name, childEntity.Name,
				                       BuildJoinTableName(_convention, parentEntity, childEntity));
				result += string.Concat(System.Environment.NewLine, builder.Serialize());
			}

			return result;
		}

		private ICollectionBuilder GetCollectionBuilder(PropertyInfo property)
		{
			ICollectionBuilder aBuilder = null;

			var builders = (from match in this.GetType().Assembly.GetTypes()
			                where typeof (ICollectionBuilder).IsAssignableFrom(match)
			                      && match.IsClass == true
			                      && match.IsAbstract == false
			                select match).Distinct().ToList();

			foreach (var builder in builders)
			{
				var theBuilder = this.GetType().Assembly.CreateInstance(builder.FullName) as ICollectionBuilder;
				if(theBuilder == null) continue;

				if(theBuilder.IsApplicable(this._convention, property) == true)
				{
					aBuilder = theBuilder;
					break;
				}
			}

			return aBuilder;
		}


		private string DefineRelationshipFor2(string parentEntityPropertyName, System.Type parentEntity, System.Type childEntity, bool isBiDirectional, bool isGeneric)
		{
			StringBuilder builder = new StringBuilder();

			if (!isBiDirectional)
			{
				#region -- many-to-one mapping (note the parent is not required in this implementation if generics are used)--
				builder.Append(string.Format("<!-- '{0}' has many '{1}' --> ", parentEntity.Name, childEntity.Name));
				builder.Append("\r\n");


				// start tag:
				if (!_convention.ManyToManyTableName.CreateAsSet)
				{
					builder.Append("<bag");
				}
				else
				{
					builder.Append("<set");
				}

				//attribute: access
				builder.Append(BuildAttribute("access", _convention.MemberAccess.Strategy));

				// attribute: name
				builder.Append(BuildAttribute("name", parentEntityPropertyName));

				//attribute: table
				//builder.Append(BuildAttribute("table", BuildJoinTableName(convention, entity, definition.ReferencedEntity)));

				//attribute: inverse
				builder.Append(BuildAttribute("inverse", "false"));

				//attribute: cascade
				builder.Append(BuildAttribute("cascade", "all"));

				//attribute: cascade
				builder.Append(BuildAttribute("generic", isGeneric.ToString().ToLower()));

				//attribute: lazy
				//builder.Append(MappingHelper.BuildAttribute("lazy", visitee.Lazy.ToString().ToLower()));

				builder.Append(" >");

				//attribute: order-by
				//builder.Append(MappingHelper.BuildAttribute("order-by", visitee.Order_By.Strategy));

				// element: key
				builder.Append("<key");
				//attribute:  column
				builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(_convention, parentEntity)));
				//attribute:  foreign-key
				builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(_convention, parentEntity, childEntity)));
				builder.Append("/>");

				// element: one-to-many
				builder.Append("<one-to-many");

				//attribute:  class
				//builder.Append(BuildAttribute("class", childEntity.Name));
				builder.Append(BuildAttribute("class", RelationshipDefinition.CreateQualifiedName(childEntity)));

				//attribute:  column
				//builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, definition.ReferencedEntity)));
				builder.Append("/>");

				// ending tag:
				if (!_convention.ManyToManyTableName.CreateAsSet)
				{
					builder.Append("\r\n");
					builder.Append("</bag>");
					builder.Append("\r\n");
				}
				else
				{
					builder.Append("\r\n");
					builder.Append("</set>");
					builder.Append("\r\n");
				}
				#endregion
			}
			else
			{
				#region -- many-to-many mapping --
				builder.Append("\r\n");
				builder.Append(string.Format("<!-- '{0}' has many '{1}' and '{1}' has many '{0}' linked by '{2}' --> ",
											 parentEntity.Name, childEntity.Name,
											 BuildJoinTableName(_convention, parentEntity, childEntity)));
				builder.Append("\r\n");

				// start tag:
				if (!_convention.ManyToManyTableName.CreateAsSet)
				{
					builder.Append("<bag");
				}
				else
				{
					builder.Append("<set");
				}

				//attribute: access
				builder.Append(BuildAttribute("access", _convention.MemberAccess.Strategy));

				// attribute: name
				builder.Append(BuildAttribute("name", parentEntityPropertyName));

				//attribute: table
				builder.Append(BuildAttribute("table", BuildJoinTableName(_convention, parentEntity, childEntity)));

				//attribute: inverse
				builder.Append(BuildAttribute("inverse", "false"));

				//attribute: cascade
				builder.Append(BuildAttribute("cascade", "all"));

				//attribute: cascade
				builder.Append(BuildAttribute("generic", isGeneric.ToString().ToLower()));

				//attribute: lazy
				//builder.Append(MappingHelper.BuildAttribute("lazy", visitee.Lazy.ToString().ToLower()));

				builder.Append(" >");

				//attribute: order-by
				//builder.Append(MappingHelper.BuildAttribute("order-by", visitee.Order_By.Strategy));

				// element: key
				builder.Append("<key");
				//attribute:  column
				builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(_convention, parentEntity)));
				//attribute:  foreign-key
				builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(_convention, parentEntity, childEntity)));
				builder.Append("/>");

				// element: many-to-many
				builder.Append("<many-to-many");

				//attribute:  class
				//builder.Append(BuildAttribute("class", childEntity.Name));
				builder.Append(BuildAttribute("class", RelationshipDefinition.CreateQualifiedName(childEntity)));

				//attribute:  column
				builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(_convention, childEntity)));
				builder.Append("/>");

				// ending tag:
				if (!_convention.ManyToManyTableName.CreateAsSet)
				{
					builder.Append("\r\n");
					builder.Append("</bag>");
					builder.Append("\r\n");
				}
				else
				{
					builder.Append("\r\n");
					builder.Append("</set>");
					builder.Append("\r\n");
				}
				#endregion
			}

			return builder.ToString();
		}

	}
}