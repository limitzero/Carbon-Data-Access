using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Cfg;

namespace Carbon.Repository.AutoPersistance.Persistance
{
    public interface IPersistanceStrategy
    {
        /// <summary>
        /// This is the NHibernate configuration that holds the information to attach to the persistance store.
        /// </summary>
        Configuration Configuration { get;  set;}

        /// <summary>
        /// This will configure the strategy to communicate with the persistance store.
        /// </summary>
        void Initialize();

    }
}