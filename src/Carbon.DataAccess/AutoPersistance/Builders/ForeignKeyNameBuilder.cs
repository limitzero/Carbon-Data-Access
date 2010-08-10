using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Repository.AutoPersistance.Core;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public class ForeignKeyNameBuilder : ICanBuildForeignKeyName
    {
        private Convention m_convention = null;
        private Type m_parent = null;
        private Type m_child = null;

        public ForeignKeyNameBuilder(Convention convention, Type parent, Type child)
        {
            m_convention = convention;
            m_parent = parent;
            m_child = child;
        }

        #region IBuilder Members

        public Convention Convention
        {
            get
            {
                return m_convention;
            }
            set
            {
                m_convention = value;
            }
        }

        public Type Entity
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        public string Build()
        {
            string results = string.Empty;

            if (m_convention.ForeignKey.CanRenderAsParentEntityHasInstancesOfChildEntity)
            {
                results = string.Format("fk_{0}_has_instances_of_{1}", m_parent.Name, m_child.Name);
            }
            else if (m_convention.ForeignKey.CanRenderAsParentEntityHasPluralizedChildEntities)
            {
                results = string.Format("fk_{0}_has_{1}", m_parent.Name, ORMUtils.Pluralize(m_child.Name));
            }

            return results;
        }

        #endregion
    }
}