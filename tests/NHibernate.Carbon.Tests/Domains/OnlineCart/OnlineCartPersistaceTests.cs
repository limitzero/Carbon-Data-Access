using System;
using NHibernate.Carbon.ForTesting;
using NHibernate.Carbon.Tests.Domains.OnlineCart.Model;
using Xunit;

namespace NHibernate.Carbon.Tests.Domains.OnlineCart
{
	public class OnlineCartPersistaceTests : BaseAutoPersistanceTestFixture
	{
		public OnlineCartPersistaceTests()
		{
			OneTimeInitalizeWithModel(new OnlineCartModelConventions().GetPersistanceModel());
			CreateUnitOfWork();
		}

		[Fact]
		public void can_save_customer()
		{
			var customer = this.CreateCustomer();

			using(var session = base.OpenTransactedSession())
			{
				session.Save<Customer>(customer);

				var fromDB = session.FindById<Customer>(customer.Id);

				Assert.Equal(customer.Id, fromDB.Id);
			}
		}

		private Customer CreateCustomer()
		{
			var customer = new Customer();
			customer.ChangeName(new Name("John", "Smith"));
			customer.ChangeBillingAddress(new BillingAddress("123 First Avenue", string.Empty, "New York", "NY", "45123"));
			customer.UseBillingAddressForShipping();
			return customer;
		}
	}
}