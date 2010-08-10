using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Carbon.Repository.AutoPersistance.Builders;
using Carbon.Repository.AutoPersistance.Persistance;

namespace Carbon.Repository.AutoPersistance.Core
{
    public class AutoPersistanceModel : IAutoPersistanceModel
    {
        private const string _default_nhibernate_config_file = @"hibernate.cfg.xml";

        private NHibernate.ISessionFactory m_sessionFactory = null;
        private bool _isSchemaCreated = false;
        private bool _isSchemaDropped = false;
        private string m_mappingsFilePath = string.Empty;

        private Assembly m_assembly = null;
        private IList<Type> m_entities = new List<Type>();
        private IList<Type> _entityBaseClasses = new List<Type>();
        private Convention m_convention = null;
        private EntityNamespace<AutoPersistanceModel> m_namespace = null;
        private bool _renderMappingPerEntity = false;
        private IList<string> _maps = new List<string>();
        private bool _cacheCurrentMapping = false;
        private IPersistanceStrategy m_strategy = null;
        private string m_configurationFile;
        private bool _isBuilt;

        public AutoPersistanceModel(IPersistanceStrategy strategy, Convention convention)
        {
            m_convention = convention;
            m_strategy = strategy;
            m_namespace = new EntityNamespace<AutoPersistanceModel>(this);
        }

        /// <summary>
        /// Read-only. Contains the collection of entity maps for persistance. 
        /// </summary>
        public IList<string> Maps
        {
            get { return _maps; }
        }

        public ICollection<Type> Entities
        {
            get { return m_entities; }
        }

        /// <summary>
        /// (Read-Only). This will return the assembly of the domain model that is inspected for auto-persistance.
        /// </summary>
        public Assembly DomainModelAssembly
        {
            get { return m_assembly; }
        }

        public string GetMaps()
        {
            string results = string.Empty;

            for (int index = 0; index < _maps.Count; index++)
            {
                results += GetMaps(index);
            }

            return results;
        }

        public string GetMaps(int index)
        {
            return _maps[index];
        }

        public string GetMapFor<T>()
            where T : class
        {
            string results = string.Empty;

            for (int index = 0; index < _maps.Count; index++)
            {
                if (GetMaps(index).Contains(string.Format("class name=\"{0}\"", typeof(T).Name)))
                {
                    results = GetMaps(index);
                    break;
                }
            }

            return results;
        }

        public string GetCurrentConfigurationFile()
        {
            return m_configurationFile;
        }

        /// <summary>
        /// Sets the model to emit a mapping per entity. Default is one mapping for all entities.
        /// </summary>
        public bool CanRenderMappingPerEntity
        {
            get { return _renderMappingPerEntity; }
        }

        public EntityNamespace<AutoPersistanceModel> Namespace
        {
            get { return m_namespace; }
        }

        public NHibernate.Cfg.Configuration CurrentConfiguration
        {
            get { return m_strategy.Configuration; }
        }

        public NHibernate.ISessionFactory CurrentSessionFactory
        {
            get { return m_sessionFactory; }
            set { m_sessionFactory = value; }
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
            m_assembly = System.Reflection.Assembly.Load(assembly);
        }

        /// <summary>
        /// Sets the assembly to scan for entities.
        /// </summary>
        /// <param name="assembly">Assembly of the entities.</param>
        public void Assembly(Assembly assembly)
        {
            m_assembly = assembly;
        }

        public void ConfigurationFile(string configurationFile)
        {
            m_configurationFile = configurationFile;
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
            _cacheCurrentMapping = true;
            return this;
        }

        public AutoPersistanceModel WriteMappingsTo(string filePath)
        {
            m_mappingsFilePath = filePath;
            return this;
        }

        /// <summary>
        /// This will build the model for persisting entities to the indicated data store.
        /// </summary>
        public void Build()
        {
            if(_isBuilt) return;

            m_strategy.Initialize();

            if (!string.IsNullOrEmpty(m_configurationFile))
            {
                m_strategy.Configuration.Configure(m_configurationFile);
            }
            else
            {
                m_strategy.Configuration.Configure(_default_nhibernate_config_file);
                this.m_configurationFile = _default_nhibernate_config_file;
            }

            // grab the list of persistent entities from the domain model:
            m_entities = ORMUtils.FindAllEntities(m_convention, m_assembly);

            // if the maps have been created and the cache option is set, then use the existing maps:
            if (_maps.Count > 0 & _cacheCurrentMapping)
                return;

            // build the maps for the persistent entities:
            IList<string> documents = new List<string>();
            string combine = string.Empty;

            _maps.Clear();

            foreach (Type entity in m_entities)
            {

                // build the mapping:
                ICanBuildEntityDefinition entityBuilider = new EntityBuilder(m_convention, entity);
                string map = entityBuilider.Build();
                documents.Add(map);

                WriteEntityMapToFileFor(entity, map);

                if (_renderMappingPerEntity)
                    combine += map;

            }

            if (!_renderMappingPerEntity)
            {
                _maps.Add(BuildNHibernateShell(m_entities[0], combine));
            }
            else
            {
                foreach (var map in documents)
                {
                    _maps.Add(BuildNHibernateShell(m_entities[0], map));
                }
            }
           
            // create the NHibernate session factory with the created maps:
            if(string.IsNullOrEmpty(m_mappingsFilePath))
            {
                foreach (string map in _maps)
                {
                    try
                    {
                        var entityMap = new XmlDocument();
                        entityMap.LoadXml(map);
                        m_strategy.Configuration.AddDocument(entityMap);
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Configure()::LoadMap -  Error: {0} \r\n Map : {1}", exc.ToString(), map));
                        throw;
                    }
                }
            }

            // add the assembly containing the entities to persist to the strategy configuration:
            m_strategy.Configuration.AddAssembly(m_assembly);

            //_configuration.AddAssembly(_assembly); 

            try
            {
                if (m_sessionFactory == null)
                    m_sessionFactory = m_strategy.Configuration.BuildSessionFactory();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Configure()::CreateSessionFactory - Error: {0}", exc.ToString()));
                throw;
            }

            _isBuilt = true;

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
                if (m_strategy.Configuration != null)
                {
                    NHibernate.Tool.hbm2ddl.SchemaExport exporter = new NHibernate.Tool.hbm2ddl.SchemaExport(m_strategy.Configuration);
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
                if (m_strategy.Configuration != null)
                {
                    NHibernate.Tool.hbm2ddl.SchemaExport exporter = new NHibernate.Tool.hbm2ddl.SchemaExport(m_strategy.Configuration);
                    exporter.Execute(true, true, true);
                }

                if (m_sessionFactory != null)
                {
                    m_sessionFactory.Close();
                    m_sessionFactory.Dispose();
                    m_sessionFactory = null;
                }

                _isSchemaDropped = true;
            }
            catch (Exception exc)
            {
                string msg = "An error has occurred while attempting to drop the schema for the entities. Reason: " + exc.ToString();
                throw new Exception(msg, exc);
            }
        }

        private string BuildNHibernateShell(Type entity, string entityDefinition)
        {
            StringBuilder document = new StringBuilder();
            document.Append("<hibernate-mapping");
            document.Append(ORMUtils.BuildAttribute("xmlns", "urn:nhibernate-mapping-2.2"));
            document.Append("\n").Append("\t");
            document.Append(ORMUtils.BuildAttribute("assembly", entity.Assembly.GetName().Name));
            document.Append(ORMUtils.BuildAttribute("namespace", entity.Namespace));
            document.Append("\n").Append("\t");
            document.Append(ORMUtils.BuildAttribute("default-lazy", "false"));
            document.Append(">");
            document.Append("\r\n\r\n");
            document.Append(entityDefinition);
            document.Append("\r\n\r\n");
            document.Append("</hibernate-mapping>");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(document.ToString());

            return xmlDoc.OuterXml;
        }

        private void WriteEntityMapToFileFor(Type entity, string contents)
        {
            string fileName = "{0}.hbm.xml";

            if (string.IsNullOrEmpty(m_mappingsFilePath))
                return;

            if (!Directory.Exists(m_mappingsFilePath))
                throw new ArgumentException("The directory [" + m_mappingsFilePath + "] was not found to place the mapping files.");

            string file = Path.Combine(m_mappingsFilePath, string.Format(fileName, entity.Name));

            try
            {
                File.Delete(file);
                using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
                {
                    byte[] data = ASCIIEncoding.ASCII.GetBytes(BuildNHibernateShell(entity, contents));
                    fs.Write(data, 0, data.Length);
                }

            }
            catch (Exception exc)
            {
                throw;
            }

        }

        private Convention DefaultConvention()
        {
            Convention convention = new Convention();
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