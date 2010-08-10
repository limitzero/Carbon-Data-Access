using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public interface ICanBuildEntityDefinition : IBuilder
    {
        IList<Type> RenderedEntities { get; }
    }
}