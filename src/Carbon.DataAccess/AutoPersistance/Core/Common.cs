using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Core
{
    public enum RelationshipDefinitionType
    {
        ManyToMany,
        ManyToOne
    }

    public class ORMUtils
    {
        static ORMUtils()
        { }

        /// <summary>
        /// Helper function used to build a proper xml attribute definition defined as attr_name="attr_value".
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Value of the attribute</param>
        /// <returns></returns>
        public static string BuildAttribute(string name, string value)
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
        public static string Pluralize(string value)
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
        /// This will return the list of all entities in the domain that have been marked for persistance
        /// by a unique identifier as defined in the conventions for auto-persistance.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IList<Type> FindAllEntities(Convention convention, Assembly assembly)
        {
            IList<Type> results = new List<Type>();
            System.Diagnostics.Debug.WriteLine("Searching for domain entities started...");

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsClass & !type.IsAbstract)
                {
                    if (type.Assembly.FullName.ToLower() == assembly.FullName.ToLower())
                    {
                        if (!results.Contains(type) && FindIdentityFieldFor(convention, type) != null)
                        {
                            if (type.BaseType.Assembly.FullName.ToLower() == assembly.FullName.ToLower())
                            {
                                if (FindIdentityFieldFor(convention, type.BaseType) == null)
                                {
                                    results.Add(type);
                                    System.Diagnostics.Debug.WriteLine("Entity Found: " + type.Name);
                                }
                            }
                            else
                            {
                                results.Add(type);
                                System.Diagnostics.Debug.WriteLine("Entity Found: " + type.Name);
                            }
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine("Searching for domain entities completed.");
            return results;
        }

        /// <summary>
        /// This will return the property information for the field used for uniqueness in 
        /// entity persistance as defined in the conventions for auto-persistance.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PropertyInfo FindIdentityFieldFor(Convention convention, Type entity)
        {
            PropertyInfo results = null;

            foreach (PropertyInfo property in entity.GetProperties())
            {
                if (property.Name.ToLower() == convention.PrimaryKey.PrimaryKeyName.ToLower())
                {
                    results = property;
                    break;
                }
            }

            return results;

        }

        /// <summary>
        /// This will determine whether or not the current type for the property is a "value-object"
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entityPropertyType"></param>
        /// <returns></returns>
        public static bool IsComponent(Convention convention, Type entityPropertyType)
        {
            bool retval = false;
            if (entityPropertyType.GetProperty(convention.PrimaryKey.PrimaryKeyName) == null
                & !entityPropertyType.FullName.Contains("System")
                & !entityPropertyType.IsArray
                & entityPropertyType.IsClass)
            {
                retval = true;
            }

            // make sure that the base class for the current declared property is not an entity class:
            if (entityPropertyType.BaseType != null)
            {
                if (entityPropertyType.BaseType.GetProperty(convention.PrimaryKey.PrimaryKeyName) == null)
                {
                    retval = retval && true;
                }
            }

            return retval;
        }
    }
}