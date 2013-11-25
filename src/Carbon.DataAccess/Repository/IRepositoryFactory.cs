namespace NHibernate.Carbon.Repository
{
    /// <summary>
    /// Contract for creating repositories based on type.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// This will create a <seealso cref="IRepository{T}"/> based on the type 
        /// and instantiate a default session to the data store using the  configuration 
        /// file in the executable directory.
        /// </summary>
        /// <typeparam name="T">Type to create the repository for.</typeparam>
        /// <returns>
        ///   <seealso cref="IRepository{T}"/>
        /// </returns>
        IRepository<T> CreateFor<T>()where T : class;

        /// <summary>
        /// This will create a <seealso cref="IRepository{T}"/> based on the type 
        /// and instantiate a default session to the data store using the  configuration 
        /// file in the executable directory.
        /// </summary>
        /// <typeparam name="T">Type to create the repository for.</typeparam>
        /// <param name="configuration">Full path to the configuration file for connecting to the data source</param>
        /// <returns>
        ///   <seealso cref="IRepository{T}"/>
        /// </returns>
        IRepository<T> CreateFor<T>(string configuration) where T : class;

    }
}