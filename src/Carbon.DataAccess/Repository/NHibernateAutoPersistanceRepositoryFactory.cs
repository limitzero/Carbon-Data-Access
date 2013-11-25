using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.Repository
{
    /// <summary>
    /// Factory for creating repositories based on type from the 
    /// <seealso cref="IRepository{T}"/> concrete implementation
    /// that works specifically with the auto-persistance model.
    /// </summary>
    public class NHibernateAutoPersistanceRepositoryFactory : IRepositoryFactory
    {
        private readonly IAutoPersistanceModel _model = null;

        public NHibernateAutoPersistanceRepositoryFactory(IAutoPersistanceModel model)
        {
            _model = model;
        }

        public IRepository<T> CreateFor<T>() where T : class
        {
            NHibernateSessionManager.Instance.RegisterPersistanceModel(_model);
            var session = NHibernateSessionManager.Instance.GetSessionFor(_model.GetCurrentConfigurationFile());
            return new NHibernateRepository<T>(session);
        }

        public IRepository<T> CreateFor<T>(string configuration) where T : class
        {
			// force re-build of model if configuration file is passed:
			_model.ConfigurationFile(configuration);
			_model.Build();

            return CreateFor<T>();
        }
    }
}