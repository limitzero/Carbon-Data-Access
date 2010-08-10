using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// Contract for building relationships between entities
    /// </summary>
    public interface IRelationship
    {
        /// <summary>
        /// This will return the relationship of one entity to others within the model.
        /// </summary>
        /// <returns></returns>
        string Build();
    }
}