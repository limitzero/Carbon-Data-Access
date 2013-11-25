using System.Collections.Generic;
using System.Text;
using NHibernate.Carbon.AutoPersistance.Builders.For.Components;
using NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys;
using NHibernate.Carbon.AutoPersistance.Builders.For.Properties;
using NHibernate.Carbon.AutoPersistance.Builders.For.Subclasses;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    public class EntityBuilder : ICanBuildEntityDefinition
    {
        private IList<System.Type> _renderedEntities = new List<System.Type>();
        private ModelConvention _convention = null;
        private System.Type _entity = null;

        public EntityBuilder()
            : this(null, null)
        {
        }

        public EntityBuilder(ModelConvention convention, System.Type entity)
        {
            _convention = convention;
            _entity = entity;
        }

        #region IBuilder Members

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

        public IList<System.Type> RenderedEntities
        {
            get { return _renderedEntities; }
        }

        public string Build()
        {
            StringBuilder document = new StringBuilder();

            IPrimaryKeyBuilder primaryBuilder = new PrimaryKeyBuilder(_convention, _entity);
            ICanBuildProperty propertyBuilder = new PropertyBuilder(_convention, _entity);
            IComponentBuilder componentBuilderBuilder = new ComponentBuilder(_convention, _entity);
            ISubclassBuilder subclassBuilder = new SubClassBuilder(_convention, _entity);
            ManyToOneRelationshipStrategy many2OneBuilder = new ManyToOneRelationshipStrategy(_convention, _entity);
            ManyToManyRelationshipStrategy many2ManyBuilder = new ManyToManyRelationshipStrategy(_convention, _entity);

            document.Append(string.Format("<!-- entity: {0} -->", _entity.Name));
            document.Append("\r\n");

            document.Append("<class");
            document.Append(ORMUtils.BuildAttribute("name", _entity.Name));

            if (_convention.CanPluralizeTableNames)
                document.Append(ORMUtils.BuildAttribute("table", ORMUtils.Pluralize(_entity.Name)));

            //document.Append(MappingHelper.BuildAttribute("where", visitee.FilterCondition));

            document.Append(">");
            document.Append("\r\n");

            document.Append(primaryBuilder.Build());
            document.Append("\r\n");

            document.Append(propertyBuilder.Build());
            document.Append("\r\n");

            componentBuilderBuilder.PropertiesForInspection = propertyBuilder.ExcludedProperties;
            document.Append(componentBuilderBuilder.Build());
            document.Append("\r\n");

            document.Append(subclassBuilder.Build());
            document.Append("\r\n");

            // make sure to pass the entities that were rendered back 
            // to the calling render process so that duplicates are not 
            // made:
            if (subclassBuilder.SubClassedEntities.Count > 0)
            {
                _renderedEntities = subclassBuilder.SubClassedEntities;
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