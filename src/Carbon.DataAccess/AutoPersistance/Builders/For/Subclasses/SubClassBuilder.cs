using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Builders.For.Components;
using NHibernate.Carbon.AutoPersistance.Builders.For.ForeignKeys;
using NHibernate.Carbon.AutoPersistance.Builders.For.PrimaryKeys;
using NHibernate.Carbon.AutoPersistance.Builders.For.Properties;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders.For.Subclasses
{
	/// <summary>
	/// Contract to realize the "is a" relationship between  the parent and child.
	/// </summary>
	[Optimization("For entities realized as subclasses (i.e. 'is a' relationship between abstract and concrete entities)," + 
		" a table-per-concrete entity strategy will be implemented using the 'joined-subclass' definition.")]
	public class SubClassBuilder : ISubclassBuilder
	{
		private ModelConvention _convention;
		private System.Type _entity;
		private IList<System.Type> _subClassedEntities = new List<System.Type>();

		public IList<System.Type> SubClassedEntities
		{
			get { return _subClassedEntities; }
		}

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

		public SubClassBuilder(ModelConvention convention, System.Type entity)
		{
			_convention = convention;
			_entity = entity;
		}

		public string Build()
		{
			string results = string.Empty;

			_subClassedEntities = FindAllEntitiesThatExtend(_entity);

			foreach (System.Type subclass in _subClassedEntities)
			{
				results += RenderSubClassDefinitionFor(_entity, subclass);
			}

			return results;
		}

		private IList<System.Type> FindAllEntitiesThatExtend(System.Type entity)
		{
			IList<System.Type> results = new List<System.Type>();

			foreach (System.Type type in entity.Assembly.GetTypes())
			{
				if (type.IsClass && !type.IsAbstract && type.BaseType == entity)
				{
					results.Add(type);
				}
			}

			return results;
		}

		private string RenderSubClassDefinitionFor(System.Type parent, System.Type subClass)
		{
			IPrimaryKeyNameBuilder primaryKeyNameBuilderBuilder = new PrimaryKeyNameBuilder(_convention, parent);
			ICanBuildProperty propertyBuilder = new PropertyBuilder(_convention, subClass);
			IComponentBuilder componentBuilderBuilder = new ComponentBuilder(_convention, subClass);
			IForeignKeyNameBuilder foreignKeyNameBuilder = new ForeignKeyNameBuilder(_convention, parent, subClass);
			ManyToOneRelationshipStrategy many2OneBuilder = new ManyToOneRelationshipStrategy(_convention, subClass);
			ManyToManyRelationshipStrategy many2ManyBuilder = new ManyToManyRelationshipStrategy(_convention, subClass);

			string retval = string.Empty;
			StringBuilder document = new StringBuilder();

			document.Append("\r\n");
			document.Append(string.Format("<!-- {0} 'is a' {1} -->", subClass.Name, parent.Name));
			document.Append("\r\n");
			document.Append("<joined-subclass");
			string typeName = string.Concat(subClass.FullName, ", ", subClass.Assembly.GetName().Name);
			document.Append(ORMUtils.BuildAttribute("name", typeName));

			if (_convention.CanPluralizeTableNames)
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
			document.Append(ORMUtils.BuildAttribute("column", primaryKeyNameBuilderBuilder.Build()));
			document.Append(ORMUtils.BuildAttribute("foreign-key", foreignKeyNameBuilder.Build()));
			document.Append("/>");
			document.Append("\r\n");

			PropertyInfo idColumn = ORMUtils.FindIdentityFieldFor(_convention, parent);
			IList<PropertyInfo> parentClassProperties = new List<PropertyInfo>(parent.GetProperties());
			if (idColumn != null)
				parentClassProperties.Remove(idColumn);

			propertyBuilder.PropertiesNotToRender = parentClassProperties;
			document.Append(propertyBuilder.Build());
			document.Append("\r\n");

			componentBuilderBuilder.PropertiesForInspection = propertyBuilder.ExcludedProperties;
			document.Append(componentBuilderBuilder.Build());
			document.Append("\r\n");

			document.Append(many2OneBuilder.Build());
			document.Append("\r\n");

			document.Append(many2ManyBuilder.Build());
			document.Append("\r\n");

			document.Append("</joined-subclass>");

			retval = document.ToString();
			return retval;
		}

	}
}