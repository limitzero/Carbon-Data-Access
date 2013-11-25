using System;
using NHibernate.Carbon.AutoPersistance.Core;
using NHibernate.Carbon.Tests.Domain.OnlineShopping;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Xunit;
using Order = NHibernate.Carbon.Tests.Domain.OnlineShopping.Order;

namespace NHibernate.Carbon.Tests.AutoPersistance
{
	public class AutoPersistanceModelPersistanceTests
	{
		private readonly AutoPersistanceModel m_model;

		public AutoPersistanceModelPersistanceTests()
		{
			m_model = TestConfigurator.GetModel();
			m_model.ConfigurationFile(@"hibernate.cfg.xml");

			m_model.Build();
			m_model.CreateSchema();
		}

		[Fact]
		public void can_save_product_with_persistance_model()
		{
			using (var session = m_model.CurrentSessionFactory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					var product = new Product();
					product.ChangeProduct("Windex", "Window/Tile Cleaner", 24.95M);
					session.Save(product);
					txn.Commit();

					var fromDB = session.Get<Product>(product.Id);
					Assert.Equal(product.Id, fromDB.Id);
				}
				catch (Exception)
				{
					txn.Rollback();
					throw;
				}
			}
		}

		[Fact]
		public void can_save_order_with_persistance_model()
		{
			using (var session = m_model.CurrentSessionFactory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextDayAir));
					session.Save(order);
					txn.Commit();

					var fromDB = session.Get<Order>(order.Id);
					Assert.Equal(order.Id, fromDB.Id);
				}
				catch (Exception)
				{
					txn.Rollback();
					throw;
				}
			}
		}

		[Fact]
		public void Can_save_line_item_to_order_with_persistance_model()
		{
			Order order;

			using (var session = m_model.CurrentSessionFactory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextSecond));
					OrderLine orderline = order.CreateOrderLine();

					Product product = orderline.CreateProduct();
					product.ChangeProduct("Windex", "Window/Tile Cleaner", 24.95M);
					orderline.SaveProduct(product, 4);
					order.AddLine(orderline);

					session.Save(order);
					txn.Commit();

					var fromDB = session.Get<Order>(order.Id);

					var orderlinesCriteria = DetachedCriteria.For<OrderLine>()
						.CreateCriteria("Orders", JoinType.InnerJoin)
						.Add(Restrictions.Eq("Id", fromDB.Id));

					var orderlines = orderlinesCriteria.GetExecutableCriteria(session).List<OrderLine>();

					Assert.Equal(order.Id, fromDB.Id); //"The identifier for the order could not be found.");
					Assert.Equal(1, orderlines.Count); // "The items for the order could not be retrieved.");
				}
				catch (Exception)
				{
					txn.Rollback();
					throw;
				}
			}
		}

		[Fact]
		public void Can_save_orderline_with_persistance_model()
		{
			using (var session = m_model.CurrentSessionFactory.OpenSession())
			using (var txn = session.BeginTransaction())
			{
				try
				{
					var item = new OrderLine(null);
					Product product = item.CreateProduct();
					product.ChangeProduct("Windex", "Glass Cleaner", 8.95M);
					item.SaveProduct(product, 1);

					session.Save(item);
					txn.Commit();

					var fromDB = session.Get<OrderLine>(item.Id);
					Assert.Equal(item.Id, fromDB.Id); //"The identifier for the order line could not be found.");
				}
				catch (Exception)
				{
					txn.Rollback();
					throw;
				}
			}
		}
	}
}