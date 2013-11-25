namespace NHibernate.Carbon.AutoPersistance.Core
{
	public class VersioningConvention<T>
	{
		private T _reference = default(T);

		public VersioningConvention(T reference)
		{
			_reference = reference;
		}

		public VersioningConvention<T> PropertyNameAndUnsavedValue(string versionPropertyName, object unsavedValue = null)
		{
			this.VersionPropertyName = versionPropertyName;
			this.UnsavedValue = unsavedValue;
			return this;
		}

		public object UnsavedValue { get; private set; }

		public string VersionPropertyName { get; private set; }
	}
}