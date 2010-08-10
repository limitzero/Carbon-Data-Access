using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using Carbon.Repository.AutoPersistance.Persistance;
using NHibernate.Cfg;

namespace Carbon.DataAccess.Tests
{
    public class SqlCePersistanceStrategy : IPersistanceStrategy
    {
        private Configuration m_configuration = null;
        private string m_databaseName = string.Empty;

        public SqlCePersistanceStrategy(string databaseName)
        {
            m_databaseName = databaseName;
        }

        #region IPersistanceStrategy Members

        public NHibernate.Cfg.Configuration Configuration
        {
            get { return m_configuration; }
            set { m_configuration = value; }
        }

        public void Initialize()
        {
            #region -- create the local in-memory database (if needed)--

            if (string.IsNullOrEmpty(m_databaseName))
                m_databaseName = "local";

            if (!m_databaseName.Contains(".sdf"))
                m_databaseName = string.Concat(m_databaseName, ".sdf");

            string database = string.Format("Data Source={0}", m_databaseName);


            var properties = new Dictionary<string, string>();
            properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
            properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlServerCeDriver");
            properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSqlCeDialect");
            properties.Add("hibernate.connection.connection_string", database);
            properties.Add("hibernate.connection.isolation", "ReadCommitted");
            properties.Add("hibernate.show_sql", "true");

            m_configuration = new Configuration();
            m_configuration.Properties = properties;

            try
            {
                if (m_databaseName.Contains("local.sdf"))
                {
                    try
                    {
                        File.Delete(Path.Combine(System.Environment.CurrentDirectory, "local.sdf"));
                    }
                    catch { }
                }

                SqlCeEngine engine = new SqlCeEngine(database);
                engine.CreateDatabase();
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("Error creating SqlCe database. Reason: " + exc.Message);
            }

            #endregion
        }

        #endregion
    }
}