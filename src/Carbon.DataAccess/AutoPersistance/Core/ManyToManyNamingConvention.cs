using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.Repository.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define many-to-many relationships that NHibernate will use to persist values of the entity.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="Convention"/> for model generation.</typeparam>
    public class ManyToManyNamingConvention<T>
    {
        private bool _createAsSet = false;
        private bool _createWithParentEntityNameConcatenatedWithChildEntityNameNotPluralized = false;
        private bool _createWithParentEntityNameConcatenatedWithChildEntityNamePluralized = false;

        private T _reference = default(T);

        public bool CreateAsSet
        {
            get { return _createAsSet; }
        }

        public bool CreateWithParentEntityNameConcatenatedWithChildEntityNamePluralized
        {
            get { return _createWithParentEntityNameConcatenatedWithChildEntityNamePluralized; }
        }

        public bool CreateWithParentEntityNameConcatenatedWithChildEntityNameNotPluralized
        {
            get { return _createWithParentEntityNameConcatenatedWithChildEntityNameNotPluralized; }
        }

        public ManyToManyNamingConvention(T reference)
        {
            _reference = reference;
        }

        public T RenderAsParentEntityNameConcatenatedWithChildEntityNameNotPluralized()
        {
            _createWithParentEntityNameConcatenatedWithChildEntityNameNotPluralized = true;
            return _reference;
        }

        public T RenderAsParentEntityNameConcatenatedWithChildEntityNamePluralized()
        {
            _createWithParentEntityNameConcatenatedWithChildEntityNamePluralized = true;
            return _reference;
        }

        public T RenderAsSet()
        {
            _createAsSet = true;
            return _reference;
        }

    }
}