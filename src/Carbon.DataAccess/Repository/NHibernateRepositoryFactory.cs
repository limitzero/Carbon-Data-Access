using System;

namespace Carbon.Repository.Repository
{
    /// <summary>
    /// Factory for creating repositories based on type from the 
    /// <seealso cref="IRepository{T}"/> concrete implementation.
    /// </summary>
    public class NHibernateRepositoryFactory : IRepositoryFactory
    {
        public IRepository<T> CreateFor<T>() where T : class
        {
            var session = NHibernateSessionManager.Instance.GetSessionFor(string.Empty);
            return new NHibernateRepository<T>(session); 
        }

        public IRepository<T> CreateFor<T>(string configuration) where T : class
        {
            var session = NHibernateSessionManager.Instance.GetSessionFor(configuration);
            return new NHibernateRepository<T>(session);
        }
    }
}