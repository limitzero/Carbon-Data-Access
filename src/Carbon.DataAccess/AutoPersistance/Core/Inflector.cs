using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This will translate the convention to the NHibernate specific verbiage as needed.
    /// </summary>
    public class Inflector
    {
        private IList<Type> _entities = new List<Type>();
        private IList<Type> _baseEntities = new List<Type>();
        private Hashtable _joinTableNames = new Hashtable();

        public Inflector()
            : this(new List<Type>())
        {

        }

        public Inflector(IList<Type> entities)
        {
            _entities = entities;
        }

        /// <summary>
        /// Sets the plural of the word passed using common language conventions.
        /// </summary>
        /// <param name="value">Word to be pluralized</param>
        /// <returns></returns>
        public string Pluralize(string value)
        {
            string retval = string.Empty;

            if (value.EndsWith("ty"))
            {
                retval = value.Substring(0, value.Length - 2);
                retval = retval + "ties";
            }
            else if (value.EndsWith("ex"))
            {
                retval = value.Substring(0, value.Length - 2);
                retval = retval + "ices";
            }
            else if (value.EndsWith("y"))
            {
                retval = value.Substring(0, value.Length - 1);
                retval = retval + "ies";
            }
            else if (value.EndsWith("s"))
            {
                retval = value + "es";
            }
            else
            {
                retval = value + "s";
            }

            return retval;
        }

        /// <summary>
        /// Constructs the name of the entity primary in the associated entity table per convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns>String. Name of the primary key column</returns>
        public string BuildPrimaryKeyColumnName(Convention convention, Type entity)
        {
            string retval = string.Empty;

            if (convention.PrimaryKey.IsEntityNameFollowedByID)
                retval = string.Concat(entity.Name, "ID");

            if (convention.PrimaryKey.IsLowerCaseEntityNameFollowedByID)
                retval = string.Concat(entity.Name.ToLower(), "ID");

            if (convention.PrimaryKey.IsLowerCasePKUnderscoreEntityName)
                retval = string.Concat("pk_", entity.Name);

            if (convention.PrimaryKey.IsLowerCasePKUnderscoreEntityNameUnderscoreID)
                retval = string.Concat("pk_", entity.Name, "_ID");

            return retval;
        }

        /// <summary>
        /// Constructs the name of the foreign key between two entities per convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="parentEntity"></param>
        /// <param name="childEntity"></param>
        /// <returns>String. Name of the foreign key</returns>
        public string BuildForeignKeyName(Convention convention, Type parentEntity, Type childEntity)
        {
            string retval = string.Empty;

            if (convention.ForeignKey.CanRenderAsParentEntityHasInstancesOfChildEntity)
            {
                retval = string.Format("fk_{0}_has_instances_of_{1}", parentEntity.Name, childEntity.Name);
            }
            else if (convention.ForeignKey.CanRenderAsParentEntityHasPluralizedChildEntities)
            {
                retval = string.Format("fk_{0}_has_{1}", parentEntity.Name, Pluralize(childEntity.Name));
            }

            return retval;
        }

        /// <summary>
        /// Constructs the join-table name for a many-to-many relationship for two entities per convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="parentEntity"></param>
        /// <param name="childEntity"></param>
        /// <returns>String: Name of the join table with no duplicates or overlaps</returns>
        public string BuildJoinTableName(Convention convention, Type parentEntity, Type childEntity)
        {
            string retval = string.Empty;
            bool useInverse = false;

            if (parentEntity.Name.ToLower() == childEntity.Name.ToLower())
                return retval;
            

            if (convention.ManyToManyTableName.CreateWithParentEntityNameConcatenatedWithChildEntityNameNotPluralized)
            {
                retval = string.Concat(parentEntity.Name, childEntity.Name);
            }
            else if (convention.ManyToManyTableName.CreateWithParentEntityNameConcatenatedWithChildEntityNamePluralized)
            {
                retval = string.Concat(parentEntity.Name, Pluralize(childEntity.Name));
            }

            // create the dictionary entry of join table names:
            string key = string.Concat(parentEntity.Name, "=", childEntity.Name);

            if (_joinTableNames.Keys.Count == 0)
                _joinTableNames.Add(key, retval);

            foreach (DictionaryEntry de in _joinTableNames)
            {
                if (!de.Key.ToString().Contains(parentEntity.Name) & !de.Key.ToString().Contains(childEntity.Name))
                {
                    try
                    {
                        _joinTableNames.Add(key, retval);
                    }
                    catch { }
                    break;
                }
            }

            if (_joinTableNames.Keys.Count > 0)
            {
                retval = _joinTableNames[key] as string;
                //useInverse = false; // this is the first time the relationship is seen so make this the controller
            }

            if (string.IsNullOrEmpty(retval))
            {
                key = string.Concat(childEntity.Name, "=", parentEntity.Name);
                retval = _joinTableNames[key] as string;
                //useInverse = true; // this is the second time the relationship is seen, make this the passive of the relationship.
            }

            return retval;

        }

        /// <summary>
        /// Constructs the entity definition based on the current convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildEntityByConvention(Convention convention, Type entity)
        {
            StringBuilder document = new StringBuilder();
 
            if (IsComponent(convention, entity))
                return string.Empty;

            document.Append("<class");
            document.Append(BuildAttribute("name", entity.Name));

            if (convention.CanPluralizeTableNames)
                document.Append(BuildAttribute("table", Pluralize(entity.Name)));

            //document.Append(MappingHelper.BuildAttribute("where", visitee.FilterCondition));

            document.Append(">");
            document.Append("\r\n");
            document.Append(BuildPrimaryKeyForEntityByConvention(convention, entity));
            document.Append("\r\n");
            document.Append(BuildPropertiesForEntityByConvention(convention, entity));
            document.Append("\r\n");
            document.Append(BuildComponentForEntityByConvention(convention, entity));
            document.Append("\r\n");
            document.Append(BuildEntityRelationshipByConvention(convention, entity));
            document.Append("\r\n");

            //prepare sub-class definition:
            foreach (Type subclass in FindImplementorsOfBaseEntity(_entities, entity))
            {
                document.Append(BuildSubClassForEntityByConvention(convention, subclass, entity));
            }

            document.Append("\r\n");
            document.Append("</class>");
            document.Append("\r\n");
            document.Append("\r\n");

            return document.ToString();

        }

        /// <summary>
        /// Constructs the primary key column for the entity in the repository based on convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildPrimaryKeyForEntityByConvention(Convention convention, Type entity)
        {
            string retval = string.Empty;
            StringBuilder builder = new StringBuilder();

            if (entity.GetProperty(convention.PrimaryKey.PrimaryKeyName) != null)
            {
                builder.Append("<id");
                builder.Append(BuildAttribute("name", convention.PrimaryKey.PrimaryKeyName));
                builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, entity)));
                builder.Append(BuildAttribute("type", entity.GetProperty(convention.PrimaryKey.PrimaryKeyName).PropertyType.Name));
                builder.Append(BuildAttribute("access", convention.PrimaryKey.MemberAccess.Strategy));
                builder.Append(">").Append("\r\n");
                builder.Append("<generator class=\"identity\"/>");
                builder.Append("\r\n");
                builder.Append("</id>").Append("\r\n");
            }

            retval = builder.ToString();
            return retval;

        }

        /// <summary>
        /// Constructs the entity properties definition for persistance based on convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildPropertiesForEntityByConvention(Convention convention, Type entity)
        {
            string retval = string.Empty;
            IList<PropertyInfo> _properties = new List<PropertyInfo>();

            PropertyInfo[] properties = entity.GetProperties();
            Type baseType = entity.BaseType;

            foreach (PropertyInfo property in properties)
            {
                // only list properties of the current instance, not the inherited properties:    
                if (baseType != null)
                {
                    PropertyInfo baseProp = baseType.GetProperty(property.Name);
                    if (baseProp == null)
                    {
                        _properties.Add(property);
                    }
                }
                else
                {
                    _properties.Add(property);
                }

            }

            StringBuilder builder = new StringBuilder();
            if (_properties.Count > 0)
            {
                builder.Append(string.Format("<!-- properties for entity '{0}' --> ", entity.Name));
                builder.Append("\r\n");

                foreach (PropertyInfo property in _properties)
                {
                    // only render properties that do not implement any collection interfaces (these will indicate relationship bindings):
                    bool renderProperty = (!property.PropertyType.FullName.Contains("Collections") &
                                           !property.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) &
                                           !IsComponent(convention, property.PropertyType) &
                                           !_entities.Contains(property.PropertyType) &
                                           !property.PropertyType.IsArray);
 
                    if(property.PropertyType.BaseType != null)
                    {
                        if (property.PropertyType.BaseType.FullName.StartsWith(entity.Namespace))
                            renderProperty = renderProperty & false;
                    }

                    if (renderProperty)
                    {
                        builder.Append("<property");

                        //attribute: name:
                        builder.Append(BuildAttribute("name", property.Name));

                        //attribute: column
                        if (convention.Property.CanRenderAsLowerCase)
                            builder.Append(BuildAttribute("column", property.Name.ToLower()));

                        //attribute: type
                        if (property.PropertyType.FullName.Contains("System"))
                        {
                            builder.Append(BuildAttribute("type", property.PropertyType.Name));
                        }
                        else
                        {
                            string typeName = string.Concat(property.PropertyType.FullName, ", ", property.DeclaringType.Assembly.GetName().Name);
                            builder.Append(BuildAttribute("type", typeName));
                        }

                        //attribute: length:
                        if (convention.Property.LargeTextFieldNames.Contains(property.Name))
                        {
                            builder.Append(BuildAttribute("length", convention.Property.LargeTextFieldLength.ToString()));
                        }
                        else
                        {
                            builder.Append(BuildAttribute("length", convention.Property.DefaultTextFieldLength.ToString()));
                        }

                        //attribute: access
                        if (!string.IsNullOrEmpty(convention.MemberAccess.Strategy))
                        {
                            builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));
                        }

                        builder.Append("/>");
                        builder.Append("\r\n");
                    }
                }
            }

            retval = builder.ToString();

            return retval;
        }


        /// <summary>
        /// Constructs the relationships for the entities based convention
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildEntityRelationshipByConvention(Convention convention, Type entity)
        {
            string results = string.Empty;

            ManyToOneRelationshipStrategy manyToOneStrategy = new ManyToOneRelationshipStrategy(convention, entity);
            results += manyToOneStrategy.Build();

            ManyToManyRelationshipStrategy manyToManyStrategy = new ManyToManyRelationshipStrategy(convention, entity);
            results += manyToManyStrategy.Build();

            return results;

        }

        /// <summary>
        /// Constructs the relationships for the entities based convention
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildEntityRelationshipByConventionEx(Convention convention, Type entity)
        {
          
            string retval = string.Empty;
            string parentPropertyName = string.Empty;
            bool isManyToManyRelationship = false;
            bool isGeneric = false;
            bool isInverse = false;
            Type childEntity = null;
            string entityPropertyName = string.Empty;
            StringBuilder builder = new StringBuilder();
            IList<PropertyInfo> _properties = new List<PropertyInfo>();

            PropertyInfo[] properties = entity.GetProperties();
            _properties = new List<PropertyInfo>(properties);

            Type baseType = entity.BaseType;
            #region -- load properties from parent entity --
            /*
            foreach (PropertyInfo property in properties)
            {
                // only list properties of the current instance, not the inherited properties:    
                if (baseType != null)
                {
                    PropertyInfo baseProp = baseType.GetProperty(property.Name);
                    if (baseProp == null)
                    {
                        _properties.Add(property);
                    }
                }
                else
                {
                    _properties.Add(property);
                }

            }
            */
            #endregion

            /*
            if (IsOneToOne(convention, _properties, entity, out childEntity, out entityPropertyName))
            {
                #region -- one-to-one mapping --
                builder.Append(string.Format("<!-- '{0}' references '{1}' --> ", entity.Name, childEntity.Name));
                builder.Append("\r\n");
                builder.Append("<one-to-one");
                builder.Append(BuildAttribute("name", entityPropertyName));
                builder.Append(BuildAttribute("class", childEntity.Name));
                builder.Append(BuildAttribute("fetch", "join"));
                builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));
                builder.Append("/>");
                builder.Append("\r\n");
                #endregion 
            }
            */
            foreach (PropertyInfo property in properties)
            {
                RelationshipDefinition definition = IsManyTo(convention, property, entity);
                if (definition.ReferencedEntity != null)
                {
                    if (definition.RelationshipType == RelationshipDefinitionType.ManyToOne)
                    {
                        #region -- many-to-one mapping (note the parent is not required in this implementation if generics are used)--
                        builder.Append(string.Format("<!-- '{0}' has many '{1}' --> ", entity.Name, definition.ReferencedEntity.Name));
                        builder.Append("\r\n");

                        // start tag:
                        if (!convention.ManyToManyTableName.CreateAsSet)
                        {
                            builder.Append("<bag");
                        }
                        else
                        {
                            builder.Append("<set");
                        }

                        //attribute: access
                        builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));

                        // attribute: name
                        builder.Append(BuildAttribute("name", definition.PropertyName));

                        //attribute: table
                        //builder.Append(BuildAttribute("table", BuildJoinTableName(convention, entity, definition.ReferencedEntity)));

                        //attribute: inverse
                        builder.Append(BuildAttribute("inverse", definition.IsInverse.ToString().ToLower()));

                        //attribute: cascade
                        builder.Append(BuildAttribute("cascade", "all"));

                        //attribute: cascade
                        builder.Append(BuildAttribute("generic", definition.IsGeneric.ToString().ToLower()));

                        //attribute: lazy
                        //builder.Append(MappingHelper.BuildAttribute("lazy", visitee.Lazy.ToString().ToLower()));

                        builder.Append(" >");

                        //attribute: order-by
                        //builder.Append(MappingHelper.BuildAttribute("order-by", visitee.Order_By.Strategy));

                        // element: key
                        builder.Append("<key");
                        //attribute:  column
                        builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, entity)));
                        //attribute:  foreign-key
                        builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, entity, definition.ReferencedEntity)));
                        builder.Append("/>");

                        // element: one-to-many
                        builder.Append("<one-to-many");
                        //attribute:  class
                        builder.Append(BuildAttribute("class", definition.QualifiedName));
                        //attribute:  column
                        //builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, definition.ReferencedEntity)));
                        builder.Append("/>");

                        // ending tag:
                        if (!convention.ManyToManyTableName.CreateAsSet)
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

                        #region -- old code for many-to-one (parent required on entity without collection) -- 
                        /*
                        //element: many-to-one
                        builder.Append("<many-to-one ");

                        // attribute: class
                        builder.Append(BuildAttribute("class", definition.ReferencedEntity.Name));

                        // attribute: name
                        builder.Append(BuildAttribute("name", definition.PropertyName));

                        // attribute: cascade
                        builder.Append(BuildAttribute("cascade", "save-update"));

                        //attribute: not-found
                        //builder.Append(BuildAttribute("not-found", "ignore"));

                        // attribute: column
                        builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, definition.ReferencedEntity)));

                        // attribute: not-null
                        builder.Append(BuildAttribute("not-null", definition.IsNull.ToString().ToLower()));

                        // attribute: access
                        builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));

                        // attribute: foreign-key
                        builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, entity, definition.ReferencedEntity)));

                        // attribute: fetch
                        builder.Append(BuildAttribute("fetch", "join"));

                        builder.Append("/>");
                        */
                        #endregion 

                    }
                    else
                    {
                        #region -- many-to-many mapping --
                        builder.Append("\r\n");
                        builder.Append(string.Format("<!-- '{0}' has many '{1}' and '{1}' has many '{0}' linked by '{2}' --> ",
                                                     entity.Name, definition.ReferencedEntity.Name,
                                                     BuildJoinTableName(convention, entity, definition.ReferencedEntity)));
                        builder.Append("\r\n");

                        // start tag:
                        if (!convention.ManyToManyTableName.CreateAsSet)
                        {
                            builder.Append("<bag");
                        }
                        else
                        {
                            builder.Append("<set");
                        }

                        //attribute: access
                        builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));

                        // attribute: name
                        builder.Append(BuildAttribute("name", definition.PropertyName));

                        //attribute: table
                        builder.Append(BuildAttribute("table", BuildJoinTableName(convention, entity, definition.ReferencedEntity)));

                        //attribute: inverse
                        builder.Append(BuildAttribute("inverse", definition.IsInverse.ToString().ToLower()));

                        //attribute: cascade
                        builder.Append(BuildAttribute("cascade", "all"));

                        //attribute: cascade
                        builder.Append(BuildAttribute("generic", definition.IsGeneric.ToString().ToLower()));

                        //attribute: lazy
                        //builder.Append(MappingHelper.BuildAttribute("lazy", visitee.Lazy.ToString().ToLower()));

                        builder.Append(" >");

                        //attribute: order-by
                        //builder.Append(MappingHelper.BuildAttribute("order-by", visitee.Order_By.Strategy));

                        // element: key
                        builder.Append("<key");
                        //attribute:  column
                        builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, entity)));
                        //attribute:  foreign-key
                        builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, entity, definition.ReferencedEntity)));
                        builder.Append("/>");

                        // element: many-to-many
                        builder.Append("<many-to-many");
                        //attribute:  class
                        //builder.Append(BuildAttribute("class", definition.ReferencedEntity.Name));
						builder.Append(BuildAttribute("class", definition.QualifiedName));

                        //attribute:  column
                        builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, definition.ReferencedEntity)));
                        builder.Append("/>");

                        // ending tag:
                        if (!convention.ManyToManyTableName.CreateAsSet)
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
                }
            }

            #region -- old code -- 
            /*
            foreach (PropertyInfo parentProperty in _properties)
            {
                isManyToManyRelationship = false;
                childEntity = null;
                parentPropertyName = string.Empty;

                // only render properties that implement any collection/array type definitions (these will indicate relationship bindings):
                if (!parentProperty.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName))
                {
                    //ex: IList<ChildEntity> PropertyName
                    if (parentProperty.PropertyType.IsGenericType)
                    {
                        parentPropertyName = parentProperty.Name;
                        isGeneric = true; 

                        #region -- check to see if child entity has a reference to the parent (entity) with the parent entity's type as a declaration --
                        //extract out the inner type from the mangled version arguments:
                        childEntity = parentProperty.PropertyType.GetGenericArguments()[0];

                        //scan the current entity for the childEntity type (many-to-many)
                        foreach (PropertyInfo childProperty in childEntity.GetProperties())
                        {
                            if (!childProperty.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) & childProperty.PropertyType.IsGenericType)
                            {
                                if (childProperty.PropertyType.GetGenericArguments()[0] == entity
                                    && !IsComponent(convention, childProperty.PropertyType.GetGenericArguments()[0]))
                                {
                                    isManyToManyRelationship = true;     
                                }
                            }
                        }
                        #endregion
                    }
                    //ex: ChildEntity[] PropertyName
                    else if (parentProperty.PropertyType.IsArray)
                    {
                        #region -- check to see if the parent entity properties can be examined to determine the type for reference to child entity --
                        if (parentProperty.PropertyType.FullName.Contains("[]"))
                        {
                            parentPropertyName = parentProperty.Name;

                            //extract the name of the object based on the array definition:
                            string[] parts = parentProperty.PropertyType.FullName.Split(new char[] { '.' });
                            Array.Reverse(parts);
                            string parentEntityName = parts[0].Replace("[]", string.Empty);
                            isManyToManyRelationship = false;

                            //find the child entity from the types in the assembly:
                            childEntity = null;
                            foreach (Type type in parentProperty.PropertyType.Assembly.GetTypes())
                            {
                                if (type.Name == parentEntityName && !IsComponent(convention, type))
                                {
                                    childEntity = type;
                                    break;
                                }
                            }

                            //scan the current entity for the childEntity type (many-to-many):
                            foreach (PropertyInfo childProperty in childEntity.GetProperties())
                            {
                                if (!childProperty.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName))
                                {
                                    if (childProperty.PropertyType.FullName.Contains("[]"))
                                    {
                                        //extract the name of the object based on the array definition:
                                        parts = parentProperty.PropertyType.FullName.Split(new char[] { '.' });
                                        Array.Reverse(parts);
                                        string childEntityName = parts[0].Replace("[]", string.Empty);

                                        if (childEntityName == parentEntityName)
                                        {
                                            isManyToManyRelationship = true;
                                        }

                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    #region -- build the relationship ---
                    if (childEntity != null)
                    {
                        
                        if (isManyToManyRelationship)
                        {
                            #region -- many-to-many mapping --
                            builder.Append("\r\n");
                            builder.Append(string.Format("<!-- '{0}' has many '{1}' and '{1}' has many '{0}' linked by '{2}' --> ",
                                entity.Name, childEntity.Name,
                                BuildJoinTableName(convention, entity, childEntity, out isInverse)));
                            builder.Append("\r\n");

                            // start tag:
                            if (!convention.ManyToManyTableName.CreateAsSet)
                            {
                                builder.Append("<bag");
                            }
                            else
                            {
                                builder.Append("<set");
                            }

                            //attribute: access
                            builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));

                            // attribute: name
                            builder.Append(BuildAttribute("name", parentPropertyName));

                            //attribute: table
                            builder.Append(BuildAttribute("table", BuildJoinTableName(convention, entity, childEntity, out isInverse)));

                            //attribute: inverse
                            builder.Append(BuildAttribute("inverse", isInverse.ToString().ToLower()));

                            //attribute: cascade
                            builder.Append(BuildAttribute("cascade", "save-update"));

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
                            builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, entity)));
                            //attribute:  foreign-key
                            builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, entity, childEntity)));
                            builder.Append("/>");

                            // element: many-to-many
                            builder.Append("<many-to-many");
                            //attribute:  class
                            builder.Append(BuildAttribute("class", childEntity.Name));
                            //attribute:  column
                            builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, childEntity)));
                            builder.Append("/>");

                            // ending tag:
                            if (!convention.ManyToManyTableName.CreateAsSet)
                            {
                                builder.Append("\r\n");
                                builder.Append("</bag>");
                            }
                            else
                            {
                                builder.Append("\r\n");
                                builder.Append("</set>");
                            }
                            isManyToManyRelationship = false;
                            #endregion
                        }
                        else
                        {
                            #region -- many-to-one mapping --
                            builder.Append(string.Format("<!-- '{0}' has many '{1}' --> ", entity.Name, childEntity.Name));
                            builder.Append("\r\n");

                            //element: many-to-one
                            builder.Append("<many-to-one ");

                            // attribute: class
                            builder.Append(BuildAttribute("class", childEntity.Name));

                            // attribute: name
                            builder.Append(BuildAttribute("name", parentProperty.Name));

                            // attribute: cascade
                            builder.Append(BuildAttribute("cascade", "save-update"));

                            //attribute: not-found
                            builder.Append(BuildAttribute("not-found", "ignore"));

                            // attribute: column
                            builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, childEntity)));

                            // attribute: not-null
                            //builder.Append(BuildAttribute("not-null", visitee.Nillable.Nullable.ToString().ToLower()));

                            // attribute: access
                            builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));

                            // attribute: foreign-key
                            builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, entity, childEntity)));

                            // attribute: fetch
                            builder.Append(BuildAttribute("fetch", "join"));

                            builder.Append("/>");

                            #endregion
                        }
                        builder.Append("\r\n");
                    }
                    #endregion
                }

            }
            */
            #endregion 

            retval = builder.ToString();
            return retval;
        }

        /// <summary>
        /// Constructs the component definition for entities that use value-objects per convention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildComponentForEntityByConvention(Convention convention, Type entity)
        {
            string retval = string.Empty;
            StringBuilder builder = new StringBuilder();

            IList<PropertyInfo> properties = GetEntityProperties(entity);

            foreach (PropertyInfo property in properties)
            {
                if (IsComponent(convention, property.PropertyType))
                {
                    builder.Append(string.Format("<!-- '{0}' has component/value-object class of  '{1}' realized as '{2}' --> ", entity.Name, property.PropertyType.Name, property.Name));
                    builder.Append("\r\n");
                    builder.Append("<component");
                    builder.Append(BuildAttribute("name", property.Name));
                    builder.Append(BuildAttribute("class", property.PropertyType.Name));
                    builder.Append(BuildAttribute("access", convention.MemberAccess.Strategy));
                    builder.Append(">");
                    builder.Append("\r\n");
                    builder.Append(BuildPropertiesForEntityByConvention(convention, property.PropertyType));
                    builder.Append("</component>");
                    builder.Append("\r\n");
                }
            }

            retval = builder.ToString();
            return retval;

        }

        /// <summary>
        /// Constructs the inheritance definition for base classes and users of the base class per convention. This implements the table-per-subclass hierarchy per NHibernate documention.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string BuildSubClassForEntityByConvention(Convention convention, Type entity, Type parentEntity)
        {
            string retval = string.Empty;
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("<!-- {0} 'is a' {1} -->", entity.Name, parentEntity.Name));
            builder.Append("\r\n");
            builder.Append("<joined-subclass");
            string typeName = string.Concat(entity.FullName, ", ", entity.Assembly.GetName().Name);
            builder.Append(BuildAttribute("name", typeName));
            builder.Append(BuildAttribute("table", this.Pluralize(entity.Name)));
            builder.Append(BuildAttribute("extends", parentEntity.Name));
            builder.Append(">");
            builder.Append("\r\n");
            builder.Append("<key");
            builder.Append(BuildAttribute("column", BuildPrimaryKeyColumnName(convention, parentEntity)));
            builder.Append(BuildAttribute("foreign-key", BuildForeignKeyName(convention, parentEntity, entity)));
            builder.Append("/>");
            builder.Append("\r\n");
            builder.Append(BuildPropertiesForEntityByConvention(convention, entity));
            builder.Append("\r\n");
            builder.Append(BuildComponentForEntityByConvention(convention, entity));
            builder.Append("\r\n");
            builder.Append(BuildEntityRelationshipByConvention(convention, entity));
            builder.Append("\r\n");
            builder.Append("</joined-subclass>");

            retval = builder.ToString();
            return retval;
        }

        private IList<PropertyInfo> GetEntityProperties(Type entity)
        {
            IList<PropertyInfo> retval = new List<PropertyInfo>();
            Type baseType = null;

            foreach (PropertyInfo property in entity.GetProperties())
            {
                // only list properties of the current instance, not the inherited properties:    
                if (baseType != null)
                {
                    PropertyInfo baseProp = baseType.GetProperty(property.Name);
                    if (baseProp == null)
                    {
                        retval.Add(property);
                    }
                }
                else
                {
                    retval.Add(property);
                }

            }

            return retval;
        }

        private bool IsComponent(Convention convention, Type entity)
        {
            bool retval = false;
            if (entity.GetProperty(convention.PrimaryKey.PrimaryKeyName) == null
                & !entity.FullName.Contains("System") 
                & !entity.FullName.Contains("[]")
                & entity.IsClass)
            {
                retval = true;
            }

            // make sure that the base class for the current declared property is not an entity class:
            if (entity.BaseType != null)
            {
                if (entity.BaseType.GetProperty(convention.PrimaryKey.PrimaryKeyName) == null)
                {
                    retval = retval && true;
                }
            }

            return retval;
        }

        private bool IsSubClass(Convention convention, Type entity)
        {
            bool retval = false;
            if (entity.BaseType != null)
            {
                if (entity.BaseType.GetProperty(convention.PrimaryKey.PrimaryKeyName) != null)
                    retval = true;
            }
            return retval;
        }

        private IList<Type> FindBaseEntities(IList<Type> entities)
        {
            IList<Type> retval = new List<Type>();
            foreach (Type type in entities)
            {
                if (entities.Contains(type.BaseType))
                {
                    if (!retval.Contains(type.BaseType))
                        retval.Add(type.BaseType);
                    System.Diagnostics.Debug.WriteLine("Entity " + type.FullName + " implements base class of " + type.BaseType.FullName);
                }
            }
            return retval;
        }

        private IList<Type> FindImplementorsOfBaseEntity(IList<Type> entities, Type baseEntity)
        {
            IList<Type> retval = new List<Type>();
            foreach (Type type in entities)
            {
                if (type.BaseType == baseEntity)
                {
                    retval.Add(type);
                }
            }
            return retval;
        }

        private string BuildAttribute(string name, string value)
        {
            string retval = string.Empty;

            if (!string.IsNullOrEmpty(value))
                retval = string.Concat(" ", name, "=\"", value, "\"", " ");

            return retval;
        }

        /// <summary>
        /// Scans the current assembly and retrieves the type by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assemblyToScan"></param>
        /// <returns></returns>
        private Type GetTypeForName(string name, Assembly assemblyToScan)
        {
            Type retval = null;
            foreach (Type type in assemblyToScan.GetTypes())
            {
                if (type.Name == name)
                {
                    retval = type;
                    break;
                }
            }

            return retval;
        }

        private bool IsOneToOne(Convention convention, IList<PropertyInfo> properties, Type entity, out Type childEntity, out string entityPropertyName)
        {
            bool hasPrimaryKey = false;
            bool hasCollectionProperties = false;
            childEntity = null;
            entityPropertyName = string.Empty; 

            foreach (PropertyInfo property in properties)
            {
                if (property.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName))
                {
                    hasPrimaryKey = true;
                    break;
                }
            }

            if (hasPrimaryKey)
            {
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType.IsGenericType)
                    {
                        hasCollectionProperties = true;
                        entityPropertyName = property.Name;
                        childEntity = property.PropertyType.GetGenericArguments()[0];
                        break;
                    }
                    else if (property.PropertyType.FullName.Contains("[]"))
                    {
                        string parentPropertyName = property.Name;

                        //extract the name of the object based on the array definition:
                        string[] parts = property.PropertyType.FullName.Split(new char[] { '.' });
                        Array.Reverse(parts);
                        string parentEntityName = parts[0].Replace("[]", string.Empty);

                        //find the child entity from the types in the assembly:
                        childEntity = null;
                        for (int i = 0; i < property.PropertyType.Assembly.GetTypes().Length; i++)
                        {
                            Type type = property.PropertyType.Assembly.GetTypes()[i];
                            if (type.Name == parentEntityName && !IsComponent(convention, type))
                            {
                                childEntity = type;
                                hasCollectionProperties = true;
                                entityPropertyName = property.Name;
                                i = property.PropertyType.Assembly.GetTypes().Length + 1;
                                break;
                            }
                        }
                    }

                }
            }

            // need to find the referenced entity if no collection properties are found:
            if (childEntity == null & !hasCollectionProperties)
            {
                foreach (PropertyInfo property in properties)
                {
                    entityPropertyName = property.Name;

                    //cycle through types in the assebly to see if one matches the current property type and namespace:
                    for (int index = 0; index < entity.Assembly.GetTypes().Length & childEntity == null; index++)
                    {
                        Type type = entity.Assembly.GetTypes()[index];
                        if (type == property.PropertyType & 
                            property.PropertyType.IsClass &
                            !property.PropertyType.IsEnum &
                            type.Namespace == property.PropertyType.Namespace)
                        {
                            childEntity = type;
                            //index = entity.Assembly.GetTypes().Length + 1;
                        }
                    }

                    if (childEntity != null)
                        break;
                }
            }

            return hasPrimaryKey & !hasCollectionProperties & childEntity != null;
        }

        private RelationshipDefinition IsManyTo(Convention convention, PropertyInfo property, Type entity)
        {
            RelationshipDefinition definition = new RelationshipDefinition();
            definition.RelationshipType = RelationshipDefinitionType.ManyToOne; // default

            #region -- check for generic property list types (ex: IList<Type> PropertyName) --
            if (!property.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) & 
                property.PropertyType.IsGenericType & 
                definition.RelationshipType == RelationshipDefinitionType.ManyToOne)
            {
                //extract the first arguement from the generic list (default convention);
                definition.PropertyName = property.Name;
                definition.IsGeneric = true;
                definition.ReferencedEntity = property.PropertyType.GetGenericArguments()[0];

                //look on the referenced entity for the current property type and collection instance:
                for (int i = 0; i < definition.ReferencedEntity.GetProperties().Length & definition.RelationshipType== RelationshipDefinitionType.ManyToOne; i++)
                {
                    PropertyInfo childProperty = definition.ReferencedEntity.GetProperties()[i];
                    if (!childProperty.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) 
                        & childProperty.PropertyType.IsGenericType)
                    {
                        //both sides have a collection reference, this is many-to-many:
                        if (childProperty.PropertyType.GetGenericArguments()[0] == entity
                            && !IsComponent(convention, childProperty.PropertyType.GetGenericArguments()[0]))
                        {
                            definition.RelationshipType = RelationshipDefinitionType.ManyToMany;
                        }

                        // search for parent instance on the child entity:
                        if (childProperty.PropertyType == entity)
                        {
                            definition.IsNull = false;
                            definition.IsInverse = true;
                        }
                    }
                }
            }
            #endregion

            #region -- check for strongly-typed collections (ex: Order[] Orders)
            if (!property.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) & 
                property.PropertyType.FullName.Contains("[]") &
                definition.RelationshipType == RelationshipDefinitionType.ManyToOne)
            {

                definition.PropertyName = property.Name;
                definition.IsGeneric = false;

                //extract the name of the object based on the array definition:
                string[] parts = property.PropertyType.FullName.Split(new char[] { '.' });
                Array.Reverse(parts);
                string parentEntityName = parts[0].Replace("[]", string.Empty);
                
                //find the child entity from the types in the assembly:
                for (int i = 0; i < property.PropertyType.Assembly.GetTypes().Length; i++)
                {
                    Type type = property.PropertyType.Assembly.GetTypes()[i];
                    if (type.Name == parentEntityName && !IsComponent(convention, type))
                    {
                        definition.ReferencedEntity = type;
                        i = property.PropertyType.Assembly.GetTypes().Length + 1;
                    }
                }

                if (definition.ReferencedEntity != null)
                {
                    for (int index = 0; index < definition.ReferencedEntity.GetProperties().Length & definition.RelationshipType == RelationshipDefinitionType.ManyToOne; index++)
                    {
                        PropertyInfo childProperty = definition.ReferencedEntity.GetProperties()[index];

                        if (!childProperty.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName))
                        {
                            if (childProperty.PropertyType.FullName.Contains("[]"))
                            {
                                //extract the name of the object based on the array definition:
                                parts = property.PropertyType.FullName.Split(new char[] { '.' });
                                Array.Reverse(parts);
                                string childEntityName = parts[0].Replace("[]", string.Empty);

                                if (childEntityName == parentEntityName)
                                {
                                    definition.RelationshipType = RelationshipDefinitionType.ManyToMany;
                                }

                            }

                            // search for parent instance on the child entity:
                            if (childProperty.PropertyType == entity)
                            {
                                definition.IsNull = false;
                                definition.IsInverse = true;
                            }
                        }
                    }
                }

            }
            #endregion 
    
            #region -- check for object collections (ex: IList, ISet)
            /*
            if (!property.Name.StartsWith(convention.PrimaryKey.PrimaryKeyName) &
                property.PropertyType.FullName.Contains("Collections") &
                !property.PropertyType.FullName.Contains("Generic"))
            {
                definition.RelationshipType = RelationshipDefinitionType.ManyToOne;
                definition.PropertyName = property.Name;
            }
             */ 
            #endregion 

            return definition;
        }

        private Type FindTypeForField(Convention convention, Type entity, string propertyName)
        {
            return null;
        }
    }

    public class RelationshipDefinition
    {
    	private string _qualifiedName;

        public RelationshipDefinition()
        {

        }

        private RelationshipDefinitionType _relationshipType;
        public RelationshipDefinitionType RelationshipType
        {
            get { return _relationshipType; }
            set { _relationshipType = value; }
        }
	
        private bool _isGeneric = false;
        public bool IsGeneric
        {
            get { return _isGeneric; }
            set { _isGeneric = value; }
        }

        private bool _isnull = true;
        public bool IsNull
        {
            get { return _isnull; }
            set { _isnull = value; }
        }

        private bool _isinverse = false;
        public bool IsInverse
        {
            get { return _isinverse; }
            set { _isinverse = value; }
        }

        private string _propertyname;
        public string PropertyName
        {
            get { return _propertyname; }
            set { _propertyname = value; }
        }

        private Type _referencedentity;
        public Type ReferencedEntity
        {
            get { return _referencedentity; }
            set
			{
				_referencedentity = value;
				this.MakeQualifiedName(value);
			}
        }

    	public string QualifiedName
    	{
			get { return _qualifiedName; }
    	}

		private string MakeQualifiedName(Type entity)
		{
			return CreateQualifiedName(entity);
		}

		public static string CreateQualifiedName(Type entity)
		{
			var parts = entity.AssemblyQualifiedName.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
			return string.Concat(entity.FullName, ", ", parts[1].Trim());
		}
    }
}