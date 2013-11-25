using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    /// <summary>
    /// Contract for constructing properties on the entity
    /// </summary>
    public interface ICanBuildProperty  : IBuilder
    {
        /// <summary>
        /// The list of properties that need to be examined for other relationships.
        /// </summary>
        IList<PropertyInfo> ExcludedProperties { get; }


        /// <summary>
        /// The list of properties  not to render:
        /// </summary>
        IList<PropertyInfo> PropertiesNotToRender { set;}
    }
}