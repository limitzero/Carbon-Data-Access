using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    public class EntityNamespace<T>
    {
        private T _reference = default(T);
        private string _contains = string.Empty;
        private string _startsWith = string.Empty;
        private string _endsWith = string.Empty;

        public EntityNamespace(T reference)
        {
            _reference = reference;
        }

        public T EndsWith(string value)
        {
            _endsWith = value;
            return _reference;
        }

        public T StartsWith(string value)
        {
            _startsWith = value;
            return _reference;
        }

        public T Contains(string value)
        {
            _contains = value;
            return _reference;
        }

        public void Reset()
        {
            _contains = string.Empty;
            _startsWith = string.Empty;
            _endsWith = string.Empty;
        }

        public bool IsMatchFor(string value)
        {
            if (_endsWith.Length > 0)
            {
                return value.EndsWith(_endsWith);
            }

            if (_startsWith.Length > 0)
            {
                return value.StartsWith(_startsWith);
            }

            if (_contains.Length > 0)
            {
                return value.Contains(_contains);
            }

            return true;
        }



    }
}