using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Repository.AutoPersistance.Core;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Builders
{
    /// <summary>
    /// Basic contract for all builders.
    /// </summary>
    public interface IBuilder
    {

        /// <summary>
        /// The current set of conventions used to define how items are constructed.
        /// </summary>
        Convention Convention { get;  set;}

        /// <summary>
        /// The current type for the entity to initiate the bulding process for.
        /// </summary>
        Type Entity { get; set;}

        /// <summary>
        /// This will execute the building part for the concrete instance.
        /// </summary>
        /// <returns></returns>
        string Build();
    }
}