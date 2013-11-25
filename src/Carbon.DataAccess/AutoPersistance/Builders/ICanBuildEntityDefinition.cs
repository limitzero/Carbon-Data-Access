using System.Collections.Generic;

namespace NHibernate.Carbon.AutoPersistance.Builders
{
    public interface ICanBuildEntityDefinition : IBuilder
    {
        IList<System.Type> RenderedEntities { get; }
    }
}