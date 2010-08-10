using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This is the general access strategies that NHibernate will use to load the data to the associated field or property on the referenced object.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="Convention"/> for model generation.</typeparam>
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

        public T NoSetterCamelCaseUnderscore()
        {
            _access = "nosetter.camelcase-underscore";
            return _reference;
        }

        public T NoSetterLowerCase()
        {
            _access = "nosetter.lowercase";
            return _reference;
        }

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