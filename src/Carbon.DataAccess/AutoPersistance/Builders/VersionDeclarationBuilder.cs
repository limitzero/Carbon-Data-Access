using System.Reflection;
using System.Text;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
	public class VersionDeclarationBuilder : ICanBuildVersionDeclaration
	{
		public ModelConvention Convention { get; set; }

		public System.Type Entity { get; set; }

		public PropertyInfo Property { get; set; }

		public string Build()
		{
			string results = string.Empty;

			if(this.Convention.Versioning != null)
			{
				if(string.IsNullOrEmpty(this.Convention.Versioning.VersionPropertyName) == false && 
				   this.Convention.Versioning.UnsavedValue != null)
				{
					if(this.Property != null && this.Convention.Versioning.UnsavedValue.GetType() == this.Property.PropertyType)
					{
						StringBuilder sb = new StringBuilder();
						sb.Append("<version ")
							.Append(ORMUtils.BuildAttribute("name", this.Convention.Versioning.VersionPropertyName))
							.Append(ORMUtils.BuildAttribute("column", this.Convention.Versioning.VersionPropertyName))
							.Append(ORMUtils.BuildAttribute("type", this.Property.PropertyType.Name))
							.Append(ORMUtils.BuildAttribute("unsaved-value", this.Convention.Versioning.UnsavedValue.ToString()))
							.Append(" />");

						results = sb.ToString();
					}
				}
			}

			return results;
		}
	}
}