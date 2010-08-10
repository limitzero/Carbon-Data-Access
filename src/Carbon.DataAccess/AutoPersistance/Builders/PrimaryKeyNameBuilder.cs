using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Repository.AutoPersistance.Core;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public class PrimaryKeyNameBuilder : ICanBuildPrimaryKeyName
    {
        private Convention m_convention = null;
        private Type m_entity = null;

        public PrimaryKeyNameBuilder()
            : this(null, null)
        {

        }

        public PrimaryKeyNameBuilder(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
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
                return m_entity;
            }
            set
            {
                m_entity = value;
            }
        }

        public string Build()
        {
            string retval = string.Empty;

            if (m_convention.PrimaryKey.IsEntityNameFollowedByID)
                retval = string.Concat(m_entity.Name, "ID");

            if (m_convention.PrimaryKey.IsLowerCaseEntityNameFollowedByID)
                retval = string.Concat(m_entity.Name.ToLower(), "ID");

            if (m_convention.PrimaryKey.IsLowerCasePKUnderscoreEntityName)
                retval = string.Concat("pk_", m_entity.Name);

            if (m_convention.PrimaryKey.IsLowerCasePKUnderscoreEntityNameUnderscoreID)
                retval = string.Concat("pk_", m_entity.Name, "_ID");

            return retval;
        }

        #endregion

    }
}