using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define primary key for each entity that NHibernate will use to for persistance and retreival.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="Convention"/> for model generation.</typeparam>
    public class PrimaryKeyColumnConvention<T>
    {
        private string _primaryKeyPropertyName = string.Empty;
        private bool _isLowerCaseEntityNameFollowedByID = false;  //employeeID
        private bool _isEntityNameFollowedByID = false;   //EmployeeID
        private bool _isLowerCasePKUnderscoreEntityName = false; // pk_Employee
        private bool _isLowerCasePKUnderscoreEntityNameUnderscoreID = false; // pk_Employee_ID
        private MemberAccessConvention<PrimaryKeyColumnConvention<T>> _memberAccessConvention = null;

        private T _reference = default(T);

        /// <summary>
        /// Read-Only. Value to search for on entities as a reference for the primary key.
        /// </summary>
        public string PrimaryKeyName
        {
            get { return _primaryKeyPropertyName; }
        }

        public PrimaryKeyColumnConvention(T reference)
        {
            _reference = reference;
            _memberAccessConvention = new MemberAccessConvention<PrimaryKeyColumnConvention<T>>(this);
        }


        /// <summary>
        /// Sets the value for searching on entities for the property that represents the primary key. 
        /// </summary>
        /// <param name="name">Property name that represents the primary key for each entity.</param>
        /// <returns></returns>
        public T PropertyName(string name)
        {
            _primaryKeyPropertyName = name;
            return _reference;
        }

        public bool IsLowerCaseEntityNameFollowedByID
        {
            get { return _isLowerCaseEntityNameFollowedByID; }
        }

        public bool IsEntityNameFollowedByID
        {
            get { return _isEntityNameFollowedByID; }
        }

        public bool IsLowerCasePKUnderscoreEntityName
        {
            get { return _isLowerCasePKUnderscoreEntityName; }
        }

        public bool IsLowerCasePKUnderscoreEntityNameUnderscoreID
        {
            get { return _isLowerCasePKUnderscoreEntityNameUnderscoreID; }
        }

        /// <summary>
        /// Read-only. Sets the convention for access entity data members.
        /// </summary>
        /// <returns></returns>
        public MemberAccessConvention<PrimaryKeyColumnConvention<T>> MemberAccess
        {
            get { return _memberAccessConvention; }
        }

        public T RenderAsLowerCaseEntityNameFollowedByID()
        {
            _isLowerCaseEntityNameFollowedByID = true;
            return _reference;
        }

        public T RenderAsEntityNameFollowedByID()
        {
            _isEntityNameFollowedByID = true;
            return _reference;
        }

        public T RenderAsLowerCasePKUnderscoreEntityName()
        {
            _isLowerCasePKUnderscoreEntityName = true;
            return _reference;
        }

        public T RenderAsLowerCasePKUnderscoreEntityNameUnderscoreID()
        {
            _isLowerCasePKUnderscoreEntityNameUnderscoreID = true;
            return _reference;
        }

        public string CreateConvention(string entityname)
        {
            string retval = string.Empty;

            if (_isLowerCaseEntityNameFollowedByID)
                retval = string.Concat(entityname.ToLower(), "ID");

            if (_isEntityNameFollowedByID)
                retval = string.Concat(entityname, "ID");

            if (_isLowerCasePKUnderscoreEntityName)
                retval = string.Concat("pk_", entityname);

            if (_isLowerCasePKUnderscoreEntityNameUnderscoreID)
                retval = string.Concat("pk_", entityname, "_ID");

            return retval;
        }


    }
}