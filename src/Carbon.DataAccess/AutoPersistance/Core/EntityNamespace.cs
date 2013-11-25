namespace NHibernate.Carbon.AutoPersistance.Core
{
	/// <summary>
	/// Class for defining how to search for entities to be collected for auto-mapping by namespace delineation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
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

		/// <summary>
		/// This will look in the entity assembly for all entities that end with the particular sub-namespace name 
		/// for creating the set of mappings for auto-persistance. 
		/// Ex: Supplying the "Entities" value for the parent assembly of MyNamespace.Entities will only look 
		/// for entities that end with the text "Entities" despite other classes being part of the entire namespace.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public T EndsWith(string value)
        {
            _endsWith = value;
            return _reference;
        }

		/// <summary>
		/// This will look in the entity assembly for all entities that start with the particular sub-namespace name 
		/// for creating the set of mappings for auto-persistance. 
		/// Ex: Supplying the "MyNamespace.Entities" value for the parent assembly of MyNamespace.Entities will only look 
		/// for entities that start with the text "MyNamespace.Entities" despite other classes being part of the entire namespace.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public T StartsWith(string value)
        {
            _startsWith = value;
            return _reference;
        }

		/// <summary>
		/// This will look in the entity assembly for all entities that contain  the particular sub-namespace name 
		/// for creating the set of mappings for auto-persistance. 
		/// Ex: Supplying the "Entities" value for the parent assembly of MyNamespace.Entities will only look 
		/// for entities that contain the text "Entities" as part of the namespace. This is less restrictive and may lead 
		/// to classes being auto-mapped that are not desired.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
        public T Contains(string value)
        {
            _contains = value;
            return _reference;
        }

		/// <summary>
		/// This will clear the references to the sub-namespaces to search for in creating the mapping files for persisted entities.
		/// </summary>
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