using System.Collections.Generic;

namespace NHibernate.Carbon.AutoPersistance.Core
{
    public class EntityModel
    {
        private System.Type _entity = null;
        private IList<System.Type> _implementedentities = new List<System.Type>(); 

        public EntityModel(System.Type entity, IList<System.Type> implementedEntities)
        {
            _entity = entity;
            _implementedentities = implementedEntities;
        }

        public System.Type Entity
        {
            get { return _entity; }
        }

        public IList<System.Type> ImplementedEntities
        {
            get { return _implementedentities; }
        }


    }
}