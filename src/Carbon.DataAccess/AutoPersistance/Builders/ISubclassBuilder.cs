using System.Collections.Generic;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    public interface ISubclassBuilder   : IBuilder
    {
        /// <summary>
        /// The collection of entities that have been subclassed within the parent entity.
        /// </summary>
        IList<System.Type> SubClassedEntities { get;}
    }
}