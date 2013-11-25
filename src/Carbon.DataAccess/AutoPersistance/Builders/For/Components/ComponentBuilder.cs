using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Components
{
    public class ComponentBuilder : IComponentBuilder
    {
    	private IList<PropertyInfo> _propertiesForInspection;

    	public ModelConvention Convention { get; set; }

    	public System.Type Entity { get; set; }

    	public IList<PropertyInfo> PropertiesForInspection
		{
			set { _propertiesForInspection = value; }
		}

        public ComponentBuilder(ModelConvention convention, System.Type entity)
        {
            this.Convention = convention;
            this.Entity = entity;
        }

        public string Build()
        {
            string retval = string.Empty;
            StringBuilder builder = new StringBuilder();

            foreach (PropertyInfo property in _propertiesForInspection)
            {
                if (ORMUtils.IsComponent(Convention, property.PropertyType))
                {
                	var nhComponent = new NHComponent().Build(Convention, property);
                	builder.Append(nhComponent.Serialize());

					//builder.Append(string.Format("<!-- '{0}' has component/value-object class of  '{1}' realized as '{2}' --> ",
                    //		_entity.Name, property.PropertyType.Name, property.Name));
					//ICanBuildProperty propertyBuilder = new PropertyBuilder(_convention, property.PropertyType);
                	//builder.Append("\r\n");
                	//builder.Append("<component");
                	//builder.Append(ORMUtils.BuildAttribute("name", property.Name));
                	//builder.Append(ORMUtils.BuildAttribute("class", property.PropertyType.Name));
                	//builder.Append(ORMUtils.BuildAttribute("access", _convention.MemberAccess.Strategy));
                	//builder.Append(">");
                	//builder.Append("\r\n");
                	//builder.Append(propertyBuilder.Build());
                	//builder.Append("</component>");
                	//builder.Append("\r\n");
                }
            }

            retval = builder.ToString();
            return retval;
        }

    }
}