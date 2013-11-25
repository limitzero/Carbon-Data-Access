using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using NHibernate.Carbon.AutoPersistance.Persistance;
using NHibernate.Carbon.AutoPersistance.Persistance.Strategies;
using NHibernate.Carbon.AutoPersistance.Schema.Elements;
using NHibernate.Carbon.Repository;

namespace NHibernate.Carbon.AutoPersistance.Core
{
    public class AutoPersistanceModel : IAutoPersistanceModel
    {
        private const string _default_nhibernate_config_file = @"hibernate.cfg.xml";

        private NHibernate.ISessionFactory _sessionFactory = null;
        private bool _isSchemaCreated = false;
        private bool _isSchemaDropped = false;
        private string _mappingsFilePath = string.Empty;

        private Assembly _assembly = null;
        private IList<System.Type> _entities = new List<System.Type>();
    	private readonly ModelConvention _convention = null;
        private readonly EntityNamespace<AutoPersistanceModel> _namespace = null;
        private bool _renderMappingPerEntity = true;
        private readonly IDictionary<System.Type, string> _maps = new Dictionary<System.Type, string>();
        private readonly IPersistanceStrategy _strategy = null;
        private string _configurationFile;

		public AutoPersistanceModel()
			:this(new DefaultPersistanceStrategy(), new DefaultConvention())
		{
		}

    	public AutoPersistanceModel(ModelConvention convention)
			:this(new DefaultPersistanceStrategy(), convention)
		{
		}

    	public AutoPersistanceModel(IPersistanceStrategy strategy, ModelConvention convention)
        {
            _convention = convention;
            _strategy = strategy;
            _namespace = new EntityNamespace<AutoPersistanceModel>(this);
        }

		/// <summary>
		/// Gets the value to indicate whether or not the model has been built.
		/// </summary>
		public bool IsBuilt { get; private set; }

        /// <summary>
        /// Read-only. Contains the collection of entity maps for persistance. 
        /// </summary>
        public IDictionary<System.Type, string> Maps
        {
            get { return _maps; }
        }

        public ICollection<System.Type> Entities
        {
            get { return _entities; }
        }

        /// <summary>
        /// (Read-Only). This will return the assembly of the domain model that is inspected for auto-persistance.
        /// </summary>
        public Assembly DomainModelAssembly
        {
            get { return _assembly; }
        }

		public void Dispose()
		{
			if(this.CurrentSessionFactory != null)
			{
				this.CurrentSessionFactory.Close();
			}
			this.CurrentSessionFactory = null;

			this.CurrentConfiguration = null;
		}

    	public string GetMaps()
        {
            string results = string.Empty;

        	foreach (var map in this.Maps)
        	{
        		results += string.Concat(map.Value, System.Environment.NewLine);
        	}
           
            return results;
        }

        public string GetMapFor<T>()
            where T : class
        {
			string map = string.Empty;
			this.Maps.TryGetValue(typeof(T), out map);
			return map;
        }

        public string GetCurrentConfigurationFile()
        {
            return _configurationFile;
        }

        /// <summary>
        /// Sets the model to emit a mapping per entity. Default is one mapping for all entities.
        /// </summary>
        public bool CanRenderMappingPerEntity
        {
            get { return _renderMappingPerEntity; }
        }

		/// <summary>
		/// Namespace to start searching for entities for auto-mapping.
		/// </summary>
        public EntityNamespace<AutoPersistanceModel> Namespace
        {
            get { return _namespace; }
        }

        public NHibernate.Cfg.Configuration CurrentConfiguration
        {
            get { return _strategy.Configuration; }
			private set { _strategy.Configuration = value; }
        }

        public NHibernate.ISessionFactory CurrentSessionFactory
        {
            get { return _sessionFactory; }
            set { _sessionFactory = value; }
        }

        public bool IsSchemaCreated
        {
            get { return _isSchemaCreated; }
        }

        public bool IsSchemaDropped
        {
            get { return _isSchemaDropped; }
        }

        /// <summary>
        /// Sets the assembly to scan for entities.
        /// </summary>
        /// <param name="assembly">Name of assembly to scan.</param>
        public void Assembly(string assembly)
        {
            _assembly = System.Reflection.Assembly.Load(assembly);
        }

        /// <summary>
        /// Sets the assembly to scan for entities.
        /// </summary>
        /// <param name="assembly">Assembly of the entities.</param>
        public void Assembly(Assembly assembly)
        {
            _assembly = assembly;
        }

        public void ConfigurationFile(string configurationFile)
        {
            _configurationFile = configurationFile;
        	this.IsBuilt = false;
        }

        /// <summary>
        /// Sets the model to emit a mapping per entity. Default is one mapping for all entities.
        /// </summary>
        public void RenderMappingPerEntity()
        {
            _renderMappingPerEntity = true;
        }

        /// <summary>
        /// Indicator for the model to cache the entity mapping definition.
        /// </summary>
        /// <returns></returns>
        public AutoPersistanceModel CacheMapping()
        {
            return this;
        }

		/// <summary>
		/// This will write the set of mapping files for all entities that are auto-mapped
		/// to a particular directory. They are re-created on every call for auto-configuration.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
        public AutoPersistanceModel WriteMappingsTo(string filePath)
        {
            _mappingsFilePath = filePath;
            return this;
        }

        /// <summary>
        /// This will build the model for persisting entities to the indicated data store.
        /// </summary>
        public void Build(bool useApplicationConfigurationForSettings = false)
        {
			if (IsBuilt) return;
            _strategy.Initialize();

			if (useApplicationConfigurationForSettings == true)
			{
				_strategy.Configuration.Configure();
			}
			else
			{
				if (!string.IsNullOrEmpty(_configurationFile))
				{
					_strategy.Configuration.Configure(_configurationFile);
				}
				else
				{
					_strategy.Configuration.Configure(_default_nhibernate_config_file);
					this._configurationFile = _default_nhibernate_config_file;
				}
			}

        	// get the working set of all entities in the target domain model assembly:
			_entities = ORMUtils.FindAllEntities(_convention, _assembly);

			// remove entities that are not in the desired target namespace (if defined):
			if (this.Namespace != null)
			{
				var toKeep = (from match in _entities
							  where this.Namespace.IsMatchFor(match.Namespace) == true
							  select match).ToList();

				if (toKeep != null && toKeep.Count > 0)
				{
					_entities = (from match in _entities
								 join keep in toKeep on match.Name equals keep.Name
								 select keep).ToList();

					// re-initialize the set of working entities:
					ORMUtils.FindAllEntities(_convention, _assembly, _entities);
				}
			}

			//_convention.BuildAllEntities(_assembly);

			//// grab the list of persistent entities from the domain model:
			//if(_convention.Entities != null && _convention.Entities.Count > 0)
			//{
			//    _entities = _convention.Entities
			//        .Where(x => this.Namespace.IsMatchFor(x.Namespace))
			//        .Distinct().ToList();

			//    // just pass back the set of created entities, but must initialize it first:
			//    _entities = ORMUtils.FindAllEntities(_convention, _assembly, _entities);	
			//}
			//else
			//{
			//    _entities = ORMUtils.FindAllEntities(_convention, _assembly);	

			//    // remove entities that are not in the target namespace (if defined):
			//    if(this.Namespace != null)
			//    {
			//        var toKeep = (from match in _entities
			//                      where this.Namespace.IsMatchFor(match.Namespace) == true
			//                      select match).ToList();

			//        if(toKeep != null && toKeep.Count > 0)
			//        {
			//            _entities = (from match in _entities
			//                         join keep in toKeep on match.Name equals keep.Name
			//                         select keep).ToList();
			//        }
			//    }
			//}
        	
			_maps.Clear();

            foreach (System.Type entity in _entities)
            {
                // build the mapping:
                //ICanBuildEntityDefinition entityBuilider = new EntityBuilder(_convention, entity);
                //string map = entityBuilider.Build();
            	try
            	{
					// only create mappings for entities that have an identifier for persistance:
					if (ORMUtils.FindIdentityFieldFor(_convention, entity) != null)
					{
						var nhMap = new NHMap().Build(_convention, entity);
						string map = nhMap.Serialize();
						_maps.Add(entity, ORMUtils.FormatXMLDocument(map));
						WriteEntityMapToFileFor(entity, map);
					}
            	}
            	catch (Exception e)
            	{
            		string message =
            			string.Format(
            				"An error has occurred while trying to build the " +
							"mapping document for entity '{0}'. Reason: {1}",
            				entity.FullName, e.Message);
            		
            		throw new Exception(message, e);
            	}
			
            }
           
            // create the NHibernate session factory with the created maps:
            if(string.IsNullOrEmpty(_mappingsFilePath))
            {
				foreach (var map in _maps)
                {
                    try
                    {
                        var entityMap = new XmlDocument();
						entityMap.LoadXml(map.Value);

                        _strategy.Configuration.AddDocument(entityMap);
                    }
                    catch (Exception exc)
                    {
						System.Diagnostics.Debug.WriteLine(string.Format("Build()::LoadMap -  Error: {0} \r\n Map : {1}", exc.ToString(), map.Value));
                        throw;
                    }
                }
            }

            // add the assembly containing the entities to persist to the strategy configuration:
            _strategy.Configuration.AddAssembly(_assembly);

            try
            {
                if (_sessionFactory == null)
                    _sessionFactory = _strategy.Configuration.BuildSessionFactory();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Build()::CreateSessionFactory - Error: {0}", exc.ToString()));
                throw;
            }

			IsBuilt = true;
        }

        /// <summary>
        /// This will create the data schema for persistance based on the model in the data store.
        /// </summary>
        public void CreateSchema()
        {
            if (_isSchemaCreated)
                throw new Exception("The schema has already been created for the entity mappings.");

            // configure the data repository based on the NHibernate mappings:
            try
            {
                if (_strategy.Configuration != null)
                {
                    NHibernate.Tool.hbm2ddl.SchemaExport exporter = 
						new NHibernate.Tool.hbm2ddl.SchemaExport(_strategy.Configuration);
                    exporter.Execute(true, true, false);
                }

                _isSchemaCreated = true;
            }
            catch (Exception exc)
            {
                string msg = "An error has occurred while attempting to create the schema for the entities. Reason: " + exc.ToString();
                throw new Exception(msg, exc);
            }
        }

        /// <summary>
        /// This will delete the data schema for persistance based on the model from the data store.
        /// </summary>
        public void DropSchema()
        {
            try
            {
                // configure the data repository based on the NHibernate mappings:
                if (_strategy.Configuration != null)
                {
                    NHibernate.Tool.hbm2ddl.SchemaExport exporter = 
						new NHibernate.Tool.hbm2ddl.SchemaExport(_strategy.Configuration);
                    exporter.Execute(true, true, true);
                }

                if (_sessionFactory != null)
                {
                    _sessionFactory.Close();
                    _sessionFactory.Dispose();
                    _sessionFactory = null;
                }

                _isSchemaDropped = true;
            }
            catch (Exception exc)
            {
                string msg = "An error has occurred while attempting to drop the schema for the entities. Reason: " + exc.ToString();
                throw new Exception(msg, exc);
            }
        }

    	/// <summary>
    	/// This will return an instance of the <seealso cref="NHibernateRepository{T}"/> for a specific 
    	/// class for basic data retrieval and persistance operations.
    	/// </summary>
    	/// <typeparam name="T"></typeparam>
    	/// <returns></returns>
    	public NHibernateRepository<T> GetRepositoryFor<T>() where T : class
    	{
    		return new NHibernateRepository<T>(this._sessionFactory.OpenSession());
    	}

    	private string BuildNHibernateShell(System.Type entity, string entityDefinition)
        {
            StringBuilder document = new StringBuilder();
            document.Append("<hibernate-mapping");
            document.Append(ORMUtils.BuildAttribute("xmlns", "urn:nhibernate-mapping-2.2"));
            document.Append("\n").Append("\t");
            document.Append(ORMUtils.BuildAttribute("assembly", entity.Assembly.GetName().Name));
            document.Append(ORMUtils.BuildAttribute("namespace", entity.Namespace));
            document.Append("\n").Append("\t");
            document.Append(ORMUtils.BuildAttribute("default-lazy", _convention.CanUseLazyLoading.ToString().ToLower()));
            document.Append(">");
            document.Append("\r\n\r\n");
            document.Append(entityDefinition);
            document.Append("\r\n\r\n");
            document.Append("</hibernate-mapping>");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(document.ToString());

            string shell = FormatEntityHbmContents(xmlDoc.OuterXml);

            return shell;
        }

        private string  WriteEntityMapToFileFor(System.Type entity, string contents)
        {
            string document = string.Empty;

            string fileName = "{0}.hbm.xml";

            if (string.IsNullOrEmpty(_mappingsFilePath))
                return document;

            if (!Directory.Exists(_mappingsFilePath))
                throw new ArgumentException("The directory [" + _mappingsFilePath + "] was not found to place the mapping files.");

            string file = Path.Combine(_mappingsFilePath, string.Format(fileName, entity.Name));

			File.Delete(file);
			using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
			{
				var nhibernateDocument = BuildNHibernateShell(entity, contents);
				document = nhibernateDocument;

				byte[] data = ASCIIEncoding.ASCII.GetBytes(nhibernateDocument);
				fs.Write(data, 0, data.Length);
			}

            return document;
        }

        public static string FormatEntityHbmContents(string entityHBMContents)
        {
            // properly indent the xml contents:
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(entityHBMContents);

            StringWriter sw = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(sw);
            xtw.Formatting = Formatting.Indented;
            doc.WriteTo(xtw);

            return sw.ToString();
        }

        private ModelConvention DefaultConvention()
        {
            ModelConvention convention = new ModelConvention();
            convention.PluralizeTableNames();
            convention.MemberAccess.NoSetterLowerCaseUnderscore();

            convention.PrimaryKey.PropertyName("Id"); //exclude primary keys in property definition.
            convention.PrimaryKey.RenderAsLowerCaseEntityNameFollowedByID();
            convention.PrimaryKey.MemberAccess.NoSetterCamelCaseUnderscore();

            convention.ForeignKey.RenderAsParentEntityHasInstancesOfChildEntity();
            convention.ManyToManyTableName.RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized();

            convention.Property.SetLargeTextFieldLengthsAndNames(10000, "Comments", "Notes", "Body", "History", "Message", "Event", "Text");
            convention.Property.SetDefaultTextFieldLength(100);
            convention.Property.RenderAsLowerCaseInRepository();

            return convention;
        }

    }
}