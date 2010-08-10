using Carbon.DataAccess.Tests.Domain;
using Carbon.Repository.AutoPersistance.Core;
using Xunit;

namespace Carbon.DataAccess.Tests.AutoPersistance
{

    public class AutoPersistanceModelPersistanceTests
    {
        private AutoPersistanceModel m_model;

        public AutoPersistanceModelPersistanceTests()
        {
            m_model = Configurator.GetModel();
            m_model.CacheMapping();

            m_model.ConfigurationFile(@"hibernate.cfg.xml");

            m_model.Build();
            m_model.CreateSchema();
        }

        [Fact]
        public void Can_save_product_with_persistance_model()
        {
            using (var session = m_model.CurrentSessionFactory.OpenSession())
            using (var txn = session.BeginTransaction())
            {
                try
                {
                    var product = new Product("Windex", "Window/Tile Cleaner", 24.95M);
                    session.Save(product);
                    txn.Commit();

                    var fromDB = session.Get<Product>(product.Id);
                    Assert.Equal(product.Id, fromDB.Id);
                }
                catch (System.Exception)
                {
                    txn.Rollback();
                    throw;
                }
            }

        }

        [Fact]
        public void Can_save_order_with_persistance_model()
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
                catch (System.Exception)
                {
                    txn.Rollback();
                    throw;
                }
            }
        }

        [Fact]
        public void Can_save_line_item_to_order_with_persistance_model()
        {
            using (var session = m_model.CurrentSessionFactory.OpenSession())
            using (var txn = session.BeginTransaction())
            {
                try
                {
                    var order = new Order(new OrderStatus(OrderStatusEnum.Created), new OrderPriority(OrderPriorityEnums.NextSecond));
                    order.AddItem(new OrderLine(new Product("Windex", "Glass Cleaner", 8.95M), 4));

                    session.Save(order);
                    txn.Commit();

                    var fromDB = session.Get<Order>(order.Id);
                    Assert.Equal(order.Id, fromDB.Id);  //"The identifier for the order could not be found.");
                    Assert.Equal(1, fromDB.Orderlines.Count); // "The items for the order could not be retrieved.");
                }
                catch (System.Exception)
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
            using(var txn = session.BeginTransaction())
            {
                try
                {
                    var item = new OrderLine(new Product("Windex", "Glass Cleaner", 8.95M), 4);
                    session.Save(item);
                    txn.Commit();

                    var fromDB = session.Get<OrderLine>(item.Id);
                    Assert.Equal(item.Id, fromDB.Id); //"The identifier for the order line could not be found.");
                }
                catch (System.Exception)
                {
                    txn.Rollback();
                    throw;
                }

            }
        }

    }
}