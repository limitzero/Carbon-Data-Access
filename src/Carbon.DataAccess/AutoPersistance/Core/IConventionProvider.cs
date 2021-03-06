namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// Contract for creating the conventions for persisting objects to the data store via NHibernate auto-mappings.
    /// </summary>
    public interface IConventionProvider
    {
        /// <summary>
        /// This will retrieve the conventons for the persistance model for persistance.
        /// </summary>
        /// <returns></returns>
        ModelConvention GetConventions();
    }
}