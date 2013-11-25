namespace NHibernate.Carbon.AutoPersistance.Core
{
    /// <summary>
    /// Contract for specifiying how the peristance model will be created for the entities.
    /// </summary>
    public interface IPersistanceModelProvider
    {
        /// <summary>
        /// This will return the model for persisting/retreiving entities based on 
        /// conventions and model behavior.
        /// </summary>
        /// <param name="conventionProvider">The current set of convetions for creating the entity mapping for persistance.</param>
        /// <returns></returns>
        AutoPersistanceModel GetModel(IConventionProvider conventionProvider);
    }
}