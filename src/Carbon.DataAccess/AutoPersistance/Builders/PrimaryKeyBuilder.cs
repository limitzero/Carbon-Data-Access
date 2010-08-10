using System;
using System.Collections.Generic;
using System.Text;
using Carbon.Repository.AutoPersistance.Core;
using Giza.ORM.ForNHibernate.Builders;

namespace Carbon.Repository.AutoPersistance.Builders
{
    public class PrimaryKeyBuilder : ICanBuildPrimaryKey
    {
        private ICanBuildPrimaryKeyName m_builder = null;
        private Convention m_convention = null;
        private Type m_entity = null;

        public PrimaryKeyBuilder()
            : this(null, null)
        {

        }

        public PrimaryKeyBuilder(Convention convention, Type entity)
        {
            m_convention = convention;
            m_entity = entity;
            m_builder = new PrimaryKeyNameBuilder(m_convention, m_entity);
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
            StringBuilder results = new StringBuilder();

            if (m_entity.GetProperty(m_convention.PrimaryKey.PrimaryKeyName) != null)
            {
                results.Append("<id");
                results.Append(ORMUtils.BuildAttribute("name", m_convention.PrimaryKey.PrimaryKeyName));
                results.Append(ORMUtils.BuildAttribute("column", m_builder.Build()));
                results.Append(ORMUtils.BuildAttribute("type", m_entity.GetProperty(m_convention.PrimaryKey.PrimaryKeyName).PropertyType.Name));
                results.Append(ORMUtils.BuildAttribute("access", m_convention.PrimaryKey.MemberAccess.Strategy));
                results.Append(">").Append("\r\n");
                results.Append("<generator class=\"identity\"/>");
                results.Append("\r\n");
                results.Append("</id>").Append("\r\n");
            }

            return results.ToString();
        }

        #endregion

    }
}