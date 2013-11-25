using System;
using NHibernate.Cfg;

namespace NHibernate.Carbon.AutoPersistance.Persistance.Strategies
{
    /// <summary>
    /// This is the persistance strategy that will read the settings from the app.config file for NHibernate.
    /// </summary>
    public class DefaultPersistanceStrategy : IPersistanceStrategy
    {
        private Configuration m_configuration = null;

        #region IPersistanceStrategy Members

        public Configuration Configuration
        {
            get
            {
                return m_configuration;
            }
            set
            {
                m_configuration = value;
            }
        }

        public void Initialize()
        {
			m_configuration = new NHibernate.Cfg.Configuration();
        }

        #endregion
    }
}