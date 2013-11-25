using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Carbon.AutoPersistance.Strategies;

namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// This will set the conventions that can be used to create the configuration for entities for use by NHibernate.
    /// </summary>
    public class ModelConvention
    {
        private bool _pluralizeTableNames = false;
        private PrimaryKeyColumnConvention<ModelConvention> _primaryKey = null;
        private PropertyConvention<ModelConvention> _property = null;
        private MemberAccessConvention<ModelConvention> _memberAccessConvention = null;
        private ForeignKeyNamingConvention<ModelConvention> _foreignKey = null;
        private ManyToManyNamingConvention<ModelConvention> _manyToManyNamingConvention = null;
    	private VersioningConvention<ModelConvention> _versionConvention;
		private bool _canUseLazyLoading = true;

		public IList<System.Type> Entities { get; private set; }

		public void BuildAllEntities(Assembly assembly)
		{
			var candidates = (from match in assembly.GetExportedTypes()
			                  where match.IsClass == true && 
							  match.IsAbstract == false
			                  select match).ToList();

			this.Entities = candidates;
		}

    	public void BaseEntitiesOnType(System.Type entityType)
		{
			var candidates = (from match in entityType.Assembly.GetTypes()
			                  where match.IsClass == true
			                        && match.IsAbstract == false
			                  select match).Distinct().ToList();

			if(entityType.IsGenericType == true)
			{
				this.Entities = (from match in candidates
				                 let genericType = entityType.MakeGenericType(match)
				                 where genericType.IsAssignableFrom(match)
				                 select match).Distinct().ToList();
			}
			else
			{
				this.Entities = (from match in candidates
								 where entityType.IsAssignableFrom(match)
								 select match).Distinct().ToList();
			}
		}

    	/// <summary>
        /// Read-only. Indicator to whether the table names will be pluralized in the repository.
        /// </summary>
        public bool CanPluralizeTableNames
        {
            get { return _pluralizeTableNames; }
        }

        /// <summary>
        /// Sets the value for searching on entities for the property that represents the primary key. 
        /// </summary>
        /// <param name="name">Property name that represents the primary key for each entity.</param>
        /// <returns></returns>
        public PrimaryKeyColumnConvention<ModelConvention> PrimaryKey
        {
            get { return _primaryKey; }
        }

        /// <summary>
        /// Read-only. Sets the naming strategy for foreign key names for relationships between entities.
        /// </summary>
        /// <returns></returns>
        public ForeignKeyNamingConvention<ModelConvention> ForeignKey
        {
            get { return _foreignKey; }
        }

        /// <summary>
        /// Read-only. Sets the convention for entity properties.
        /// </summary>
        /// <returns></returns>
        public PropertyConvention<ModelConvention> Property
        {
            get { return _property; }
        }

        /// <summary>
        /// Read-only. Sets the convention for access entity data members.
        /// </summary>
        /// <returns></returns>
        public MemberAccessConvention<ModelConvention> MemberAccess
        {
            get { return _memberAccessConvention; }
        }

        /// <summary>
        /// Read-only. Sets the naming strategy for join tables in the many-to-many relationships between entities:
        /// </summary>
        public ManyToManyNamingConvention<ModelConvention> ManyToManyTableName
        {
            get { return _manyToManyNamingConvention; }
        }
		
    	public VersioningConvention<ModelConvention> Versioning
    	{
			get { return _versionConvention; }
    	}

        /// <summary>
        /// Gets the current custom naming strategy for the column names 
        ///  in the mapping file to be reflected in the database.
        /// </summary>
        public IColumnNamingStrategy PropertyNamingStrategy 
        { 
            get;
            private set;
        }

        /// <summary>
        /// Gets the flag indicating whether or not to use 
        /// lazy loading on the entities (default = true)
        /// </summary>
        public bool CanUseLazyLoading
        {
            get
            {
                return _canUseLazyLoading;
            }
        }

        public ModelConvention()
        {
            _primaryKey = new PrimaryKeyColumnConvention<ModelConvention>(this);
            _property = new PropertyConvention<ModelConvention>(this);
            _memberAccessConvention = new MemberAccessConvention<ModelConvention>(this);
            _foreignKey = new ForeignKeyNamingConvention<ModelConvention>(this);
            _manyToManyNamingConvention = new ManyToManyNamingConvention<ModelConvention>(this);
        	_versionConvention = new VersioningConvention<ModelConvention>(this);
        	this.PropertyNamingStrategy = new DefaultPropertyNamingStrategy();
        }

        /// <summary>
        /// Sets the indicator to establish whether or not table names will be pluralized in the repository.
        /// </summary>
        /// <returns></returns>
        public ModelConvention PluralizeTableNames()
        {
            _pluralizeTableNames = true;
            return this;
        }

        /// <summary>
        /// Sets the custom naming strategy to use for the columns in the database as a result of the auto-mapping.
        /// </summary>
        /// <param name="propertyNamingStrategy"></param>
        public ModelConvention SetColumnNamingStrategy(IColumnNamingStrategy propertyNamingStrategy)
        {
            this.PropertyNamingStrategy = propertyNamingStrategy;
            return this;
        }

        /// <summary>
        /// Sets the flag indicating that all entities mappings generated will not employ lazy loading.
        /// </summary>
        /// <returns></returns>
        public ModelConvention DoNotUseLazyLoading()
        {
            _canUseLazyLoading = false;
            return this;
        }
    }
}