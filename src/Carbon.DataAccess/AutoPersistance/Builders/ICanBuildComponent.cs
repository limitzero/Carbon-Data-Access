using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public interface ICanBuildComponent : IBuilder
    {
        IList<PropertyInfo> PropertiesForInspection { set; }
    }
}