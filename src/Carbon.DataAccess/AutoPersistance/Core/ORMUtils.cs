using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NHibernate.Carbon.AutoPersistance.Core
{
	public class ORMUtils
    {
		private static  IList<System.Type> _entities;

        static ORMUtils()
        {
        	if(_entities == null)
        	{
				_entities = new List<System.Type>();
        	}
        }

		~ORMUtils()
		{
			if (_entities != null)
			{
				_entities.Clear();
			}
			_entities = null;

		}

		public static string Serialize<T>(T entity) where T : class
		{
			string results = string.Empty;
			XmlSerializer serializer = new XmlSerializer(typeof(T));

			using (MemoryStream memoryStream = new MemoryStream())
			{
				serializer.Serialize(memoryStream, entity);
				memoryStream.Seek(0, SeekOrigin.Begin);
				results = ASCIIEncoding.ASCII.GetString(memoryStream.ToArray());
			}

			results = results.Replace("<?xml version=\"1.0\"?>", string.Empty);
			results = results.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
			results = results.Replace(System.Environment.NewLine, string.Empty);
			results = results.Replace("&lt;", "<");
			results = results.Replace("&gt;", ">");

			return results;
		}

		public static string FormatXMLDocument(string xmlContent)
		{
			// properly indent the xml contents:
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlContent);

			StringWriter sw = new StringWriter();
			XmlTextWriter xtw = new XmlTextWriter(sw);
			xtw.Formatting = Formatting.Indented;
			doc.WriteTo(xtw);

			return sw.ToString();
		}

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
		/// <param name="suppliedEntities"></param>
		/// <returns></returns>
		public static IList<System.Type> FindAllEntities(ModelConvention convention, 
			Assembly assembly, 
			IList<System.Type> suppliedEntities = null)
        {
            IList<System.Type> results = new List<System.Type>();

			if(suppliedEntities !=null && suppliedEntities.Count > 0)
			{
				_entities = suppliedEntities;
			}

			if (_entities.Count > 0) return _entities;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Searching for domain entities started...");
#endif
            foreach (System.Type type in assembly.GetTypes())
            {
                if (type.IsClass 
					& !type.IsAbstract 
					& string.IsNullOrEmpty(type.Namespace) == false) // in-place generic class will not have a namespace
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
#if DEBUG
                                    System.Diagnostics.Debug.WriteLine("Entity Found: " + type.Name);
#endif
                                }
                            }
                            else
                            {
                                results.Add(type);
#if DEBUG
								System.Diagnostics.Debug.WriteLine("Entity Found: " + type.Name);
#endif
                            }
                        }
                    }
                }
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Searching for domain entities completed.");
#endif
        	_entities = results;
			return _entities;
        }

        /// <summary>
        /// This will return the property information for the field used for uniqueness in 
        /// entity persistance as defined in the conventions for auto-persistance.
        /// </summary>
        /// <param name="convention"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PropertyInfo FindIdentityFieldFor(ModelConvention convention, System.Type entity)
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
        public static bool IsComponent(ModelConvention convention, System.Type entityPropertyType)
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