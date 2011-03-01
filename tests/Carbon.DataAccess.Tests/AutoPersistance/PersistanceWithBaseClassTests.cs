using System.Collections.Generic;
using Carbon.DataAccess.Tests.Domain;
using Carbon.Repository.ForTesting;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Xunit;
using Order = Carbon.DataAccess.Tests.Domain.Order;

namespace Carbon.DataAccess.Tests.AutoPersistance
{
	public class PersistanceWithBaseClassTests : BaseAutoPersistanceTestFixture
	{
		public PersistanceWithBaseClassTests()
		{
			var model = Configurator.GetModel();
			//model.CacheMapping(); // maps already created..use the existing *.hbm.xml files.

			OneTimeInitalizeWithModel(Configurator.GetModel());
			CreateUnitOfWork();
		}

		[Fact]
		public void Can_save_product_with_persistance_model()
		{
			var product = new Product(null);
			product.ChangeProduct("Windex", "Window/Tile Cleaner", 24.95M);
			Repository.Save<Product>(product);

			var fromDB = Repository.FindById<Product>(product.Id);
			Assert.Equal(product.Id, fromDB.Id); // "The identifier for the product could not be found."
		}

		[Fact]
		public void Can_save_order_with_persistance_model()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
			Repository.Save<Order>(order);

			var fromDB = Repository.FindById<Order>(order.Id);
			Assert.Equal(order.Id, fromDB.Id); //  "The identifier for the order could not be found."
		}

		[Fact]
		public void can_save_home_address_to_order()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
			order.ChangeHomeAddress("123 Street", "E 53rd", "New York", "NY", "54433");

			Repository.Save<Order>(order);

			var fromDB = Repository.FindById<Order>(order.Id);
			Assert.Equal(order.HomeAddress.Id, fromDB.HomeAddress.Id);
		}

		[Fact]
		public void can_save_shipping_address_to_order()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
			order.ChangeShippingAddress("123 Street", "E 53rd", "New York", "NY", "54433");

			Repository.Save<Order>(order);

			var fromDB = Repository.FindById<Order>(order.Id);
			Assert.Equal(order.ShippingAddress.Id, fromDB.ShippingAddress.Id);
		}

		[Fact]
		public void Can_save_line_item_to_order_with_persistance_model()
		{
			var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextSecond));
			var orderline = order.CreateOrderLine();

			var product = orderline.CreateProduct();

			product.ChangeProduct("Windex", "Glass Cleaner", 8.95M);
			orderline.SaveProduct(product, 4);
			order.AddLine(orderline);
			
			Repository.Save<Order>(order);

			var fromDB = Repository.FindById<Order>(order.Id);
			Assert.Equal(order.Id, fromDB.Id); // "The identifier for the order could not be found."

			// this will create a lazy load exception since the session to the repository is closed
			// need to create a criteria object to retrieve this on another session:

			/* EX:
			 *  select lines.* from orderlines lines inner join order  on order.id = lines.OrderId
			 */ 

			var orderlinesCriteria = DetachedCriteria.For<OrderLine>()
												 .CreateCriteria("Order", JoinType.InnerJoin)
												 .Add(Restrictions.Eq("Id", fromDB.Id));

			IList<OrderLine> orderlines;
		    using(var session = Repository.GetSessionFactory().OpenSession())
		    {
		    	orderlines = orderlinesCriteria.GetExecutableCriteria(session).List<OrderLine>();
		    }

			Assert.Equal(1, orderlines.Count); // "The items for the order could not be retrieved."

		}

	}
}