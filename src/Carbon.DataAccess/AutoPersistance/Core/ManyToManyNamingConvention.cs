namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// This is the general strategy that is used to define many-to-many relationships that NHibernate will use to persist values of the entity.
    /// </summary>
    /// <typeparam name="T">Referenced convention <seealso cref="ModelConvention"/> for model generation.</typeparam>
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

		/// <summary>
		/// This will cause the collection of many to many or many to one objects to be rendered in a "set"
		/// which is a list of non-repeating elements. The data persisted in the database will have to be 
		/// resident in this fashion in order for the elements not to trigger an exception noting duplicates
		/// for the underlying data type of the property containing the listing of the elements that supports
		/// non-repeating elements (i.e. "sets").
		/// </summary>
		/// <returns></returns>
        public T RenderAsSet()
        {
            _createAsSet = true;
            return _reference;
        }

    }
}