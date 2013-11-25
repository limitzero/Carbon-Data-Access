using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[XmlRoot(ElementName = "property")]
	public class NHProperty
	{
		[XmlText()]
		public string Comment;

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "column")]
		public string Column { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		[XmlAttribute(AttributeName = "not-null")]
		public bool NotNull { get; set; }

		[XmlAttribute(AttributeName = "length")]
		public int Length { get; set; }

		[XmlAttribute(AttributeName = "access")]
		public string Access { get; set; }

		public NHProperty()
		{

		}

		public IList<NHProperty> Build(ModelConvention modelConvention,
											 System.Type entity,
											 ICollection<PropertyInfo> properties)
		{
			var mappedProperties = new List<NHProperty>();
			this.Comment = string.Format(string.Format("<!-- properties for entity '{0}' --> ", entity.Name));

			foreach (var property in properties)
			{
				// skip rendering of the 'Version' property:
				if (new NHVersionProperty().Build(modelConvention, entity, new List<PropertyInfo> { property }) != null) continue;

				var mappedProperty = new NHProperty { Name = property.Name };
				this.DetermineColumnName(mappedProperty, modelConvention, property);
				this.DetermineColumnType(mappedProperty, modelConvention, property);
				this.DetermineColumnLength(mappedProperty, modelConvention, property);
				this.DetermineColumnAccess(mappedProperty, modelConvention, property);
				mappedProperties.Add(mappedProperty);
			}

			return mappedProperties.AsReadOnly();
		}

		public string Serialize()
		{
			return ORMUtils.Serialize<NHProperty>(this);
		}

		private void DetermineColumnName(NHProperty mappedProperty, ModelConvention modelConvention, PropertyInfo property)
		{
			var columnName = property.Name;

			if (modelConvention.PropertyNamingStrategy != null)
			{
				if (property.PropertyType.FullName.StartsWith(typeof(Nullable<>).FullName) == true)
				{
					// supporting nullable types (i.e. DateTime?, int?, etc.)..
					var declaringType = FindUnderlyingNullableType(property.PropertyType);
					columnName = modelConvention.PropertyNamingStrategy.Execute(property.Name, declaringType);
				}
				else
				{
					columnName = modelConvention.PropertyNamingStrategy.Execute(property.Name, property.PropertyType);
				}
			}

			if (columnName != string.Empty)
			{
				mappedProperty.Column = columnName;
			}
			else if (modelConvention.Property.CanRenderAsLowerCase)
			{
				mappedProperty.Column = columnName.ToLower();
			}
		}

		private void DetermineColumnType(NHProperty mappedProperty, ModelConvention modelConvention, PropertyInfo property)
		{
			//attribute: type
			if (property.PropertyType.FullName.Contains("System"))
			{
				if (property.PropertyType.FullName.StartsWith(typeof(Nullable<>).FullName) == true)
				{
					// supporting nullable types (i.e. DateTime?, int?, etc.)..
					var declaringType = FindUnderlyingNullableType(property.PropertyType);
					mappedProperty.Type = declaringType.FullName;
					mappedProperty.NotNull = false;
				}
				else
				{
					mappedProperty.Type = property.PropertyType.Name;
				}
			}
			else
			{
				string typeName = string.Concat(property.PropertyType.FullName, ", ", property.DeclaringType.Assembly.GetName().Name);
				mappedProperty.Type = typeName;
			}
		}

		private void DetermineColumnLength(NHProperty mappedProperty, ModelConvention modelConvention, PropertyInfo property)
		{
			//attribute: length:
			if (modelConvention.Property.LargeTextFieldNames.Contains(property.Name))
			{
				mappedProperty.Length = modelConvention.Property.LargeTextFieldLength;
			}
			else
			{
				mappedProperty.Length = modelConvention.Property.DefaultTextFieldLength;
			}
		}

		private void DetermineColumnAccess(NHProperty mappedProperty, ModelConvention modelConvention, PropertyInfo property)
		{
			//attribute: access
			if (!string.IsNullOrEmpty(modelConvention.MemberAccess.Strategy))
			{
				mappedProperty.Access = modelConvention.MemberAccess.Strategy;
			}
		}

		/// <summary>
		/// This will add support for constructing property mappings for nullable types.
		/// </summary>
		/// <param name="nullableType"></param>
		/// <returns></returns>
		private System.Type FindUnderlyingNullableType(System.Type nullableType)
		{
			System.Type underlyingType = null;

			if (typeof(Nullable<>).MakeGenericType(typeof(DateTime)) == nullableType)
			{
				underlyingType = typeof(DateTime);
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