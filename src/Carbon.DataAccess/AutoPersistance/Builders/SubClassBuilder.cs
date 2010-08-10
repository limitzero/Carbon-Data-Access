using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Carbon.Repository.AutoPersistance.Core;

namespace Carbon.Repository.AutoPersistance.Builders
{
    /// <summary>
    /// Contract to realize the "is a" relationship between  the parent and child.
    /// </summary>
    public class SubClassBuilder   : ICanBuildSubClass
    {
        private Convention m_convention = null;
        private Type m_entity = null;
        private IList<Type> m_subClassedEntities = new List<Type>();

        public SubClassBuilder(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
        }

        #region ICanBuildSubClassDefinition Members

        public IList<Type> SubClassedEntities
        {
            get { return m_subClassedEntities; }
        }

        #endregion

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

        public string Build()
        {
            string results = string.Empty;

            m_subClassedEntities = FindAllEntitiesThatExtend(m_entity); 

            foreach(Type subclass in m_subClassedEntities)
            {
                results += RenderSubClassDefinitionFor(m_entity, subclass);
            }

            return results;
        }

        private IList<Type> FindAllEntitiesThatExtend(Type entity)
        {
            IList<Type> results = new List<Type>();

            foreach (Type type in entity.Assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.BaseType == entity)
                {
                    results.Add(type);
                }
            }

            return results;
        }

        private string RenderSubClassDefinitionFor(Type parent, Type subClass)
        {
            ICanBuildPrimaryKeyName primaryKeyNameBuilder = new PrimaryKeyNameBuilder(m_convention, parent);
            ICanBuildProperty propertyBuilder = new PropertyBuilder(m_convention, subClass);
            ICanBuildComponent componentBuilder = new ComponentBuilder(m_convention, subClass);
            ICanBuildForeignKeyName foreignKeyNameBuilder = new ForeignKeyNameBuilder(m_convention, parent, subClass);
            ManyToOneRelationshipStrategy many2OneBuilder = new ManyToOneRelationshipStrategy(m_convention, subClass);
            ManyToManyRelationshipStrategy many2ManyBuilder = new ManyToManyRelationshipStrategy(m_convention, subClass); 

            string retval = string.Empty;
            StringBuilder document = new StringBuilder();

            document.Append("\r\n");
            document.Append(string.Format("<!-- {0} 'is a' {1} -->", subClass.Name, parent.Name));
            document.Append("\r\n");
            document.Append("<joined-subclass");
            string typeName = string.Concat(subClass.FullName, ", ", subClass.Assembly.GetName().Name);
            document.Append(ORMUtils.BuildAttribute("name", typeName));

            if (m_convention.CanPluralizeTableNames)
            {
                document.Append(ORMUtils.BuildAttribute("table", ORMUtils.Pluralize(subClass.Name)));
            }
            else
            {
                document.Append(ORMUtils.BuildAttribute("table", subClass.Name));
            }

            document.Append(ORMUtils.BuildAttribute("extends", parent.Name));
            document.Append(">");
            document.Append("\r\n");
            document.Append("<key");
            document.Append(ORMUtils.BuildAttribute("column", primaryKeyNameBuilder.Build()));
            document.Append(ORMUtils.BuildAttribute("foreign-key", foreignKeyNameBuilder.Build()));
            document.Append("/>");
            document.Append("\r\n");

            PropertyInfo idColumn = ORMUtils.FindIdentityFieldFor(m_convention, parent);
            IList<PropertyInfo> parentClassProperties = new List<PropertyInfo>(parent.GetProperties());
            if(idColumn != null)
                parentClassProperties.Remove(idColumn);

            propertyBuilder.PropertiesNotToRender = parentClassProperties;
            document.Append(propertyBuilder.Build());
            document.Append("\r\n");

            componentBuilder.PropertiesForInspection = propertyBuilder.ExcludedProperties;
            document.Append(componentBuilder.Build());
            document.Append("\r\n");

            document.Append(many2OneBuilder.Build());
            document.Append("\r\n");

            document.Append(many2ManyBuilder.Build());
            document.Append("\r\n");

            document.Append("</joined-subclass>");

            retval = document.ToString();
            return retval;
        }

        #endregion
    }
}