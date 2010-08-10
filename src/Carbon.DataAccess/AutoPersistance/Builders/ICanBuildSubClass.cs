using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public interface ICanBuildSubClass   : IBuilder
    {
        /// <summary>
        /// The collection of entities that have been subclassed within the parent entity.
        /// </summary>
        IList<Type> SubClassedEntities { get;}
    }
}