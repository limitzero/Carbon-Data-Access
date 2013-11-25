using System.Collections.Generic;
using System.Linq;
using NHibernate.Carbon.ForTesting;
using NHibernate.Carbon.Repository;
using NHibernate.Carbon.Tests.Domain.OnlineShopping;
using NHibernate.Criterion;
using Xunit;

namespace NHibernate.Carbon.Tests.Repository
{
	public class RepositoryTests : BaseAutoPersistanceTestFixture
	{
		private readonly IRepository<Product> _repository = null;

		public RepositoryTests()
		{
			var model = TestConfigurator.GetModel();
			model.Build();
			model.CreateSchema();

			_repository = model.GetRepositoryFor<Product>();
		}

		[Fact]
		public void Can_save_product_with_repository()
		{
			var product = new Product();
			product.ChangeProduct("Windex", "Window/Tile Cleaner", 24.95M);
			_repository.Persist(PersistanceAction.Save, product);

			var fromDB = _repository.FindById(product.Id);
			Assert.Equal(product.Id, fromDB.Id); // "The identifier for the product could not be found."
		}

		[Fact]
		public void Can_save_multiple_products_with_repository_and_return_paginated_list()
		{
			int pageSize = 5;
			int totalRecords = pageSize*2;

			for (int index = 0; index < totalRecords; index++)
			{
				var product = new Product();
				product.ChangeProduct(index.ToString(), index.ToString(), index*13.50M);
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
			int pageSize = 5;
			int totalRecords = pageSize*2;

			for (int index = 0; index < totalRecords; index++)
			{
				var product = new Product();
				product.ChangeProduct(index.ToString(), index.ToString(), index*1.50M);
				_repository.Persist(PersistanceAction.Save, product);
			}

			var products = _repository.FindAll(new FindAllProductsLessThan10DollarsQuery());
			Assert.NotEqual(0, products.Count);
			Assert.Equal(7, products.Count);
		}


		[Fact]
		public void Can_pass_in_custom_query_object_for_filtering_results_and_use_linq_with_custom_collection_for_querying()
		{
			int pageSize = 5;
			int totalRecords = pageSize*2;

			for (int index = 0; index < totalRecords; index++)
			{
				var product = new Product();
				product.ChangeProduct(index.ToString(), index.ToString(), index*1.50M);
				_repository.Persist(PersistanceAction.Save, product);
			}

			// this is the disconnected state for querying a collection of objects that may or may not have 
			// come from the underlying data store. usefull in unit testing the query object:
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