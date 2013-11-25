using System.Collections.Generic;
using NHibernate.Carbon.ForTesting;
using NHibernate.Carbon.Tests.Domain.OnlineShopping;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Xunit;
using Order = NHibernate.Carbon.Tests.Domain.OnlineShopping.Order;

namespace NHibernate.Carbon.Tests.AutoPersistance
{
	public class PersistanceWithBaseClassTests : BaseAutoPersistanceTestFixture
	{
		public PersistanceWithBaseClassTests()
		{
			// dyanmically creates the schema and mappings to conform to domain model:
			OneTimeInitalizeWithModel(TestConfigurator.GetModel());
			CreateUnitOfWork();
		}

		[Fact]
		public void can_save_customer_with_persistance_model()
		{
			var customer = new Customer();
			customer.ChangeName(new Name("John", "Smith"));

			using (var repository = OpenTransactedSession())
			{
				repository.Save<Customer>(customer);

				var fromDB = repository.FindById<Customer>(customer.Id);
				Assert.Equal(customer.Id, fromDB.Id);
			}
		}

		[Fact]
		public void can_save_product_with_persistance_model()
		{
			var product = new Product();
			product.ChangeProduct("Windex", "Window/Tile Cleaner", 24.95M);

			using (var repository = OpenTransactedSession())
			{
				repository.Save<Product>(product);

				var fromDB = repository.FindById<Product>(product.Id);
				Assert.Equal(product.Id, fromDB.Id); // "The identifier for the product could not be found."
			}
		}

		[Fact]
		public void Can_save_order_with_persistance_model()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));

			using (var repository = OpenTransactedSession())
			{
				repository.Save<Order>(order);

				var fromDB = repository.FindById<Order>(order.Id);
				Assert.Equal(order.Id, fromDB.Id); //  "The identifier for the order could not be found."
			}
		}

		[Fact]
		public void can_save_home_address_to_order()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
			order.ChangeHomeAddress("123 Street", "E 53rd", "New York", "NY", "54433");

			using (var repository = OpenTransactedSession())
			{
				repository.Save<Order>(order);

				var fromDB = repository.FindById<Order>(order.Id);
				Assert.Equal(order.HomeAddress.Id, fromDB.HomeAddress.Id);
			}
		}

		[Fact]
		public void can_save_shipping_address_to_order()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
			order.ChangeShippingAddress("123 Street", "E 53rd", "New York", "NY", "54433");

			using (var repository = OpenTransactedSession())
			{
				repository.Save<Order>(order);

				var fromDB = repository.FindById<Order>(order.Id);
				Assert.Equal(order.ShippingAddress.Id, fromDB.ShippingAddress.Id);
			}
		}

		[Fact]
		public void Can_save_line_item_to_order_with_persistance_model()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextSecond));
			OrderLine orderline = order.CreateOrderLine();

			Product product = orderline.CreateProduct();

			product.ChangeProduct("Windex", "Glass Cleaner", 8.95M);
			orderline.SaveProduct(product, 4);
			order.AddLine(orderline);

			using (var repository = OpenTransactedSession())
			{

				repository.Save<Order>(order);

				var fromDB = repository.FindById<Order>(order.Id);
				Assert.Equal(order.Id, fromDB.Id); // "The identifier for the order could not be found."

				// this will create a lazy load exception since the session to the repository is closed
				// need to create a criteria object to retrieve this on another session:

				/* EX:
				 *  select lines.* from orderlines lines inner join order  on order.id = lines.OrderId
				 */

				var orderlinesCriteria = DetachedCriteria.For<OrderLine>()
					.CreateCriteria("Orders", JoinType.InnerJoin)
					.Add(Restrictions.Eq("Id", fromDB.Id));

				var orderlines = orderlinesCriteria.GetExecutableCriteria(repository.CurrentSession).List<OrderLine>();

				Assert.Equal(1, orderlines.Count); // "The items for the order could not be retrieved."
			}
		}
	}
}