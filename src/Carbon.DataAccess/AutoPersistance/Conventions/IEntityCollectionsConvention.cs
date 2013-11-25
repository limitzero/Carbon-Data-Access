namespace NHibernate.Carbon.AutoPersistance.Conventions
{
	/// <summary>
	/// Convention that will allow a specific entity to control how its collections are rendered.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IEntityCollectionsConvention<TEntity> where TEntity : class
	{
		EntityCollectionConventionTypes Execute(TEntity entity);
	}
}