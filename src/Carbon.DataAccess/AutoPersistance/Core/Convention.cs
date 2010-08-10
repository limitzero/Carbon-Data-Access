using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This will set the convetions that can be used to create the configuration for entities for use by NHibernate.
    /// </summary>
    public class Convention
    {
        private bool _pluralizeTableNames = false;
        private PrimaryKeyColumnConvention<Convention> _primaryKey = null;
        private PropertyConvention<Convention> _property = null;
        private MemberAccessConvention<Convention> _memberAccessConvention = null;
        private ForeignKeyNamingConvention<Convention> _foreignKey = null;
        private ManyToManyNamingConvention<Convention> _manyToManyNamingConvention = null;

        /// <summary>
        /// Read-only. Indicator to whether the table names will be pluralized in the repository.
        /// </summary>
        public bool CanPluralizeTableNames
        {
            get { return _pluralizeTableNames; }
        }

        /// <summary>
        /// Sets the value for searching on entities for the property that represents the primary key. 
        /// </summary>
        /// <param name="name">Property name that represents the primary key for each entity.</param>
        /// <returns></returns>
        public PrimaryKeyColumnConvention<Convention> PrimaryKey
        {
            get { return _primaryKey; }
        }

        /// <summary>
        /// Read-only. Sets the naming strategy for foreign key names for relationships between entities.
        /// </summary>
        /// <returns></returns>
        public ForeignKeyNamingConvention<Convention> ForeignKey
        {
            get { return _foreignKey; }
        }

        /// <summary>
        /// Read-only. Sets the convention for entity properties.
        /// </summary>
        /// <returns></returns>
        public PropertyConvention<Convention> Property
        {
            get { return _property; }
        }

        /// <summary>
        /// Read-only. Sets the convention for access entity data members.
        /// </summary>
        /// <returns></returns>
        public MemberAccessConvention<Convention> MemberAccess
        {
            get { return _memberAccessConvention; }
        }

        /// <summary>
        /// Read-only. Sets the naming strategy for join tables in the many-to-many relationships between entities:
        /// </summary>
        public ManyToManyNamingConvention<Convention> ManyToManyTableName
        {
            get { return _manyToManyNamingConvention; }
        }

        public Convention()
        {
            _primaryKey = new PrimaryKeyColumnConvention<Convention>(this);
            _property = new PropertyConvention<Convention>(this);
            _memberAccessConvention = new MemberAccessConvention<Convention>(this);
            _foreignKey = new ForeignKeyNamingConvention<Convention>(this);
            _manyToManyNamingConvention = new ManyToManyNamingConvention<Convention>(this);
        }

        /// <summary>
        /// Sets the indicator to establish whether or not table names will be pluralized in the repository.
        /// </summary>
        /// <returns></returns>
        public Convention PluralizeTableNames()
        {
            _pluralizeTableNames = true;
            return this;
        }

    }
}