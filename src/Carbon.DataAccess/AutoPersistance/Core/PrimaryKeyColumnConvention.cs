namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define primary key for each entity that NHibernate will use to for persistance and retreival.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="ModelConvention"/> for model generation.</typeparam>
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

		/// <summary>
		/// Gets the indicator if the entity primary key name will be of the form {lower case entity name}ID
		/// </summary>
        public bool IsLowerCaseEntityNameFollowedByID
        {
            get { return _isLowerCaseEntityNameFollowedByID; }
        }

		/// <summary>
		/// Gets the indicator if the entity primary key name will be of the form {entity name}ID
		/// </summary>
        public bool IsEntityNameFollowedByID
        {
            get { return _isEntityNameFollowedByID; }
        }

		/// <summary>
		/// Gets the indicator if the entity primary key name will be of the form pk_{entity name}ID
		/// </summary>
        public bool IsLowerCasePKUnderscoreEntityName
        {
            get { return _isLowerCasePKUnderscoreEntityName; }
        }

		/// <summary>
		/// Gets the indicator if the entity primary key name will be of the form pk_{entity name}_ID
		/// </summary>
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

		/// <summary>
		/// This will render the primary key name in the data store as lower case {entity name}ID.
		/// </summary>
		/// <returns></returns>
        public T RenderAsLowerCaseEntityNameFollowedByID()
        {
            _isLowerCaseEntityNameFollowedByID = true;
            return _reference;
        }

		/// <summary>
		/// This will render the primary key name in the data store as {entity name}ID.
		/// </summary>
		/// <returns></returns>
        public T RenderAsEntityNameFollowedByID()
        {
            _isEntityNameFollowedByID = true;
            return _reference;
        }

		/// <summary>
		/// This will render the primary key name in the data store as  PK_{entity name}ID.
		/// </summary>
		/// <returns></returns>
        public T RenderAsLowerCasePKUnderscoreEntityName()
        {
            _isLowerCasePKUnderscoreEntityName = true;
            return _reference;
        }

		/// <summary>
		/// This will render the primary key name in the data store as  pk_{entity name}_ID.
		/// </summary>
		/// <returns></returns>
        public T RenderAsLowerCasePKUnderscoreEntityNameUnderscoreID()
        {
            _isLowerCasePKUnderscoreEntityNameUnderscoreID = true;
            return _reference;
        }

		/// <summary>
		/// This will render the the primary key name for the entity as determined by the convention.
		/// </summary>
		/// <param name="entityname">Name of the entity</param>
		/// <returns></returns>
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