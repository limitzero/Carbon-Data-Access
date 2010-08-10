using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Carbon.Repository.AutoPersistance.Core;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public class ComponentBuilder : ICanBuildComponent
    {
        private Convention m_convention = null;
        private Type m_entity = null;
        private IList<PropertyInfo> m_propertiesForInspection = null;

        public ComponentBuilder(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
        }

        #region ICanBuildComponentDefinition Members

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

        public IList<PropertyInfo> PropertiesForInspection
        {
            set { m_propertiesForInspection = value; }
        }

        public string Build()
        {
            string retval = string.Empty;
            StringBuilder builder = new StringBuilder();

            foreach (PropertyInfo property in m_propertiesForInspection)
            {
                if (ORMUtils.IsComponent(m_convention, property.PropertyType))
                {

                    ICanBuildProperty propertyBuilder = new PropertyBuilder(m_convention, property.PropertyType);

                    builder.Append(string.Format("<!-- '{0}' has component/value-object class of  '{1}' realized as '{2}' --> ",
                                                 m_entity.Name, property.PropertyType.Name, property.Name));
                    builder.Append("\r\n");
                    builder.Append("<component");
                    builder.Append(ORMUtils.BuildAttribute("name", property.Name));
                    builder.Append(ORMUtils.BuildAttribute("class", property.PropertyType.Name));
                    builder.Append(ORMUtils.BuildAttribute("access", m_convention.MemberAccess.Strategy));
                    builder.Append(">");
                    builder.Append("\r\n");
                    builder.Append(propertyBuilder.Build());
                    builder.Append("</component>");
                    builder.Append("\r\n");
                }
            }

            retval = builder.ToString();
            return retval;
        }

        #endregion
    }
}