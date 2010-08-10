using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define foreign key names that NHibernate will use to associate one entity with another.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="Convention"/> for model generation.</typeparam>
    public class ForeignKeyNamingConvention<T>
    {
        private T _reference = default(T);
        private bool _canRenderAsParentEntityHasPluralizedChildEntities = false;
        private bool _canRenderAsParentEntityHasInstancesOfChildEntity = false;

        public ForeignKeyNamingConvention(T reference)
        {
            _reference = reference;
        }

        public bool CanRenderAsParentEntityHasPluralizedChildEntities
        {
            get { return _canRenderAsParentEntityHasPluralizedChildEntities; }
        }

        public bool CanRenderAsParentEntityHasInstancesOfChildEntity
        {
            get { return _canRenderAsParentEntityHasInstancesOfChildEntity; }
        }

        /// <summary>
        /// Sets all foreign key names as "fk_[parent-entity]_has_[child-entities]"
        /// </summary>
        /// <returns></returns>
        public T RenderAsParentEntityHasPluralizedChildEntities()
        {
            _canRenderAsParentEntityHasPluralizedChildEntities = true;
            _canRenderAsParentEntityHasInstancesOfChildEntity = false;
            return _reference;
        }

        /// <summary>
        /// Sets all foreign key names as "fk_[parent-entity]_has_instances_of_[child-entity]"
        /// </summary>
        /// <returns></returns>
        public T RenderAsParentEntityHasInstancesOfChildEntity()
        {
            _canRenderAsParentEntityHasInstancesOfChildEntity = true;
            _canRenderAsParentEntityHasPluralizedChildEntities = false;
            return _reference;
        }
    }
}