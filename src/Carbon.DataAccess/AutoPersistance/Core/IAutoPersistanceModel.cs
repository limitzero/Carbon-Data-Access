using System;
using System.Collections.Generic;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Core
{
    public interface IAutoPersistanceModel
    {
        /// <summary>
        /// Read-only. Contains the collection of entity maps for persistance. 
        /// </summary>
        IList<string> Maps { get; }

        /// <summary>
        /// (Read-Only). Contains the collection of all entities found from the target assembly for creating 
        /// the entity relation mappings.
        /// </summary>
        ICollection<Type> Entities { get; }

        /// <summary>
        /// (Read-Only). This will return the assembly of the domain model that is inspected for auto-persistance.
        /// </summary>
        Assembly DomainModelAssembly { get; }

        /// <summary>
        /// Sets the model to emit a mapping per entity. Default is one mapping for all entities.
        /// </summary>
        bool CanRenderMappingPerEntity { get; }

        EntityNamespace<AutoPersistanceModel> Namespace { get; }

        NHibernate.Cfg.Configuration CurrentConfiguration { get; }

        NHibernate.ISessionFactory CurrentSessionFactory { get; set; }

        /// <summary>
        /// (Read-Only). Flag to indicate whether the schema has been created.
        /// </summary>
        bool IsSchemaCreated { get; }

        /// <summary>
        /// (Read-Only). Flag to indicate whether the schema has been dropped.
        /// </summary>
        bool IsSchemaDropped { get; }

        /// <summary>
        /// Retrieves all of the maps for the entities as one xml document.
        /// </summary>
        /// <returns></returns>
        string GetMaps();

        /// <summary>
        /// Retrieves a map of an entity at a specific index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetMaps(int index);

        /// <summary>
        /// This will retreive a map for a particular entity.
        /// </summary>
        /// <typeparam name="T">Current entity class reference</typeparam>
        /// <returns></returns>
        string GetMapFor<T>()
            where T : class;

        /// <summary>
        /// This will return the current configuration file used to create the NHibernate configuration context.
        /// </summary>
        /// <returns></returns>
        string GetCurrentConfigurationFile();

        /// <summary>
        /// Sets the assembly to scan for entities.
        /// </summary>
        /// <param name="assembly">Name of assembly to scan.</param>
        void Assembly(string assembly);

        /// <summary>
        /// Sets the assembly to scan for entities.
        /// </summary>
        /// <param name="assembly">Assembly of the entities.</param>
        void Assembly(Assembly assembly);

        /// <summary>
        /// This will set the configuration file needed to create the NHibernate configuration.
        /// </summary>
        /// <param name="configurationFile"></param>
        void ConfigurationFile(string configurationFile);

        /// <summary>
        /// Sets the model to emit a mapping per entity. Default is one mapping for all entities.
        /// </summary>
        void RenderMappingPerEntity();

        /// <summary>
        /// Indicator for the model to cache the entity mapping definition.
        /// </summary>
        /// <returns></returns>
        AutoPersistanceModel CacheMapping();

        /// <summary>
        /// This will tell the model inflector to write the entity mappings out to a directory.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        AutoPersistanceModel WriteMappingsTo(string filePath);

        /// <summary>
        /// This will build the model for persisting entities to the indicated data store.
        /// </summary>
        void Build();

        /// <summary>
        /// This will create the data schema for persistance based on the model in the data store.
        /// </summary>
        void CreateSchema();

        /// <summary>
        /// This will delete the data schema for persistance based on the model from the data store.
        /// </summary>
        void DropSchema();
    }
}