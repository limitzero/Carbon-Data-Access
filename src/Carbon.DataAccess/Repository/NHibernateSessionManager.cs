using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Carbon.Repository.AutoPersistance.Core;
using NHibernate;
using NHibernate.Cfg;

namespace Carbon.Repository.Repository
{
    /// <summary>
    /// Adapted from Billy McCaffrey's NHibernate Best Practices for use with the Auto Persistance Model
    /// for NHibernate.
    /// </summary>
    public class NHibernateSessionManager
    {
        private const string DEFAULT_NHIBERNATE_CONFIGURATION_FILE = @"hibernate.cfg.xml";
        private const string CONTEXT_SESSIONS = "context.sessions";
        private static NHibernateSessionManager _instance = null;
        private static IDictionary<string, ISessionFactory> _session_factories = null;
        private static IDictionary<string, ISession> _context_sessions = null;
        private static IDictionary<string, IAutoPersistanceModel> _context_persistance_models = null;

        /// <summary>
        /// private constructor to enforce singleton behavior
        /// </summary>
        private NHibernateSessionManager()
        {
        }

        /// <summary>
        /// Instance of thread safe singletion.
        /// </summary>
        public static NHibernateSessionManager Instance
        {
            get
            {
                return Factory.NHibernateManager;
            }
        }

        /// <summary>
        /// Factory instance to govern lifecycle of singleton session manager
        /// </summary>
        private class Factory
        {
            static Factory()
            {
                // initialize collections for storing configurations, session factories, and sessions:
                if (_session_factories == null)
                    _session_factories = new Dictionary<string, ISessionFactory>();

                if (_context_sessions == null)
                    _context_sessions = new Dictionary<string, ISession>();

                if (_context_persistance_models == null)
                    _context_persistance_models = new Dictionary<string, IAutoPersistanceModel>();
            }
            internal static readonly NHibernateSessionManager NHibernateManager = new NHibernateSessionManager();
        }

        /// <summary>
        /// This will use  the auto persistance model that has the configuration
        /// information to connect to NHibernate and read the entities for 
        /// auto configuration and persistance.
        /// </summary>
        /// <param name="model"></param>
        public void RegisterPersistanceModel(IAutoPersistanceModel model)
        {
            lock (ContextPersistanceModels)
            {
                var configFile = model.GetCurrentConfigurationFile();

                if (!ContextPersistanceModels.ContainsKey(configFile))
                {
                    model.Build();
                    GetSessionFactoryForConfiguration(model.GetCurrentConfigurationFile());
                    ContextPersistanceModels[model.GetCurrentConfigurationFile()] = model;
                }
            }
        }

        /// <summary>
        /// This will create a session from the specific NHibernate configuration
        /// </summary>
        /// <param name="configurationFile">Full path to the NHibernate configuration file.</param>
        /// <returns></returns>
        public ISession GetSessionFor(string configurationFile)
        {
            ISession session = null;

            // look in the /bin directory for the default configuration file:
            if (string.IsNullOrEmpty(configurationFile))
                configurationFile = DEFAULT_NHIBERNATE_CONFIGURATION_FILE;

            // examine the auto-persistance models:
            if (ContextPersistanceModels.ContainsKey(configurationFile))
            {
                var model = ContextPersistanceModels[configurationFile];

                if (!ContextSessions.ContainsKey(configurationFile))
                    lock (ContextSessions)
                        ContextSessions[configurationFile] = model.CurrentSessionFactory.OpenSession();
            }

            if (ContextSessions.ContainsKey(configurationFile))
                return ContextSessions[configurationFile];

            // the session does not exist, let's build the session and store it:
            var factory = GetSessionFactoryForConfiguration(configurationFile);

            if (factory == null)
                throw new ArgumentNullException("The session factory could not be found/created for the following configuration: " + configurationFile);

            session = factory.OpenSession();

            if (session == null)
                throw new ArgumentNullException("The session could not be created for the following configuration: " + configurationFile);

            // refresh the session at the given location:
            lock (ContextSessions)
                ContextSessions[configurationFile] = session;

            return ContextSessions[configurationFile];
        }

        /// <summary>
        /// This will close a session on a given configuration.
        /// </summary>
        /// <param name="configurationFile"></param>
        public void CloseSessionFor(string configurationFile)
        {
            var session = ContextSessions[configurationFile];

            if (session != null)
                if (session.IsConnected && session.IsOpen)
                {
                    session.Flush();
                    session.Close();
                    session.Dispose();

                    lock (ContextSessions)
                        ContextSessions[configurationFile] = null;
                }
        }

        public IDictionary<string, IAutoPersistanceModel> ContextPersistanceModels
        {
            get { return _context_persistance_models; }
            private set { _context_persistance_models = value; }
        }

        /// <summary>
        /// This will generate a session factory for the given NHibernate configuration.
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        private static ISessionFactory GetSessionFactoryForConfiguration(string configurationFile)
        {

            if (!File.Exists(configurationFile))
                throw new FileNotFoundException("The NHibernate configuration file could not be fould at the specified location.", configurationFile);

            // first find the factory if it exists, if it does not exists, create a new one and 
            // add it to the current set of session factories:
            if (_session_factories.ContainsKey(configurationFile)) return _session_factories[configurationFile];

            // build the session factory based on the configuration:
            var configuration = new Configuration();
            configuration.Configure(configurationFile);

            var factory = configuration.BuildSessionFactory();

            // add the current session factory to the list of existing session factories:
            lock (_session_factories)
            {
                if (!_session_factories.ContainsKey(configurationFile))
                    _session_factories.Add(configurationFile, factory);
            }

            return factory;
        }

        /// <summary>
        /// This is created for managing different connections to different databases 
        /// at one time via web or smart-client applications.
        /// </summary>
        private static IDictionary<string, ISession> ContextSessions
        {
            get
            {
                if (IsInWebContext)
                {
                    if (HttpContext.Current.Items[CONTEXT_SESSIONS] == null)
                        HttpContext.Current.Items[CONTEXT_SESSIONS] = new Dictionary<string, ISession>();
                    return HttpContext.Current.Items[CONTEXT_SESSIONS] as IDictionary<string, ISession>;
                }
                else
                {
                    // in calling context of the current thread, use the CallContext of the current executing thread:
                    if (CallContext.GetData(CONTEXT_SESSIONS) == null)
                        CallContext.SetData(CONTEXT_SESSIONS, new Dictionary<string, ISession>());
                    return CallContext.GetData(CONTEXT_SESSIONS) as IDictionary<string, ISession>;
                }
            }
        }

        /// <summary>
        /// Flag to check whether or not the session manager will be 
        /// run inside of a web context
        /// </summary>
        private static bool IsInWebContext
        {
            get { return HttpContext.Current != null; }
        }
    }
}