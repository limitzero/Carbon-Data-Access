using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;
using NHibernate.Carbon.Extensions;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Properties
{
    /// <summary>
    /// Concrete implementation of strategy for building the definition of the properties for an entity.
    /// </summary>
    public class PropertyBuilder : ICanBuildProperty
    {
    	private ModelConvention _convention;
        private System.Type _entity;
        private IList<PropertyInfo> _exclusions = new List<PropertyInfo>();
        private IList<PropertyInfo> _propertiesNotToRender = new List<PropertyInfo>();

		public ModelConvention Convention
		{
			get
			{
				return _convention;
			}
			set
			{
				_convention = value;
			}
		}

		public System.Type Entity
		{
			get
			{
				return _entity;
			}
			set
			{
				_entity = value;
			}
		}

		public IList<PropertyInfo> ExcludedProperties
		{
			get { return _exclusions; }
		}

		public IList<PropertyInfo> PropertiesNotToRender
		{
			set { _propertiesNotToRender = value; }
		}

        public PropertyBuilder()
            : this(null, null)
        {

        }

        public PropertyBuilder(ModelConvention convention, System.Type entity)
        {
            _entity = entity;
            _convention = convention;
        }

        public string Build()
        {
            IList<PropertyInfo> properties = FindPropertiesFor(_entity, out _exclusions);
            string results = RenderProperties(properties);
            return results;
        }

        private PropertyInfo FindIdentityFieldFor(System.Type entity)
        {
            ModelConvention convention = new ModelConvention();
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

        private IList<PropertyInfo> FindPropertiesFor(System.Type entity, out IList<PropertyInfo> exclusions)
        {
            // TODO: delineate all data primatives that can be automatically rendered:
            IList<System.Type> typesToRender = new List<System.Type>(new System.Type[] { 
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

            PropertyInfo idColumn = ORMUtils.FindIdentityFieldFor(_convention, entity);

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

            foreach (PropertyInfo property in _propertiesNotToRender)
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

			var nhVersionProperty = new NHVersionProperty().Build(_convention, _entity, properties);
			var nhProperties = new NHProperty().Build(_convention, _entity, properties);
			
			if(nhVersionProperty != null)
			{
				builder.Append(nhVersionProperty.Serialize()).Append(Environment.NewLine);
			}

			builder.Append(string.Format("<!-- properties for entity '{0}' --> ", _entity.Name));
			builder.Append(Environment.NewLine);
			builder.Append(nhProperties.SerializeAll()).Append(Environment.NewLine);

			return builder.ToString();
		}

    	private string RenderPropertiesEx(IList<PropertyInfo> properties)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(string.Format("<!-- properties for entity '{0}' --> ", _entity.Name));
            builder.Append("\r\n");

        	var nhVersionProperty = new NHVersionProperty().Build(_convention, _entity, properties);
        	var nhProperties = new NHProperty().Build(_convention, _entity, properties);
        	var props = nhProperties.SerializeAll();

        	

            foreach (PropertyInfo property in properties)
            {
				if(this._convention.Versioning != null)
				{
					if (string.IsNullOrEmpty(this._convention.Versioning.VersionPropertyName) == false
						&& this._convention.Versioning.UnsavedValue != null)
					{
						if (this._convention.Versioning.VersionPropertyName == property.Name)
						{
							var versionBuilder = new VersionDeclarationBuilder {Convention = this._convention, Property = property};
							builder.Append(versionBuilder.Build());
							continue;
						}
					}
				}

            	builder.Append("<property");

                //attribute: name:
                builder.Append(ORMUtils.BuildAttribute("name", property.Name));

                //attribute: column
                var columnName = property.Name;

                if (_convention.PropertyNamingStrategy != null)
                {
                    if (property.PropertyType.FullName.StartsWith(typeof(Nullable<>).FullName) == true)
                    {
                        // supporting nullable types (i.e. DateTime?, int?, etc.)..
                        var declaringType = FindUnderlyingNullableType(property.PropertyType);
                        columnName = _convention.PropertyNamingStrategy.Execute(property.Name, declaringType);    
                    }
                    else
                    {
                        columnName = _convention.PropertyNamingStrategy.Execute(property.Name, property.PropertyType);    
                    }
                }
                
                if(columnName != string.Empty)
                {
                    builder.Append(ORMUtils.BuildAttribute("column", columnName));
                }
                else if (_convention.Property.CanRenderAsLowerCase)
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
                if (_convention.Property.LargeTextFieldNames.Contains(property.Name))
                {
                    builder.Append(ORMUtils.BuildAttribute("length", _convention.Property.LargeTextFieldLength.ToString()));
                }
                else
                {
                    builder.Append(ORMUtils.BuildAttribute("length", _convention.Property.DefaultTextFieldLength.ToString()));
                }

                //attribute: access
                if (!string.IsNullOrEmpty(_convention.MemberAccess.Strategy))
                {
                    builder.Append(ORMUtils.BuildAttribute("access", _convention.MemberAccess.Strategy));
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

        private System.Type FindUnderlyingNullableType(System.Type nullableType)
        {
            System.Type underlyingType = null;

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
                underlyingType = typeof(Single);
            }

            if (typeof(Nullable<>).MakeGenericType(typeof(short)) == nullableType)
            {
                underlyingType = typeof(short);
            }

			if (typeof(Nullable<>).MakeGenericType(typeof(bool)) == nullableType || 
				typeof(Nullable<>).MakeGenericType(typeof(Boolean)) == nullableType)
			{
				underlyingType = typeof(bool);
			}

            return underlyingType;
        }
    }
}