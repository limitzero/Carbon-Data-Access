using System;
using System.Configuration;
using System.IO;
using System.Web;
using NHibernate.Carbon.Repository;

namespace NHibernate.Carbon.HttpModule
{
    public class NHibernateSessionPerRequestHttpModule : IHttpModule
    {
        private string _default_nhibernate_key = "nhibernate.configuration";
        private string _default_nhibernate_file = "hibernate.cfg.xml";
        private string _session_configuration = string.Empty;

        public void Init(HttpApplication context)
        {
            // try to extract the current configuration from the app settings key:
            _session_configuration = ConfigurationManager.AppSettings[_default_nhibernate_key];

            if(_session_configuration == null)
                if (!File.Exists(Path.Combine(Environment.CurrentDirectory, _default_nhibernate_file)))
                {
                    var message =    string.Format(
                        "The NHibernate configuration could not be found at the user-supplied key of '{0}'. " +
                        "Please specify this key in the appSettings portion of the web.config file and provide a valid path to the NHibernate configuration file or " +
                        "place the configuration file named '{1}' in the bin directory of the web application. ",
                        _default_nhibernate_key,
                        _default_nhibernate_file);

                    throw new ArgumentException(message);
                }
                else
                {
                    _session_configuration = Path.Combine(Environment.CurrentDirectory, _default_nhibernate_file);
                }

            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
            
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            if (NHibernateSessionManager.Instance.GetSessionFor(_session_configuration) != null)
                NHibernateSessionManager.Instance.CloseSessionFor(_session_configuration);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            NHibernateSessionManager.Instance.GetSessionFor(_session_configuration);
        }



    }
}