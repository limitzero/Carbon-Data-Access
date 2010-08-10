using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Carbon.Repository.AutoPersistance.Core
{
    public abstract class BaseRelationshipStrategy
    {
        private Hashtable _joinTableNames = new Hashtable();

        /// <summary>
        /// Creates the attribute value for representing information in the mapping document.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string BuildAttribute(string name, string value)
        {
            string retval = string.Empty;

            if (!string.IsNullOrEmpty(value))
                retval = string.Concat(" ", name, "=\"", value, "\"", " ");

            return retval;
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
    }
}