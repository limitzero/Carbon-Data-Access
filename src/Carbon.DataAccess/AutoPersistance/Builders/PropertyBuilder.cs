using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Carbon.Repository.AutoPersistance.Core;

namespace Carbon.Repository.AutoPersistance.Builders
{
    /// <summary>
    /// Concrete implementation of strategy for building the definition of the properties for an entity.
    /// </summary>
    public class PropertyBuilder : ICanBuildProperty
    {
        private Convention m_convention = null;
        private Type m_entity = null;
        private IList<PropertyInfo> m_exclusions = new List<PropertyInfo>();
        private IList<PropertyInfo> m_propertiesNotToRender = new List<PropertyInfo>();

        public PropertyBuilder()
            : this(null, null)
        {

        }

        public PropertyBuilder(Convention convention, Type entity)
        {
            m_entity = entity;
            m_convention = convention;
        }

        #region IBuilder Members

        public Convention Convention
        {
            get
            {
                return m_convention;
            }
            set
            {
                m_convention = value;
            }
        }

        public Type Entity
        {
            get
            {
                return m_entity;
            }
            set
            {
                m_entity = value;
            }
        }

        public IList<PropertyInfo> ExcludedProperties
        {
            get { return m_exclusions; }
        }

        public IList<PropertyInfo> PropertiesNotToRender
        {
            set { m_propertiesNotToRender = value; }
        }

        public string Build()
        {
            IList<PropertyInfo> properties = FindPropertiesFor(m_entity, out m_exclusions);
            string results = RenderProperties(properties);
            return results;
        }

        #endregion

        private PropertyInfo FindIdentityFieldFor(Type entity)
        {
            Convention convention = new Convention();
            convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.

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

        private IList<PropertyInfo> FindPropertiesFor(Type entity, out IList<PropertyInfo> exclusions)
        {
            // TODO: delineate all data primatives that can be automatically rendered:
            IList<Type> typesToRender = new List<Type>(new Type[] { 
                typeof(int),
                typeof(int?),
                typeof(string),
                typeof(String), 
                typeof(Enum), 
                typeof(DateTime), 
                typeof(DateTime?),
                typeof(Int32), 
                typeof(Int32?),
                typeof(Int16), 
                typeof(Int16?),
                typeof(Int64), 
                typeof(Int64?),
                typeof(Byte), 
                typeof(decimal),    
                typeof(decimal?),
                typeof(Decimal), 
                typeof(Decimal?), 
                typeof(Single),                                                
                typeof(Single?), 
                typeof(float),
                typeof(float?),
                typeof(bool),
                typeof(bool?), 
                typeof(short),
                typeof(short?)
                });

            IList<PropertyInfo> properties = new List<PropertyInfo>(entity.GetProperties());

            exclusions = new List<PropertyInfo>();

            PropertyInfo idColumn = ORMUtils.FindIdentityFieldFor(m_convention, entity);

            foreach (PropertyInfo property in properties)
            {

                if (!typesToRender.Contains(property.PropertyType) && !property.PropertyType.IsEnum)
                {
                    // do not render properties:
                    exclusions.Add(property);
                }

            }

            // remove the excluded properties (these are special cases):
            foreach (PropertyInfo property in exclusions)
            {
                properties.Remove(property);
            }

            if (idColumn != null)
                properties.Remove(idColumn);

            foreach (PropertyInfo property in m_propertiesNotToRender)
            {
                PropertyInfo toFind = FindByName(property.Name, properties);
                if (toFind != null)
                    properties.Remove(toFind);
            }

            return properties;
        }

        private PropertyInfo FindByName(string name, IList<PropertyInfo> properties)
        {
            PropertyInfo results = null;

            foreach (PropertyInfo property in properties)
            {
                if (property.Name.ToLower() == name.ToLower())
                {
                    results = property;
                    break;
                }
            }

            return results;
        }

        private string RenderProperties(IList<PropertyInfo> properties)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("<!-- properties for entity '{0}' --> ", m_entity.Name));
            builder.Append("\r\n");

            foreach (PropertyInfo property in properties)
            {
                builder.Append("<property");

                //attribute: name:
                builder.Append(ORMUtils.BuildAttribute("name", property.Name));

                //attribute: column
                var columnName = property.Name;

                if (m_convention.PropertyNamingStrategy != null)
                {
                    if (property.PropertyType.FullName.StartsWith(typeof(Nullable<>).FullName) == true)
                    {
                        // supporting nullable types (i.e. DateTime?, int?, etc.)..
                        var declaringType = FindUnderlyingNullableType(property.PropertyType);
                        columnName = m_convention.PropertyNamingStrategy.Execute(property.Name, declaringType);    
                    }
                    else
                    {
                        columnName = m_convention.PropertyNamingStrategy.Execute(property.Name, property.PropertyType);    
                    }
                }
                
                if(columnName != string.Empty)
                {
                    builder.Append(ORMUtils.BuildAttribute("column", columnName));
                }
                else if (m_convention.Property.CanRenderAsLowerCase)
                {
                    builder.Append(ORMUtils.BuildAttribute("column", property.Name.ToLower()));
                }

                //attribute: type
                if (property.PropertyType.FullName.Contains("System"))
                {
                    if(property.PropertyType.FullName.StartsWith(typeof(Nullable<>).FullName) == true)
                    {
                        // supporting nullable types (i.e. DateTime?, int?, etc.)..
                        var declaringType = FindUnderlyingNullableType(property.PropertyType);
                        builder.Append(ORMUtils.BuildAttribute("type", declaringType.FullName));
                        builder.Append(ORMUtils.BuildAttribute("not-null", "false"));
                    }
                    else
                    {
                        builder.Append(ORMUtils.BuildAttribute("type", property.PropertyType.Name));
                    }
                   
                }
                else
                {
                    string typeName = string.Concat(property.PropertyType.FullName, ", ", property.DeclaringType.Assembly.GetName().Name);
                    builder.Append(ORMUtils.BuildAttribute("type", typeName));
                }

                //attribute: length:
                if (m_convention.Property.LargeTextFieldNames.Contains(property.Name))
                {
                    builder.Append(ORMUtils.BuildAttribute("length", m_convention.Property.LargeTextFieldLength.ToString()));
                }
                else
                {
                    builder.Append(ORMUtils.BuildAttribute("length", m_convention.Property.DefaultTextFieldLength.ToString()));
                }

                //attribute: access
                if (!string.IsNullOrEmpty(m_convention.MemberAccess.Strategy))
                {
                    builder.Append(ORMUtils.BuildAttribute("access", m_convention.MemberAccess.Strategy));
                }

                //unique: access
                if (property.PropertyType.IsEnum)
                {
                    //builder.Append(ORMUtils.BuildAttribute("unique","true"));
                }

                builder.Append("/>");
                builder.Append("\r\n");
            }

            return builder.ToString();

        }

        private Type FindUnderlyingNullableType(Type nullableType)
        {
            Type underlyingType = null;

            if(typeof(Nullable<>).MakeGenericType(typeof(DateTime)) == nullableType)
            {
                underlyingType = typeof (DateTime);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(Int16)) == nullableType)
            {
                underlyingType = typeof(Int16);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(Int32)) == nullableType)
            {
                underlyingType = typeof(Int32);
            }


            if (typeof(Nullable<>).MakeGenericType(typeof(Int64)) == nullableType)
            {
                underlyingType = typeof(Int64);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(float)) == nullableType)
            {
                underlyingType = typeof(float);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(Decimal)) == nullableType)
            {
                underlyingType = typeof(Decimal);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(decimal)) == nullableType)
            {
                underlyingType = typeof(Decimal);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(Single)) == nullableType)
            {
                underlyingType = typeof(decimal);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(short)) == nullableType)
            {
                underlyingType = typeof(short);
            }

            return underlyingType;
        }
    }
}