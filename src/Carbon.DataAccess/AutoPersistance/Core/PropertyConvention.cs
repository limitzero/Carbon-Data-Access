using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define entity properties that NHibernate will use to persist values of the entity.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="Convention"/> for model generation.</typeparam>
    public class PropertyConvention<T>
    {
        private T _reference = default(T);
        private bool _canRenderAsLowerCase = false;
        private int _defaultTextFieldLength = 255;
        private int _largeTextFieldLength = 1000;
        private IList<string> _largeTextFieldNames = new List<string>();

        public bool CanRenderAsLowerCase
        {
            get { return _canRenderAsLowerCase; }
        }

        public int DefaultTextFieldLength
        {
            get { return _defaultTextFieldLength; }
        }

        public int LargeTextFieldLength
        {
            get { return _largeTextFieldLength; }
        }

        public IList<string> LargeTextFieldNames
        {
            get { return ((List<string>)_largeTextFieldNames).AsReadOnly(); }
        }

        public PropertyConvention(T reference)
        {
            _reference = reference;
        }

        public T SetDefaultTextFieldLength(int length)
        {
            _defaultTextFieldLength = length;
            return _reference;
        }

        public T SetLargeTextFieldLengthsAndNames(int length, params string[] largeTextFieldNames)
        {
            _largeTextFieldLength = length;
            _largeTextFieldNames = new List<string>(largeTextFieldNames);
            return _reference;
        }

        public T RenderAsLowerCaseInRepository()
        {
            _canRenderAsLowerCase = true;
            return _reference;
        }
    }
}