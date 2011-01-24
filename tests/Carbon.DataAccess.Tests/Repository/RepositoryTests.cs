using System;
using System.Collections.Generic;
using System.Linq;
using Carbon.DataAccess.Tests.Domain;
using Carbon.Repository.ForTesting;
using Carbon.Repository.Repository;
using NHibernate.Criterion;
using Xunit;

namespace Carbon.DataAccess.Tests.Repository
{
    public class RepositoryTests : BaseAutoPersistanceTestFixture
    {
        private IRepository<Product> _repository = null;

        public RepositoryTests()
        {
            var model = Configurator.GetModel();
            model.Build();
            model.CreateSchema();

            _repository = new NHibernateRepository<Product>(model.CurrentSessionFactory.OpenSession());
        }

        [Fact]
        public void Can_save_product_with_repository()
        {
            var product = new Product("Windex", "Window/Tile Cleaner", 24.95M);
            _repository.Persist(PersistanceAction.Save, product);

            var fromDB = _repository.FindById(product.Id);
            Assert.Equal(product.Id, fromDB.Id); // "The identifier for the product could not be found."
        }

        [Fact]
        public void Can_save_multiple_products_with_repository_and_return_paginated_list()
        {
            var pageSize = 5;
            var totalRecords = pageSize * 2;

            for (var index = 0; index < totalRecords; index++)
            {
                var product = new Product(index.ToString(), index.ToString(), index * 13.50M);
                _repository.Persist(PersistanceAction.Save, product);
            }

            // create the pagination (10 records with 5 records per page):
            var products = _repository.FindAll();
            var pagination = _repository.FindAllWithPagination(1, pageSize);

            // assert that we are on the first page and the total records are equal to the page size:
            Assert.Equal(true, pagination.HasNextPage);
            Assert.Equal(false, pagination.HasPreviousPage);
            Assert.Equal(pageSize, pagination.Count);

            // move to the second page:
            pagination = _repository.FindAllWithPagination(2, pageSize);

            // assert that we are on the end of the page (10 records with 5 records per page)
            Assert.Equal(false, pagination.HasNextPage);
            Assert.Equal(true, pagination.HasPreviousPage);
            Assert.Equal(pageSize, pagination.Count);
        }

        [Fact]
        public void Can_pass_in_custom_query_object_for_filtering_results_and_use_nhibernate_criteria_object_for_querying()
        {
            var pageSize = 5;
            var totalRecords = pageSize * 2;

            for (var index = 0; index < totalRecords; index++)
            {
                var product = new Product(index.ToString(), index.ToString(), index * 1.50M);
                _repository.Persist(PersistanceAction.Save, product);
            }

            var products = _repository.FindAll(new FindAllProductsLessThan10DollarsQuery());
            Assert.NotEqual(0, products.Count);
            Assert.Equal(7, products.Count);
        }


        [Fact]
        public void Can_pass_in_custom_query_object_for_filtering_results_and_use_linq_with_custom_collection_for_querying()
        {
            var pageSize = 5;
            var totalRecords = pageSize * 2;

            for (var index = 0; index < totalRecords; index++)
            {
                var product = new Product(index.ToString(), index.ToString(), index * 1.50M);
                _repository.Persist(PersistanceAction.Save, product);
            }

            var products = new FindAllProductsLessThan10DollarsQuery(_repository.FindAll()).Find();

            Assert.NotEqual(0, products.Count);
            Assert.Equal(7, products.Count);
        }

    }

    public class FindAllProductsLessThan10DollarsQuery : AbstractQuerySpecification<Product>
    {

        public FindAllProductsLessThan10DollarsQuery()
            : this(null)
        {

        }
        
        public FindAllProductsLessThan10DollarsQuery(IList<Product> products)
        {
            // using this variant will force the specification to do an in-memory 
            // filtering of the data based on your custom implementation:
            this.Source = products;
        }

        public override IList<Product> FilterInMemory()
        {
            // using LINQ for custom implementation:
            var products = (from product in Source where product.Price < 10.00M select product).ToList();
            return products;
        }

        public override IList<Product> FilterViaCriteria()
        {
            var criteria = DetachedCriteria.For<Product>()
                .Add(Restrictions.Lt("Price", 10.0M));

            var results = criteria.GetExecutableCriteria(this.GetSession()).List<Product>();
            return results;
        }

    }
}