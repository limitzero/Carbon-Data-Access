using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    /// <summary>
    /// Basic contract for all builders.
    /// </summary>
    public interface IBuilder
    {

        /// <summary>
        /// The current set of conventions used to define how items are constructed.
        /// </summary>
        ModelConvention Convention { get;  set;}

        /// <summary>
        /// The current type for the entity to initiate the bulding process for.
        /// </summary>
        System.Type Entity { get; set;}

        /// <summary>
        /// This will execute the building part for the concrete instance.
        /// </summary>
        /// <returns></returns>
        string Build();
    }
}