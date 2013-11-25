namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// This is the general access strategies that NHibernate will use to load the data to the associated field or property on the referenced object.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="ModelConvention"/> for model generation.</typeparam>
    public class MemberAccessConvention<T>
    {
        private T _reference = default(T);

        public MemberAccessConvention(T reference)
        {
            _reference = reference;
        }

        private string _access = string.Empty;
        public string Strategy
        {
            get { return _access; }
        }

        public T Default()
        {
            _access = string.Empty;
            return _reference;
        }

        public T FieldCamelCase()
        {
            _access = "field.camelcase";
            return _reference;
        }

        public T FieldCamelCaseUnderscore()
        {
            _access = "field.camelcase-underscore";
            return _reference;
        }

        public T FieldLowerCase()
        {
            _access = "field.lowercase";
            return _reference;
        }

        public T FieldLowerCaseUnderscore()
        {
            _access = "field.lowercase-underscore";
            return _reference;
        }

        public T FieldPascalCase()
        {
            _access = "field.pascalcase";
            return _reference;
        }

        public T FieldPascalCase_m()
        {
            _access = "field.pascalcase-m";
            return _reference; ;
        }

        public T FieldPascalCaseUnderscore()
        {
            _access = "field.pascalcase-underscore";
            return _reference;
        }

        public T FieldPascalCase_m_Underscore()
        {
            _access = "field.pascalcase-m-underscore";
            return _reference;
        }

        public T NoSetterCamelCase()
        {
            _access = "nosetter.camelcase";
            return _reference;
        }

		/// <summary>
		/// This will set the member access as _{lower case property name}.  
		/// Ex: 
		/// private string _firstName; // member accessed to set value
		/// public string FirstName {get {return _firstName}; private {_firstName = value};}
		/// </summary>
		/// <returns></returns>
        public T NoSetterCamelCaseUnderscore()
        {
            _access = "nosetter.camelcase-underscore";
            return _reference;
        }

		/// <summary>
		/// This will set the member access as {lower case property name}.  
		/// Ex: 
		/// private string name; // member accessed to set value
		/// public string Name {get {return name}; private {name = value};}
		/// </summary>
		/// <returns></returns>
        public T NoSetterLowerCase()
        {
            _access = "nosetter.lowercase";
            return _reference;
        }

		/// <summary>
		/// This will set the member access as {lower case property name}.  
		/// Ex: 
		/// private string _name; // member accessed to set value
		/// public string Name {get {return _name}; private {_name = value};}
		/// </summary>
		/// <returns></returns>
        public T NoSetterLowerCaseUnderscore()
        {
            _access = "nosetter.lowercase-underscore";
            return _reference;
        }

        public T NoSetterPascalCase()
        {
            _access = "nosetter.pascalcase";
            return _reference;
        }

        public T NoSetterPascalCase_m()
        {
            _access = "nosetter.pascalcase-m";
            return _reference;
        }

        public T NoSetterPascalCaseUnderscore()
        {
            _access = "nosetter.pascalcase-underscore";
            return _reference;
        }

        public T NoSetterPascalCase_m_Underscore()
        {
            _access = "nosetter.pascalcase-m-underscore";
            return _reference;
        }
    }
}