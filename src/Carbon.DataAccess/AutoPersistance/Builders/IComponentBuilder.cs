using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    public interface IComponentBuilder : IBuilder
    {
        IList<PropertyInfo> PropertiesForInspection { set; }
    }
}