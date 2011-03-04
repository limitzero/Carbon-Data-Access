using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// Concrete instance of a relationship that will handle 
    /// the man-to-many relationship between one entity and 
    /// others in the model. 
    /// </summary>
    public class ManyToManyRelationshipStrategy : BaseRelationshipStrategy, IRelationship
    {
        private Convention m_convention = null;
        private Type m_entity = null;

        public ManyToManyRelationshipStrategy(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
        }

        public string Build()
        {
            string results = string.Empty;

            results = BuildAllRelationshipsBasedOnCollectionsFor(m_entity);

            return results;
        }

        private string BuildAllRelationshipsBasedOnCollectionsFor(Type currentEntity)
        {
            StringBuilder results = new StringBuilder();

            foreach (PropertyInfo property in currentEntity.GetProperties())
            {
                Type propertyCollectionType = GetCollectionTypeFor(property.PropertyType);
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

                    results.Append(DefineRelationshipFor(property.Name, currentEntity, propertyCollectionType, isBiDirectional, property.PropertyType.IsGenericType)); 
                }
            }

            return results.ToString();
        }

        private PropertyInfo FindPropertyDefinedByTypeOn(Type entity, Type propertyDefinitionToFind)
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

        private Type GetCollectionTypeFor(Type property)
        {
            Type modelType = null;

            bool isObjectArray = property.FullName.Contains("[]");
            bool isGenericList = property.IsGenericType;

            // exclude the conversion of ValueType? to Nullable<ValueType>
            // which is interpreted as a collection:
            if(property.FullName.StartsWith(typeof(Nullable<>).FullName))
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

        private string DefineRelationshipFor(string parentEntityPropertyName, Type parentEntity, Type childEntity, bool isBiDirectional, bool isGeneric)
        {
            StringBuilder builder = new StringBuilder();

            if (!isBiDirectional)
            {
                #region -- many-to-one mapping (note the parent is not required in this implementation if generics are used)--
                builder.Append(string.Format("<!-- '{0}' has many '{1}' --> ", parentEntity.Name, childEntity.Name));
                builder.Append("\r\n");

                // start tag:
                if (!m_convention.ManyToManyTableName.CreateAsSet)
                {
                    builder.Append("<bag");
                }
                else
                {
                    builder.Append("<set");
                }

                //attribute: access
                builder.Append(BuildAttribute("access", m_convention.MemberAccess.Strategy));

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
                builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(m_convention, parentEntity)));
                //attribute:  foreign-key
                builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(m_convention, parentEntity, childEntity)));
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
                if (!m_convention.ManyToManyTableName.CreateAsSet)
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
                                             BuildJoinTableName(m_convention, parentEntity, childEntity)));
                builder.Append("\r\n");

                // start tag:
                if (!m_convention.ManyToManyTableName.CreateAsSet)
                {
                    builder.Append("<bag");
                }
                else
                {
                    builder.Append("<set");
                }

                //attribute: access
                builder.Append(BuildAttribute("access", m_convention.MemberAccess.Strategy));

                // attribute: name
                builder.Append(BuildAttribute("name", parentEntityPropertyName));

                //attribute: table
                builder.Append(BuildAttribute("table", BuildJoinTableName(m_convention, parentEntity, childEntity)));

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
                builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(m_convention, parentEntity)));
                //attribute:  foreign-key
                builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(m_convention, parentEntity, childEntity)));
                builder.Append("/>");

                // element: many-to-many
                builder.Append("<many-to-many");

                //attribute:  class
                //builder.Append(BuildAttribute("class", childEntity.Name));
				builder.Append(BuildAttribute("class", RelationshipDefinition.CreateQualifiedName(childEntity)));

                //attribute:  column
                builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(m_convention, childEntity)));
                builder.Append("/>");

                // ending tag:
                if (!m_convention.ManyToManyTableName.CreateAsSet)
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