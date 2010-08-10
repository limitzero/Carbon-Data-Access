using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    public class EntityModel
    {
        private Type _entity = null;
        private IList<Type> _implementedentities = new List<Type>(); 

        public EntityModel(Type entity, IList<Type> implementedEntities)
        {
            _entity = entity;
            _implementedentities = implementedEntities;
        }

        public Type Entity
        {
            get { return _entity; }
        }

        public IList<Type> ImplementedEntities
        {
            get { return _implementedentities; }
        }


    }
}