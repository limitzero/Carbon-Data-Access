using System;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.AutoPersistance.Persistance.Strategies;

namespace NHibernate.Carbon.AutoPersistance
{
    /// <summary>
    /// This is an example of the persistance model fixture that is 
    /// used to persist the domain model.
    /// </summary>
    public class LocalPersistanceModelProvider : IPersistanceModelProvider
    {

        public LocalPersistanceModelProvider()
        {
        }

        public AutoPersistanceModel GetModel(IConventionProvider conventionProvider)
        {

            if(conventionProvider == null)
                throw new ArgumentException("There was not an instance of a convention provider supplied for auto-persistance.");

            //change this to your persistance strategy (if needed)...
            var strategy = new DefaultPersistanceStrategy();

            var model = new AutoPersistanceModel(strategy, conventionProvider.GetConventions());

            // your changes for the model go here!!!
            model.Assembly("{Domain Assembly Name}");


            //model.Namespace.EndsWith("SampleDomain");
            //model.CacheMapping();        // cache the mappings when created

            return model;
        }

    }
}