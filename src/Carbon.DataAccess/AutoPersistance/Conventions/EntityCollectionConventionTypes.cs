namespace NHibernate.Carbon.AutoPersistance.Conventions
{
	public enum EntityCollectionConventionTypes
	{
		/// <summary>
		/// Renders the collections on the entity as a set of unique items
		/// </summary>
		AsSet, 

		/// <summary>
		/// Renders the collections on the entity as a list of unique or non-unique items.
		/// </summary>
		AsBag,
	}
}