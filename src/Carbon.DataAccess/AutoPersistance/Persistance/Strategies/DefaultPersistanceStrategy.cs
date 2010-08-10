using System;
using NHibernate.Cfg;

namespace Carbon.Repository.AutoPersistance.Persistance.Strategies
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

            try
            {
                m_configuration = new NHibernate.Cfg.Configuration();
            }
            catch (Exception exception)
            {                
                throw;
            }
        }

        #endregion
    }
}