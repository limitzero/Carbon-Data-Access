using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Repository.AutoPersistance.Core;
using Giza.ORM.ForNHibernate.Builders;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public class EntityBuilder : ICanBuildEntityDefinition
    {
        private IList<Type> m_renderedEntities = new List<Type>();
        private Convention m_convention = null;
        private Type m_entity = null;

        public EntityBuilder()
            : this(null, null)
        {
        }

        public EntityBuilder(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
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

        public IList<Type> RenderedEntities
        {
            get { return m_renderedEntities; }
        }

        public string Build()
        {
            StringBuilder document = new StringBuilder();

            ICanBuildPrimaryKey primaryBuilder = new PrimaryKeyBuilder(m_convention, m_entity);
            ICanBuildProperty propertyBuilder = new PropertyBuilder(m_convention, m_entity);
            ICanBuildComponent componentBuilder = new ComponentBuilder(m_convention, m_entity);
            ICanBuildSubClass subclassBuilder = new SubClassBuilder(m_convention, m_entity);
            ManyToOneRelationshipStrategy many2OneBuilder = new ManyToOneRelationshipStrategy(m_convention, m_entity);
            ManyToManyRelationshipStrategy many2ManyBuilder = new ManyToManyRelationshipStrategy(m_convention, m_entity);

            document.Append(string.Format("<!-- entity: {0} -->", m_entity.Name));
            document.Append("\r\n");

            document.Append("<class");
            document.Append(ORMUtils.BuildAttribute("name", m_entity.Name));

            if (m_convention.CanPluralizeTableNames)
                document.Append(ORMUtils.BuildAttribute("table", ORMUtils.Pluralize(m_entity.Name)));

            //document.Append(MappingHelper.BuildAttribute("where", visitee.FilterCondition));

            document.Append(">");
            document.Append("\r\n");

            document.Append(primaryBuilder.Build());
            document.Append("\r\n");

            document.Append(propertyBuilder.Build());
            document.Append("\r\n");

            componentBuilder.PropertiesForInspection = propertyBuilder.ExcludedProperties;
            document.Append(componentBuilder.Build());
            document.Append("\r\n");

            document.Append(subclassBuilder.Build());
            document.Append("\r\n");

            // make sure to pass the entities that were rendered back 
            // to the calling render process so that duplicates are not 
            // made:
            if (subclassBuilder.SubClassedEntities.Count > 0)
            {
                m_renderedEntities = subclassBuilder.SubClassedEntities;
            }

            document.Append(many2OneBuilder.Build());
            document.Append("\r\n");

            document.Append(many2ManyBuilder.Build());
            document.Append("\r\n");

            document.Append("</class>");
            document.Append("\r\n");
            document.Append("\r\n");

            return document.ToString();

        }

        #endregion
    }
}