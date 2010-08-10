using Carbon.DataAccess.Tests.Domain;
using Carbon.Repository.AutoPersistance.Core;
using Carbon.Repository.Repository;
using Xunit;

namespace Carbon.DataAccess.Tests.Repository
{
    public class RepositoryFactoryTests
    {
        private IAutoPersistanceModel _model = null;
        private IRepositoryFactory _factory = null;

        public RepositoryFactoryTests()
        {
            _model = Configurator.GetModel();
            _model.Build();
            _model.CreateSchema();

            _factory = new NHibernateRepositoryFactory();
        }

        [Fact]
        public void Can_create_instance_of_reposistory_via_factory_with_session_injected()
        {
            // caveat: the mapping assembly must be noted in the *.cfg.xml file in order 
            // for persistance to work in this example!!!
            var repository = _factory.CreateFor<Product>(_model.GetCurrentConfigurationFile());
            Assert.True(typeof(IRepository<Product>).IsAssignableFrom(repository.GetType()));
        }

        [Fact]
        public void Can_create_instance_of_reposistory_via_factory_and_generate_exception_when_saving_entity_if_auto_persistance_configuration_is_used()
        {
            try
            {
                // caveat: the mapping assembly must be noted in the *.cfg.xml file in order 
                // for persistance to work in this example, this should generate an error if we try
                // to just specify the configuration file!!!
                var repository = _factory.CreateFor<Product>(_model.GetCurrentConfigurationFile());
                Assert.True(typeof(IRepository<Product>).IsAssignableFrom(repository.GetType()));

                var product = new Product("Windex", "Window/Tile Cleaner", 24.95M);

                // here is the error, no mapping assembly is defined when using the 
                // auto-persistance configuration and this will cause NHibernate 
                // not create a mapping instance for the entity because no information is stored.
                repository.Persist(PersistanceAction.Save, product); 
                
                // if it reaches here then we did something wrong:    
                Assert.Equal(1, product.Id);
            }
            catch (System.Exception exception)
            {
                Assert.Equal(typeof(NHibernate.MappingException), exception.GetBaseException().GetType());
            }
        }

        [Fact]
        public void Can_create_an_instance_of_repository_that_supports_auto_persistance_from_factory_and_save_entity()
        {
            // usage for DI, if needed:
            var factory = new NHibernateAutoPersistanceRepositoryFactory(_model);
            var repository = factory.CreateFor<Product>();

            var product = new Product("Windex", "Window/Tile Cleaner", 24.95M);
            repository.Persist(PersistanceAction.Save, product);

            Assert.Equal(1, product.Id);
        }
    }
}