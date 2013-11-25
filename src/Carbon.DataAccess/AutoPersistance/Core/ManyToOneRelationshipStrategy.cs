using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace NHibernate.Carbon.AutoPersistance.Core
{
	/// <summary>
	/// Strategy to realize the many to one realization of an parent entity to child entity.
	/// </summary>
	[Optimization("For many to one associations on entities, the parent will be assumed to be present on child object for association (not-null = 'true').")]
    public class ManyToOneRelationshipStrategy : BaseRelationshipStrategy, IRelationship
    {
        private readonly ModelConvention _convention;
        private readonly System.Type _entity;

        public ManyToOneRelationshipStrategy(ModelConvention convention, System.Type entity)
        {
            _convention = convention;
            _entity = entity;
        }

        public string Build()
        {
            string results = string.Empty;

			// find all of the entities defined on the current domain model entity (i.e. associations):
        	IList<System.Type> entityAssociations = this.GetModelEntitiesDefinedOnCurrentEntity(_entity,
        	                                                                                       ORMUtils.FindAllEntities
        	                                                                                       	(_convention,
        	                                                                                       	 _entity.Assembly));

            foreach (System.Type entityAssociation in entityAssociations)
            {
                results += GetManyToOneRelationshipDefinitonFor(entityAssociation, _entity);
            }

            return results;
        }

		private IList<System.Type> GetModelEntitiesDefinedOnCurrentEntity(System.Type entity, IList<System.Type> modelEntities)
		{
			IList<System.Type> entities = new List<System.Type>();

			// look on the current entity and find all possible entities that are defined as properties:
			var properties = entity.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				// exclude collections and arrays in this relationship binding (left for many-to-many binding):
				if(property.PropertyType.IsGenericType == true || property.PropertyType.Name.Contains("[]") == true) continue;

				if (modelEntities.Contains(property.PropertyType) &&
					ORMUtils.FindIdentityFieldFor(_convention, property.PropertyType) != null)
				{
					if (entities.Contains(property.PropertyType) == false)
						entities.Add(property.PropertyType);
				}
			}

			return entities;
		}

        private string GetManyToOneRelationshipDefinitonFor(System.Type parentEntity, System.Type entity)
        {
            StringBuilder results = new StringBuilder();

            IList<PropertyInfo> parentProperties = GetPropertiesDefinedAsModelEntitiesFor(entity);
            PropertyInfo parentProperty = null;

            //scan all of the properties on the "many" side of the relationship looking for 
            //a property defined as the parent type from the entity model:
            foreach (PropertyInfo property in parentProperties)
            {
                if (property.PropertyType == parentEntity)
                {
                    parentProperty = property;
                    break;
                }
            }

            results.Append(string.Format("<!-- many \"{0}\" are associated with one instance of a \"{1}\" -->", entity.Name, parentEntity.Name));
            results.Append("\r\n");

			// OPTIMIZATION: parent will always be present on child for association (not-null = "true")
            results.Append(string.Format("<many-to-one name=\"{0}\" class=\"{1}\" cascade=\"all\" access=\"{2}\"  column=\"{3}\" foreign-key=\"{4}\" fetch=\"join\" not-found=\"ignore\"  not-null=\"true\"/>",
                                         parentProperty.Name,
										 RelationshipDefinition.CreateQualifiedName(parentEntity),
                                         _convention.MemberAccess.Strategy,
                                         base.BuildPrimaryKeyColumnName(_convention, parentEntity),
                                         base.BuildForeignKeyName(_convention, parentEntity, entity)));
            results.Append("\r\n");

            return results.ToString();
        }

        private IList<PropertyInfo> GetPropertiesDefinedAsModelEntitiesFor(System.Type entity)
        {
            IList<PropertyInfo> results = new List<PropertyInfo>();
            IList<System.Type> modelTypes = GetModelEntitiesAsTypes(entity.Assembly); 

            foreach (PropertyInfo property in _entity.GetProperties())
            {
                if (modelTypes.Contains(property.PropertyType))
                {
                    results.Add(property);
                }
            }

            return results;
        }

        private IList<System.Type> GetModelEntitiesAsTypes(Assembly asm)
        {
            IList<System.Type> results = new List<System.Type>();
            foreach (System.Type type in asm.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract)
                    results.Add(type);
            }

            return results;
        }


    }
}