using System;
using Carbon.Repository.AutoPersistance.Persistance;
using NHibernate;
using System.Reflection;
using NHibernate.Cfg;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// Local set up fixture for testing interactions  with a database via NHibernate settings.
    /// </summary>
    public abstract class AbstractAutoPersistanceModel
    {

        #region -- private variables --
        private IPersistanceStrategy m_strategy = null;
        private static ISessionFactory m_sessionFactory = null;
        private static IAutoPersistanceModel m_model = null;
        #endregion

        /// <summary>
        /// Sets the persistance strategy to use while testing interaction with the persistance store.
        /// </summary>
        /// <param name="strategy"></param>
        public void SetPersistanceStrategyUsing(IPersistanceStrategy strategy)
        {
            m_strategy = strategy;
        }

        #region -- public methods --
        /// <summary>
        /// This will configure NHibernate with the settings as kept in the configuration file for NHibernate.
        /// </summary>
        public void OneTimeInitialize(IPersistanceStrategy strategy, string assembly)
        {
            Assembly asm = Assembly.Load(assembly);
            OneTimeInitialize(strategy, asm);
        }

        /// <summary>
        /// This will configure NHibernate with the settings as kept in the configuration file for NHibernate.
        /// </summary>
        public void OneTimeInitialize(IPersistanceStrategy strategy, params Assembly[] assemblies)
        {
            // configure NHibernate based on the assemblies:
            try
            {
                m_strategy = strategy;
                foreach (Assembly assembly in assemblies)
                {
                    m_strategy.Configuration.AddAssembly(assembly);
                }

                m_model = null;
                m_sessionFactory = m_strategy.Configuration.BuildSessionFactory();

            }
            catch (Exception exc)
            {
                string msg = "An error has occurred while attempting to initialize the persistance strategy. Reason: " + exc.Message;
                throw new Exception(msg, exc);
            }
        }

        /// <summary>
        /// This will configure NHibernate for testing based on the configuration of the auto-persist model.
        /// </summary>
        /// <param name="model"></param>
        public void OneTimeInitializeWithModel(IAutoPersistanceModel model)
        {
            // configure NHibernate based on the assemblies:
            try
            {
                // configure the model then initialize the persistance strategy:
                model.Build();
                m_model = model;
                m_sessionFactory = null;
                System.Diagnostics.Debug.WriteLine("Entity Mapping: " + model.GetMaps());
            }
            catch (Exception exc)
            {
                string msg = "An error has occurred while attempting to initialize the model based on the persistance strategy. Reason: " + exc.Message;
                throw new Exception(msg, exc);
            }
        }

        /// <summary>
        /// Exposes the NHibernate session factory.
        /// </summary>
        /// <returns></returns>
        public static ISessionFactory GetSessionFactory()
        {
            ISessionFactory factory = null;

            if (m_model != null)
            {
                factory = m_model.CurrentSessionFactory;
            }
            else
            {
                factory = m_sessionFactory;
            }

            return factory;
        }

        private Configuration GetConfiguration()
        {
            Configuration configuration = null;

            if (m_model != null)
            {
                configuration = m_model.CurrentConfiguration;
            }
            else
            {
                configuration = m_strategy.Configuration;
            }

            return configuration;
        }


        #endregion

        #region -- nested classes --
        /// <summary>
        /// This is the data persistance and retreival object for testing interactions via NHibernate (for testing only)
        /// </summary>
        public class Repository
        {

            #region -- local variables --
            private static object[] m_saveOperations = new object[] { };
            private static object[] m_deleteOperations = new object[] { };
            #endregion

            static Repository()
            { }

            /// <summary>
            /// Retrieves an object based on type as indicated by 
            /// the given object key.
            /// </summary>
            /// <typeparam name="T">Type of the object to be returned and retrieved</typeparam>
            /// <param name="key">Key used to fetch the object.</param>
            /// <returns></returns>
            public static T FindById<T>(object key)
            {
                T retval = default(T);

                try
                {
                    using (ISession session = GetSessionFactory().OpenSession())
                    {
                        retval = session.Get<T>(key);
                    }
                }
                catch (Exception exc)
                {
                    string msg = "An error has occurred while attempting to retrieve object [" +
                                 default(T).GetType().FullName + "] with the key of [" +
                                 key.ToString() +
                                 "] for the current unit of work session. Reason: " + exc.Message;
                    throw;
                }

                return retval;
            }

            /// <summary>
            /// Saves a single object for persistance.
            /// </summary>
            /// <typeparam name="T">Type of object to be persisted</typeparam>
            /// <param name="currentObject">Current object to be persisted.</param>
            public static void Save<T>(object currentObject)
            {
                RegisterSave(currentObject);
                Commit();
            }

            /// <summary>
            /// Deletes a single object from persistance store.
            /// </summary>
            /// <typeparam name="T">Type of object to be deleted</typeparam>
            /// <param name="currentObject">Current object to be deleted.</param>
            public static void Delete<T>(object currentObject)
            {
                RegisterDelete(currentObject);
                Commit();
            }
            
            /// <summary>
            /// Exposes the NHibernate session factory.
            /// </summary>
            /// <returns></returns>
            public static ISessionFactory GetSessionFactory()
            {
                ISessionFactory factory = null;

                if (m_model != null)
                {
                    factory = m_model.CurrentSessionFactory;
                }
                else
                {
                    factory = m_sessionFactory;
                }

                return factory;
            }

            /// <summary>
            /// The Commit method persists all changes to the repository in order 
            /// as they were received.
            /// </summary>
            private static void Commit()
            {
                try
                {
                    using (ISession session = GetSessionFactory().OpenSession())
                    {
                        using (ITransaction txn = session.BeginTransaction())
                        {

                            if (m_saveOperations.Length > 0)
                            {
                                foreach (object saveOperation in m_saveOperations)
                                {
                                    if (saveOperation != null)
                                        session.SaveOrUpdate(saveOperation);
                                }
                            }

                            if (m_deleteOperations.Length > 0)
                            {
                                foreach (object deleteOperation in m_deleteOperations)
                                {
                                    if (deleteOperation != null)
                                        session.Delete(deleteOperation);
                                }
                            }

                            txn.Commit();
                        }

                    }

                }
                catch (Exception exc)
                {
                    string msg = "An error has occurred while attempting to commit the unit of work for the session for list of objects. Reason: " + exc.Message;
                    throw new Exception(msg, exc);
                }
                finally
                {
                    Clear();
                }
            }

            /// <summary>
            /// Registers a series of objects to be updated or inserted 
            /// into the repository based on the order supplied.
            /// </summary>
            /// <param name="objects">A list of objects to be persisted.</param>
            private static void RegisterSave(params object[] objects)
            {
                if (objects.Length > 0)
                {
                    m_saveOperations = objects;
                }
            }

            /// <summary>
            /// Registers a series of objects to be removed from 
            /// the repository based on the order supplied.
            /// </summary>
            /// <param name="objects">A list of objects to be removed.</param>
            private static void RegisterDelete(params object[] objects)
            {
                if (objects.Length > 0)
                {
                    m_deleteOperations = objects;
                }
            }

            /// <summary>
            /// The Clear method removes all registered save and delete operations
            /// from memory for persistance.
            /// </summary>
            private static void Clear()
            {
                m_saveOperations = new object[] { };
                m_deleteOperations = new object[] { };
            }

        }

      
        #endregion

    }
}